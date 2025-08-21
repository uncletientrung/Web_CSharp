using Microsoft.AspNetCore.Mvc;
using Web_CSharp.Data;
using Web_CSharp.ViewModels;
using Web_CSharp.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace Web_CSharp.Controllers
{
    public class CartController : Controller
    {
        private readonly PaypalClient _paypalClinet;
        private readonly Hshop2023Context db;
        public CartController(Hshop2023Context context, PaypalClient paypalClient)
        {
            _paypalClinet=paypalClient;
            db = context; // Nhớ cho read-only-field để hết lỗi
        }
        public List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();
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
            HttpContext.Session.Set(MySetting.CART_KEY, giohang);
            return RedirectToAction("Index");
        }

        public IActionResult RemoveCard(int id)
        {
            var gioHang= Cart;
            var item = gioHang.SingleOrDefault( p=> p.MaHh==id);
            if(item != null)
            {
                gioHang.Remove(item);
                HttpContext.Session.Set(MySetting.CART_KEY, gioHang); 
            }
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public IActionResult Checkout()
        {
            if (Cart.Count == 0) {
                return Redirect("/");
            }
            ViewBag.PaypalClientId = _paypalClinet.ClientId;
            return View(Cart);
        }   
        [Authorize]
        [HttpPost]
        public IActionResult Checkout(CheckoutVM model)
        {
            if (ModelState.IsValid)
            {
                var customerID=HttpContext.User.Claims.SingleOrDefault(
                    p => p.Type == MySetting.CLAIM_CUSTOMERID).Value;
                // Lúc đăng nhập thì có tạo List Claim lưu trên máy gồm Email, Nam, CustomerID giờ lấy xài
                var khachHang = new KhachHang();
                if (model.GiongKhachHang)
                {
                    khachHang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == customerID);
                        // Giống select id
                }
                var hoadon = new HoaDon
                {
                    MaKh = customerID,
                    HoTen = model.HoTen ?? khachHang.HoTen,
                    DiaChi =model.DiaChi ?? khachHang.DiaChi,
                    DienThoai= model.DienThoai ?? khachHang.DienThoai,
                    NgayDat= DateTime.Now,
                    CachThanhToan= "COD",
                    CachVanChuyen ="GRAB",
                    MaTrangThai=0,
                    GhiChu= model.GhiChu
                };
                db.Database.BeginTransaction(); // Chuẩn bị các dữ liệu để push database
                try
                {
                    
                    db.Add(hoadon);
                    db.SaveChanges();
                    var cthds = new List<ChiTietHd>();
                    foreach( var item in Cart)
                    {
                        cthds.Add(new ChiTietHd
                        {
                            MaHd= hoadon.MaHd,
                            SoLuong=item.SoLuong,
                            DonGia=item.DonGia,
                            MaHh= item.MaHh,
                            GiamGia=0,
                        });
                    }
                    db.AddRange(cthds);
                    db.SaveChanges();

                    db.Database.CommitTransaction(); // Xác nhận push hết lên
                    HttpContext.Session.Set<List<CartItem>>(MySetting.CART_KEY, new List<CartItem>());
                    return View("Success");
                }
                catch (Exception ex) {
                    db.Database.RollbackTransaction(); // Nếu gặp lỗi làm mới transaction
                }
                
            }
            return View(Cart);
        }

        [Authorize]
        public IActionResult PaymentSuccess()
        {
            return View("Success");
        }


        #region Thanh toan Paypal
        [Authorize]
        [HttpPost("/Cart/create-paypal-order")]
        public async Task<IActionResult> CreatePaypalOrder(CancellationToken cancellationToken)
        {
            // Thông tin của đơn hàng gửi qua Paypal
            var tongTien = Cart.Sum(p => p.ThanhTien).ToString();
            var donViTienTe = "USD";
            var maDonHangThamChieu = "DH" + DateTime.Now.Ticks.ToString();
            try
            {
                var response = await _paypalClinet.CreateOrder(tongTien, donViTienTe, maDonHangThamChieu);
                return Ok(response);
            }catch (Exception ex)
            {
                var error = new { ex.GetBaseException().Message };
                return BadRequest(error);
            }
        }

        [Authorize]
        [HttpPost("/Cart/capture-paypal-order")]
        public async Task<IActionResult> CapturePaypalOrder(CancellationToken cancellationToken, string orderID)
        {
            try
            {
                var response= await _paypalClinet.CaptureOrder(orderID);
                // Lưu database đơn hàng của mình 
                return Ok(response);
            }
            catch (Exception ex) {
                var error = new { ex.GetBaseException().Message };
                return BadRequest(error);
            }



        #endregion


    }
}
