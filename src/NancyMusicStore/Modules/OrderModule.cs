using Nancy;
using Nancy.Security;
using NancyMusicStore.Common;
using NancyMusicStore.Models;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NancyMusicStore.Modules
{
    public class OrderModule : NancyModule
    {
        private readonly IDbHelper _dbHelper;
        public OrderModule(IDbHelper dbHelper): base("/order")
        {
            _dbHelper = dbHelper;
            this.RequiresAuthentication();

            Get("/",_ => GetOrderSummary());
            Get("/{id:int}", _ => GetOrderDetails((int)_.id));
        }

        private dynamic GetOrderSummary()
        {
            var summary = _dbHelper.Query<Order>(Queries.GetOrderList, new { username = Context.GetUserName()});
            return View["Summary",summary];
        }
        private dynamic GetOrderDetails(int orderId)
        {
            using (var conn = _dbHelper.Connection)
            {
               var detail = new OrderSummary();
               var reader =  conn.QueryMultiple(Queries.GetOrderDetails, new { oid = orderId});
               detail.Order =  reader.ReadSingle<Order>();
               detail.OrderDetails = reader.Read<OrderSummaryDetail>();
               return View["ViewOrder", detail];
            }
            
        }
    }
}
