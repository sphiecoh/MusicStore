using Nancy;
using Nancy.Json;
using Nancy.Json.Simple;
using Nancy.ModelBinding;
using Nancy.Security;
using NancyMusicStore.Common;
using NancyMusicStore.Models;
using System;
using System.Data;
using System.Net.Http;

namespace NancyMusicStore.Modules
{
    public class CheckOutModule : NancyModule
    {
        const string PromoCode = "FREE";
        private readonly IDbHelper _dbHelper;
        private readonly ShoppingCart shoppingCart;
        public CheckOutModule(HttpClient httpClient , IDbHelper helper , ShoppingCart shoppingCart , AppSettings settings) : base("/checkout")
        {
            _dbHelper = helper;
            this.shoppingCart = shoppingCart;

            this.RequiresAuthentication();

            Get("/addressandpayment", _ => View["AddressAndPayment",GetLastOrder(Context.GetUserName())]);

            Post("/addressandpayment",async _ =>
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
                        var httpContent = new StringContent(SimpleJson.SerializeObject(new { address = $"{order.Address} , {order.City} , {order.State} , {order.PostalCode}", ordernumber = oid, userid = order.Username }), System.Text.Encoding.UTF8, mediaType: "application/json");
                        var response = await httpClient.PostAsync("/shipping", httpContent);
                        response.EnsureSuccessStatusCode();
                        var result = SimpleJson.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
                        var rows = _dbHelper.ExecuteScalar(Queries.AddOrderShippingId, new { shipno = (int)result.id, oid = oid }, null, null, CommandType.Text);
                    }
                    string redirectUrl = string.Format("/checkout/complete/{0}", res.ToString());
                    return Response.AsRedirect(redirectUrl);
                }
                return View["AddressAndPayment"];
            });

            Get("/complete/{id:int}", _ =>
            {
                int id = _.id;

                string cmd = "public.get_order_count_by_uname_and_orderid";
                var res = _dbHelper.ExecuteScalar(cmd, new
                {
                    oid = id,
                    uname = this.Context.GetUserName().ToLower()
                }, null, null, CommandType.StoredProcedure);

                if (Convert.ToInt32(res) > 0)
                {
                    return View["Complete", id];
                }
                return View["Shared/Error"];
            });

          
        }

        private Order GetLastOrder(string username)
        {
            return _dbHelper.QueryFirstOrDefault<Order>(Queries.GetLastOrderAddressByUsername, new { username = username  }) ?? new Order();
        }
    }
}