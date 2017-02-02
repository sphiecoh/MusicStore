using Nancy;
using Nancy.Authentication.Forms;
using Nancy.ModelBinding;
using NancyMusicStore.Common;
using NancyMusicStore.Models;
using System;
using System.Data;

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

            Get("/logon", _ =>
            {
                ViewBag.returnUrl = this.Request.Query["returnUrl"];
                return View["LogOn"];
            });

            Post("/logon", _ =>
            {
                var logonModel = this.Bind<LogOnModel>();

                string cmd = "public.get_user_by_name_and_password";
                var user = _dbHelper.QueryFirstOrDefault<SysUser>(cmd, new
                {
                    uname = logonModel.SysUserName,
                    upwd = logonModel.SysUserPassword
                }, null, null, CommandType.StoredProcedure);

                if (user == null)
                {
                    return View["LogOn"];
                }
                else
                {
                    MigrateShoppingCart(user.SysUserName);

                    var redirectUrl = string.IsNullOrWhiteSpace(logonModel.ReturnUrl) ? "/" : logonModel.ReturnUrl;
                    return this.LoginAndRedirect(Guid.Parse(user.SysUserId), fallbackRedirectUrl: redirectUrl);
                }
            });

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