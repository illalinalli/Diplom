using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static HomelessAnimalsDiplom.Controllers.HomeController;
namespace HomelessAnimalsDiplom.Controllers
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
