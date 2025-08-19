using Microsoft.AspNetCore.Mvc;
using Web_CSharp.Data;
using Web_CSharp.ViewModels;
using Web_CSharp.Helpers;

namespace Web_CSharp.Controllers
{
    public class CartController : Controller
    {
        private readonly Hshop2023Context db;
        const string CART_KEY = "MYCART";
        public CartController(Hshop2023Context context)
        {
            db = context; // Nhớ cho read-only-field để hết lỗi
        }
        public List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>(CART_KEY) ?? new List<CartItem>();
                // Get nay là Get bên helper có truyền kiểu dữ liệu vào Get<T>
        public IActionResult Index()
        {
            return View(Cart);
        }
        public IActionResult AddToCart(int id, int quantity = 1) {
            var giohang = Cart;
            var item = giohang.SingleOrDefault(p => p.MaHh == id);
            if (item == null) {
                var hangHoa = db.HangHoas.SingleOrDefault(p => p.MaHh == id);
                if(hangHoa == null) // Trường hợp sửa domain để vào
                {
                    TempData["Message"] = $"Không tìm thấy hàng hóa có mã {id}";
                    return Redirect("/404");
                }
                item =new CartItem
                {
                    MaHh=hangHoa.MaHh,
                    TenHH=hangHoa.TenHh,
                    DonGia=hangHoa.DonGia ?? 0,
                    Hinh =hangHoa.Hinh ?? string.Empty,
                    SoLuong = quantity
                };
                giohang.Add(item);
            }
            else 
            {
                item.SoLuong += quantity;
            }
            HttpContext.Session.Set(CART_KEY, giohang);
            return RedirectToAction("Index");
        }

        public IActionResult RemoveCard(int id)
        {
            var gioHang= Cart;
            var item = gioHang.SingleOrDefault( p=> p.MaHh==id);
            if(item != null)
            {
                gioHang.Remove(item);
                HttpContext.Session.Set(CART_KEY, gioHang); 
            }
            return RedirectToAction("Index");
        }
    }
}
