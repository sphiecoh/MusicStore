using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NancyMusicStore.Models
{
    public class OrderCreatedMessage
    {
        public Order Order { get; set; }
    }
}
