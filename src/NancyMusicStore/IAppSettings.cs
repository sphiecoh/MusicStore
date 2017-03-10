using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NancyMusicStore
{
    public class AppSettings
    {
         public bool EnableShipping { get; set; }
         public string ShippingApiUrl { get; set; }
         public string DatabaseConnection { get; set; }
    }
   
}
