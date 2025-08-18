using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_CSharp.Data;
using Web_CSharp.ViewModels;

namespace Web_CSharp.Controllers
{
    public class HangHoaController : Controller
    {
        private readonly Hshop2023Context db;
        public HangHoaController(Hshop2023Context context)
        {
            db = context;
        }


        public IActionResult Index(int? loai)
        {
            var hangHoas = db.HangHoas.AsQueryable();
            if (loai.HasValue)
            {
                hangHoas = hangHoas.Where(p => p.MaLoai == loai.Value);
            }
            var result = hangHoas.Select(p => new HangHoaVM
            {
                MaHh = p.MaHh,
                TenHH = p.TenHh,
                DonGia = p.DonGia ?? 0,
                Hinh = p.Hinh ?? "",
                MoTaNgan = p.MoTaDonVi ?? "",
                TenLoai = p.MaLoaiNavigation.TenLoai // ????? MaLoaiNavigation là gì
            });
            return View(result);
        }
        public IActionResult Search(string? query)
        {
            var hangHoas = db.HangHoas.AsQueryable();
            if (query !=null)
            {
                hangHoas = hangHoas.Where(p => p.TenHh.Contains(query));
            }
            var result = hangHoas.Select(p => new HangHoaVM
            {
                MaHh = p.MaHh,
                TenHH = p.TenHh,
                DonGia = p.DonGia ?? 0,
                Hinh = p.Hinh ?? "",
                MoTaNgan = p.MoTaDonVi ?? "",
                TenLoai = p.MaLoaiNavigation.TenLoai // ????? MaLoaiNavigation là gì
            });
            return View(result);
        }

        public IActionResult Detail(int id)
        {
            var data = db.HangHoas.Include(p => p.MaLoaiNavigation).SingleOrDefault(p => p.MaHh == id);
                // Include dùng để load dữ liệu từ các bảng liên quan (eager loading).
                // SingleOrDefault Nó sẽ lấy đúng 1 phần tử nếu thỏa đk
                // Không có gì trả về null, nếu nhiều hơn 1 sẽ báo lỗi
            if (data == null)
            {
                TempData["Message"] = $"Không thấy sản phẩm có mã {id}"; //Tempdata giống biến toàn cục khi kích hoạt thì nó
                                                            // Sẽ xóa sau khi dùng
                return Redirect("/404");
            }
            var result =  new ChiTietHangHoaVM
            {
                MaHh = data.MaHh,
                TenHH = data.TenHh,
                DonGia = data.DonGia ?? 0,
                Hinh = data.Hinh ?? "",
                MoTaNgan = data.MoTaDonVi ?? "",
                TenLoai = data.MaLoaiNavigation.TenLoai,
                DiemDanhGia= 5,
                ChiTiet =data.MoTa ?? "",
                SoLuongTon =10
            };
            return View(result);
            
        }

    }
}
