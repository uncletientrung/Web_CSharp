using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Web_CSharp.Data;
using Web_CSharp.Helpers;
using Web_CSharp.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace Web_CSharp.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly Hshop2023Context db;
        private readonly IMapper _mapper; // Công cụ hỗ trợ Helper/IMapper

        public KhachHangController(Hshop2023Context context, IMapper mapper) {
            db = context;
            _mapper = mapper;
        }

        #region Dang Ky
        [HttpGet] // GET request → hiển thị form
        public IActionResult DangKy()
        {
            return View();
        }

        [HttpPost] // POST request → xử lý dữ liệu submit
        public IActionResult DangKy(RegisterVM model, IFormFile Hinh) // IFormFile Hinh là hình gửi vào khi đăng ký
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var khachhang = _mapper.Map<KhachHang>(model);
                    khachhang.RandomKey = MyUntil.GenerateRamdomKey();
                    khachhang.MatKhau = model.MatKhau.ToMd5Hash(khachhang.RandomKey);
                    khachhang.HieuLuc = true; // Xử lý khu dùng mail để active
                    khachhang.VaiTro = 0;
                    if (Hinh != null)
                    {
                        khachhang.Hinh = MyUntil.UploadHinh(Hinh, "KhachHang");
                    }
                    db.Add(khachhang);
                    db.SaveChanges();
                    return RedirectToAction("Index", "HangHoa");
                }
                catch (Exception ex)
                {
                    var mess = $"{ex.Message} shh";
                }
            }

            return View();
        }
        #endregion

        #region Login
        [HttpGet]
        public IActionResult DangNhap(string? ReturnUrl)
        {
            ViewBag.ReturnUrl= ReturnUrl;
                // ViewBag là một object động (dynamic object) dùng để truyền dữ liệu tạm thời từ Controller sang View.
                // Dữ liệu chỉ tồn tại trong một request (Controller → View), sang request khác là mất
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DangNhap(LoginVM model, string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            if(ModelState.IsValid) // Nếu model hợp lệ
            {
                var khachHang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == model.UserName);
                if(khachHang == null)
                {
                    ModelState.AddModelError("loi", "Không có khách hàng này");
                }
                else
                {
                    if (!khachHang.HieuLuc)
                    {
                        ModelState.AddModelError("loi", "Tài khoản đã bị khóa, Vui lòng liên hệ Uncletientrung");
                    }
                    else
                    {
                        if(khachHang.MatKhau != model.Password.ToMd5Hash(khachHang.RandomKey))
                        {
                            ModelState.AddModelError("loi", "Sai thông tin đăng nhập");
                        }
                        else
                        {
                            // Ghi nhận thành công
                            var claims = new List<Claim> { 
                                new Claim(ClaimTypes.Email, khachHang.Email),
                                new Claim(ClaimTypes.Name, khachHang.HoTen),
                                new Claim(MySetting.CLAIM_CUSTOMERID, khachHang.MaKh),

                                // claim - role động
                                new Claim(ClaimTypes.Role, "Customer")
                            };
                            //Dùng các hàm có sẵn trên mạng của Microsoft
                            var claimsIdentity = new ClaimsIdentity(claims, 
                                CookieAuthenticationDefaults.AuthenticationScheme);
                            var claimPrincipal = new ClaimsPrincipal(claimsIdentity);
                            await HttpContext.SignInAsync(claimPrincipal);
                            if (Url.IsLocalUrl(ReturnUrl))
                            {
                                return Redirect(ReturnUrl);
                            }
                            else
                            {
                                return Redirect("/");
                            }
                        }
                    }
                }
            }    
            return View();
        }

        #endregion
        [Authorize]
            // Vì await HttpContext.SignInAsync(claimPrincipal); đã tạo ra 1 cookie khi đăng nhập thành công
            // Authorize sẽ kiểm tra có chưa nếu nó thì cho truy cập vào nếu chưa thì không cho vào 
        public IActionResult Profile()
        {

            return View();
        }

        [Authorize]
        public async Task<IActionResult> DangXuat()
        {

            await HttpContext.SignOutAsync();
            return Redirect("/");
        }

    }
}
