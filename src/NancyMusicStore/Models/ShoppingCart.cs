using Nancy;
using NancyMusicStore.Common;
using NancyMusicStore.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace NancyMusicStore.Models
{
    public partial class ShoppingCart
    {
        private readonly IDbHelper _dbHelper;
        public ShoppingCart(IDbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public string ShoppingCartId { get; set; }

        public  string CartSessionKey = "CartId";

        public ShoppingCart GetCart(NancyContext context)
        {
            ShoppingCartId = GetCartId(context);
            return this;
        }

        public void AddToCart(Album album)
        {
            const string getItemCmd = "public.get_cart_item_by_cartid_and_albumid";
            var cartItem = _dbHelper.QueryFirstOrDefault<Cart>(getItemCmd, new
            {
                cid = ShoppingCartId,
                aid = album.AlbumId
            }, null, null, CommandType.StoredProcedure);
            string addToCartCmd = string.Empty;

            if (cartItem == null)
            {
                // Create a new cart item if no cart item exists
                AddCartItem(cartItem, album.AlbumId);
            }
            else
            {
                UpdateCartItem(cartItem);
            }
        }

        public int RemoveFromCart(int id)
        {
            const string getItemCmd = "public.get_cart_item_by_cartid_and_recordid";
            var cartItem = _dbHelper.QueryFirstOrDefault<Cart>(getItemCmd, new
            {
                cid = ShoppingCartId,
                rid = id
            }, null, null, CommandType.StoredProcedure);

            const int itemCount = 0;
            if (cartItem != null)
            {
                if (cartItem.Count > 1)
                {
                    UpdateCartItemCount(cartItem, itemCount);
                }
                else
                {
                    RemoveCartItem(cartItem.RecordId);
                }
            }
            return itemCount;
        }

        public void EmptyCart()
        {
            const string cmd = "public.delete_cart_item_by_cid";
            _dbHelper.Execute(cmd, new
            {
                cid = ShoppingCartId
            }, null, null, CommandType.StoredProcedure);
        }

        public List<CartViewModel> GetCartItems()
        {
            const string cmd = "public.get_cart_item_by_cid";
            return _dbHelper.Query<CartViewModel>(cmd, new
            {
                cid = ShoppingCartId
            }, null, true, null, CommandType.StoredProcedure).ToList();
        }

        public int GetCount()
        {
            const string cmd = "public.get_total_count_by_cartid";
            var res = _dbHelper.ExecuteScalar(cmd, new
            {
                cid = ShoppingCartId
            }, null, null, CommandType.StoredProcedure);

            return Convert.ToInt32(res);
        }

        public decimal GetTotal()
        {
            const string cmd = "public.get_total_order_by_cartid";
            var res = _dbHelper.ExecuteScalar(cmd, new
            {
                cid = ShoppingCartId
            }, null, null, CommandType.StoredProcedure);

            return res == null ? decimal.Zero : decimal.Parse(res.ToString());
        }

        public int CreateOrder(Order order)
        {
            decimal orderTotal = 0;

            var cartItems = GetCartItems();
            foreach (var item in cartItems)
            {
                AddOrderDetails(new OrderDetail
                {
                    AlbumId = item.AlbumId,
                    OrderId = order.OrderId,
                    UnitPrice = item.Price,
                    Quantity = item.Count
                });
                // Set the order total of the shopping cart
                orderTotal += item.Count * item.Price;
            }

            UpdateOrderTotal(order.OrderId, orderTotal);

            // Empty the shopping cart
            EmptyCart();

            // Return the OrderId as the confirmation number
            return order.OrderId;
        }

        public string GetCartId(NancyContext context)
        {
            if (context.Request.Session[CartSessionKey] == null)
            {
                if (context.CurrentUser != null)
                {
                    context.Request.Session[CartSessionKey] = context.CurrentUser.Identity.Name;
                }
                else
                {
                    Guid tempCartId = Guid.NewGuid();
                    context.Request.Session[CartSessionKey] = tempCartId.ToString();
                }
            }
            return context.Request.Session[CartSessionKey].ToString();
        }

        public void MigrateCart(string userName)
        {
            const string cmd = "public.update_cartid_by_recordids";
            _dbHelper.ExecuteScalar(cmd, new
            {
                ncid = userName,
                ocid = ShoppingCartId
            }, null, null, CommandType.StoredProcedure);
        }

        #region private method
        private void AddCartItem(Cart cartItem, int albumid)
        {
            cartItem = new Cart
            {
                AlbumId = albumid,//album.AlbumId,
                CartId = ShoppingCartId,
                Count = 1,
                DateCreated = DateTime.Now
            };
            const string addToCartCmd = "public.add_cart_item";
            _dbHelper.Execute(addToCartCmd, new
            {
                cid = cartItem.CartId,
                aid = cartItem.AlbumId,
                num = cartItem.Count,
                cdate = cartItem.DateCreated
            }, null, null, CommandType.StoredProcedure);
        }

        private void UpdateCartItem(Cart cartItem)
        {
            cartItem.Count++;
            const string addToCartCmd = "public.update_cart_item";
            _dbHelper.Execute(addToCartCmd, new
            {
                cid = cartItem.CartId,
                aid = cartItem.AlbumId,
                num = cartItem.Count
            }, null, null, CommandType.StoredProcedure);
        }

        private void UpdateCartItemCount(Cart cartItem, int itemCount)
        {
            cartItem.Count--;
            itemCount = cartItem.Count;

            const string cmd = "public.update_cart_count_by_recordid";
            _dbHelper.Execute(cmd, new
            {
                rid = cartItem.RecordId,
                num = cartItem.Count
            }, null, null, CommandType.StoredProcedure);
        }

        private void RemoveCartItem(int recordId)
        {
            const string cmd = "public.delete_cart_item_by_recordid";
            _dbHelper.Execute(cmd, new
            {
                rid = recordId
            }, null, null, CommandType.StoredProcedure);
        }

        private void AddOrderDetails(OrderDetail orderDetail)
        {
            const string createCmd = "public.add_order_details";
            _dbHelper.ExecuteScalar(createCmd, new
            {
                oid = orderDetail.OrderId,
                aid = orderDetail.AlbumId,
                qty = orderDetail.Quantity,
                uprice = orderDetail.UnitPrice
            }, null, null, CommandType.StoredProcedure);
        }

        private void UpdateOrderTotal(int orderId, decimal orderTotal)
        {
            const string updateCmd = "public.update_order_total_by_orderid";

            var res = _dbHelper.ExecuteScalar(updateCmd, new
            {
                t = orderTotal,
                oid = orderId
            }, null, null, CommandType.StoredProcedure);
        }
        #endregion
    }
}