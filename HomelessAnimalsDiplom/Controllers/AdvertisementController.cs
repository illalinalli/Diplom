using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static MonkeyShop.Controllers.HomeController;
namespace MonkeyShop.Controllers
{
    public class AdvertisementController : Controller
    {
        public IActionResult AdvertisementView()
        {
            return View("AdvertisementView", CurUser);
        }

        // подробные описания публикации
        public IActionResult AdvertisementDetailView()
        {
            return View("AdvertisementDetailView");
        }
    }
}
