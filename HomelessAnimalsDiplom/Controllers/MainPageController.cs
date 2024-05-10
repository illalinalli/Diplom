using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomelessAnimalsDiplom.Controllers
{
    public class MainPageController : Controller
    {
       
        /*   public IActionResult AdvertisementView()
           {
               return View("AdvertisementView");
           }*/

        [HttpPost]
        public IActionResult AdvertisementView(string curTitle)
        {
            return RedirectToAction("MainPage", "Home");
        }

    }
}
