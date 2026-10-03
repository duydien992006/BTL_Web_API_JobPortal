using Microsoft.AspNetCore.Mvc;

namespace APINguoiDung.Controllers
{
    public class KyNangController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
