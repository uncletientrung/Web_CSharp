using Microsoft.AspNetCore.Mvc;
using Web_CSharp.Data;
using Web_CSharp.ViewModels;

namespace Web_CSharp.Controllers
{
    public class HangHoaController : Controller
    {
        private readonly Hshop2023Context db;

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

        public HangHoaController(Hshop2023Context context)
        {
            db = context;
        }
    }
}
