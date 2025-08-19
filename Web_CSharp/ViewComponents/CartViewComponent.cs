using Microsoft.AspNetCore.Mvc;
using Web_CSharp.Helpers;
using Web_CSharp.ViewModels;

namespace Web_CSharp.ViewComponents
{
    public class CartViewComponent : ViewComponent // ViewComponent ở using Microsoft.AspNetCore.Mvc;
    {
        public IViewComponentResult Invoke()
        {
            var cart= HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();
            return View("CartPanel", new CartModel
            {
                Quantity = cart.Sum(p => p.SoLuong),
                Total = cart.Sum(p => p.ThanhTien)
            });
        }
    }
}
