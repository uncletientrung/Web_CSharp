using Microsoft.AspNetCore.Mvc;

namespace Web_CSharp.Controllers
{
    public class HangHoaController : Controller
    {
        public IActionResult Index(int? loai)
        {
            return View();
        }
    }
}
