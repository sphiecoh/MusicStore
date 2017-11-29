using Nancy;
using Nancy.ModelBinding;
using NancyMusicStore.Common;
using NancyMusicStore.Models;
using NancyMusicStore.ViewModels;
using System.Data;

namespace NancyMusicStore.Modules
{
    public class ShopCartModule : NancyModule
    {
        private readonly IDbHelper _dbHelper;
        private readonly ShoppingCart shoppingCart;
        public ShopCartModule(IDbHelper dbHelper , ShoppingCart shoppingCart) : base("/shoppingcart")
        {
            _dbHelper = dbHelper;
            this.shoppingCart = shoppingCart;

            Get("/cartsummary", _ =>
            {
                var cart = shoppingCart.GetCart(this.Context);
                return Response.AsJson(cart.GetCount());
            });

            Post("/addtocart/{id:int}", _ =>
            {
                if (int.TryParse(_.id, out int id))
                {
                    string cmd = "public.get_album_by_aid";
                    var addedAlbum = _dbHelper.QueryFirstOrDefault<Album>(cmd, new
                    {
                        aid = id
                    }, null, null, CommandType.StoredProcedure);

                    var cart = shoppingCart.GetCart(this.Context);
                    cart.AddToCart(addedAlbum);
                }
                return 200;
            });

            Get("/index", _ =>
            {
                var cart = shoppingCart.GetCart(this.Context);

                // Set up our ViewModel
                var viewModel = new ShoppingCartViewModel
                {
                    CartItems = cart.GetCartItems(),
                    CartTotal = cart.GetTotal()
                };

                // Return the view
                return View["Index", viewModel];
            });

            Post("/removefromcart", _ =>
            {
                var vm = this.Bind<ShoppingCartRemoveRequestViewModel>();
                string albumName = string.Empty;
                return Response.AsJson(GetRemoveResult(vm.Id, albumName));
            });
        }

        private ShoppingCartRemoveViewModel GetRemoveResult(int rid, string albumName)
        {
            int itemCount = 0;

            // Remove the item from the cart
            var cart = shoppingCart.GetCart(this.Context);

            string cmd = "public.get_album_title_by_recordid";
            var res = _dbHelper.ExecuteScalar(cmd, new
            {
                rid = rid
            }, null, null, CommandType.StoredProcedure);

            if (res != null)
            {
                albumName = res.ToString();
                itemCount = cart.RemoveFromCart(rid);
            }

            return new ShoppingCartRemoveViewModel
            {
                Message = albumName + " has been removed from your shopping cart.",
                CartTotal = cart.GetTotal(),
                CartCount = cart.GetCount(),
                ItemCount = itemCount,
                DeleteId = rid
            };
        }
    }
}