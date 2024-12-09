using Microsoft.AspNetCore.Mvc;

namespace OdevDagitimUI.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
