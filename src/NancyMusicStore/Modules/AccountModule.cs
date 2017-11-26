using Nancy;
using Nancy.ModelBinding;
using NancyMusicStore.Common;
using NancyMusicStore.Models;
using System;
using System.Data;
using Nancy.Owin;

namespace NancyMusicStore.Modules
{
    public class AccountModule : NancyModule
    {
        private readonly IDbHelper _dbHelper;
        private readonly ShoppingCart shoppingCart;
        public AccountModule(IDbHelper DbHelper, ShoppingCart shoppingCart) : base("/account")
        {
            _dbHelper = DbHelper;
            this.shoppingCart = shoppingCart;

            Get("/me",_ => new { User =  Context.GetUserName() });

            Get("/register", _ => View["Register"]);

            Post("/register", _ =>
            {
                var registerModel = this.Bind<RegisterModel>();

                string cmd = "public.add_user";
                _dbHelper.Execute(cmd, new
                {
                    uid = Guid.NewGuid().ToString(),
                    uname = registerModel.SysUserName,
                    upwd = registerModel.SysUserPassword,
                    uemail = registerModel.SysUserEmail
                }, null, null, CommandType.StoredProcedure);

                return Response.AsRedirect("~/");
            });
            Get("/signin",_ => {
                MigrateShoppingCart(this.Context.GetUserName());
                return null;
            });
            Get("/signout", _ => {
              
                
                return this.Response.AsRedirect("~/");
            });
        }

        private void MigrateShoppingCart(string UserName)
        {
            // Associate shopping cart items with logged-in user
            var cart = shoppingCart.GetCart(this.Context);

            cart.MigrateCart(UserName);
            Session[shoppingCart.CartSessionKey] = UserName;
        }
    }
}