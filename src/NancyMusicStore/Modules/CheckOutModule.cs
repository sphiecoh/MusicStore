using Nancy;
using Nancy.Json;
using Nancy.Json.Simple;
using Nancy.ModelBinding;
using Nancy.Security;
using NancyMusicStore.Common;
using NancyMusicStore.Messaging;
using NancyMusicStore.Models;
using Serilog;
using System;
using System.Data;
using System.Net.Http;
using System.Threading.Tasks;
using NServiceBus;

namespace NancyMusicStore.Modules
{
    public class CheckOutModule : NancyModule
    {
        const string PromoCode = "FREE";
        private readonly IDbHelper _dbHelper;
        private readonly ShoppingCart shoppingCart;
        private readonly AppSettings settings;
        private readonly HttpClient httpClient;
        private IMessageSession publisher;
        public CheckOutModule(HttpClient httpClient , IDbHelper helper , ShoppingCart shoppingCart , AppSettings settings) : base("/checkout")
        {
            _dbHelper = helper;
            this.shoppingCart = shoppingCart;
            this.httpClient = httpClient;
            this.settings = settings;
            //this.publisher = publisher;

            this.RequiresAuthentication();

            Get("/addressandpayment", _ => View["AddressAndPayment",GetLastOrder(Context.GetUserName())]);

            Post("/addressandpayment",_ => ProcessOrder());

            Get("/complete/{id:int}", _ => CompleteOrder((int)_.id));

        }

        private dynamic CompleteOrder(int orderId)
        {

            string cmd = "public.get_order_count_by_uname_and_orderid";
            var res = _dbHelper.ExecuteScalar(cmd, new
            {
                oid = orderId,
                uname = this.Context.GetUserName().ToLower()
            }, null, null, CommandType.StoredProcedure);

            if (Convert.ToInt32(res) > 0)
            {
                return View["Complete", orderId];
            }
            return View["Shared/Error"];
        }

        private Order GetLastOrder(string username)
        {
            return _dbHelper.QueryFirstOrDefault<Order>(Queries.GetLastOrderAddressByUsername, new { username = username  }) ?? new Order();
        }
        private dynamic ProcessOrder()
        {
            var order = this.Bind<Order>();
            order.Username = this.Context.GetUserName();
            order.OrderDate = DateTime.UtcNow;

            string cmd = "public.add_order";
            var res = _dbHelper.ExecuteScalar(cmd, new
            {
                odate = order.OrderDate,
                uname = order.Username,
                fname = Context.GetFirstName(),
                lname = Context.GetLastName(),
                adr = order.Address,
                cn = order.City,
                sn = order.State,
                pcode = order.PostalCode,
                cname = order.Country,
                ph = order.Phone,
                ea = order.Email,
                t = order.Total
            }, null, null, CommandType.StoredProcedure);

            if (Convert.ToInt32(res) != 0)
            {
                order.OrderId = Convert.ToInt32(res);
                var cart = shoppingCart.GetCart(this.Context);
                var oid = cart.CreateOrder(order);

                //Call shipping service
                if (settings.EnableShipping)
                {
                    var correlation = Guid.NewGuid().ToString();
                    var message = new { address = $"{order.Address} , {order.City} , {order.State} , {order.PostalCode}", ordernumber = oid, userid = order.Username };
                    Log.Logger.Information("Sending message {ID} for order #{order} created by {user}", correlation,oid,order.Username);
                    //publisher.Publish<OrderSubmitted>( evt =>{
                    //    evt.OrderId = order.OrderId;
                    //    evt.Username = order.Username;
                    //} );
                   
                }
                string redirectUrl = string.Format("/checkout/complete/{0}", res.ToString());
                return Response.AsRedirect(redirectUrl);
            }
            return View["AddressAndPayment"];
        }
    }
}