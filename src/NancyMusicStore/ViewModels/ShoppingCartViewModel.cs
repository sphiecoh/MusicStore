using System.Collections.Generic;

namespace NancyMusicStore.ViewModels
{
    public class ShoppingCartViewModel
    {
        public List<CartViewModel> CartItems { get; set; }
        public decimal CartTotal { get; set; }
    }
}