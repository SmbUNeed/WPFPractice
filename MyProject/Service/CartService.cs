using MyProject.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyProject.Backend;

namespace MyProject.Service
{
    internal class CartService
    {
        public static CartService Instance { get; } = new CartService();

        public event Action CartChanged;

        // Текущий пользователь — подставь как ты его хранишь
        private int CurrentUserId => Auth.CurrentUser.Id;

        public int GetQuantity(int productId)
        {
            if (!Auth.IsAuthenticated) return 0;

            return Core.Context.Cart
                .FirstOrDefault(c => c.UserId == CurrentUserId && c.ProductId == productId)
                ?.Quantity ?? 0;
        }

        public void Add(int productId)
        {
            if (!Auth.IsAuthenticated) return;

            var item = Core.Context.Cart
                .FirstOrDefault(c => c.UserId == CurrentUserId && c.ProductId == productId);

            if (item == null)
            {
                Core.Context.Cart.Add(new Cart
                {
                    UserId = CurrentUserId,
                    ProductId = productId,
                    Quantity = 1
                });
            }
            else
            {
                item.Quantity++;
            }

            Core.Context.SaveChanges();
            CartChanged?.Invoke();
        }

        public void Remove(int productId)
        {
            if (!Auth.IsAuthenticated) return;

            var item = Core.Context.Cart
                .FirstOrDefault(c => c.UserId == CurrentUserId && c.ProductId == productId);

            if (item == null) return;

            if (item.Quantity <= 1)
                Core.Context.Cart.Remove(item);
            else
                item.Quantity--;

            Core.Context.SaveChanges();
            CartChanged?.Invoke();
        }
    }
}
