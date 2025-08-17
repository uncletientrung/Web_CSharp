using Microsoft.AspNetCore.Mvc;
using Web_CSharp.Data;
using Web_CSharp.ViewModels;
namespace Web_CSharp.ViewComponents
{
    public class MenuLoaiViewComponent : ViewComponent
    {
        private readonly Hshop2023Context db;
        public MenuLoaiViewComponent(Hshop2023Context context) => db = context;

        public IViewComponentResult Invoke()
        {
            var data = db.Loais.Select(lo => new MenuLoaiVM
            {
                MaLoai = lo.MaLoai,
                TenLoai = lo.TenLoai,
                SoLuong = lo.HangHoas.Count
            }).OrderBy(p => p.SoLuong); // Sắp xếp giảm dần
            return View(data); // Default.cshtml đây là tên default của nó 
            //return View("Default",data ); 
        }
    }
}
