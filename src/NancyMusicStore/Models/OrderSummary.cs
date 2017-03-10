using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NancyMusicStore.Models
{
    public class OrderSummary
    {
        public Order Order { get; set; }
        public IEnumerable<OrderSummaryDetail> OrderDetails { get; set; }
    }

    public class OrderSummaryDetail
    {
        public string Title { get; set; }
        public string Genre { get; set; }
        public string Artist { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
