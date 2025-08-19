using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Web_CSharp.Data;
using Web_CSharp.Helpers;
using Web_CSharp.ViewModels;

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
                var khachhang = _mapper.Map<KhachHang>(model);
                khachhang.RandomKey=MyUntil.GenerateRamdomKey();
                khachhang.MatKhau=model.MatKhau.ToMd5Hash(khachhang.RandomKey);
                khachhang.HieuLuc = true; // Xử lý khu dùng mail để active
                khachhang.VaiTro = 0;

                if(Hinh != null)
                {
                    khachhang.Hinh = MyUntil.UploadHinh(Hinh, "KhachHang");
                }
                db.Add(khachhang);
                db.SaveChanges();
                return RedirectToAction("Index", "HangHoa");
            }

            return View();
        }

    }
}
