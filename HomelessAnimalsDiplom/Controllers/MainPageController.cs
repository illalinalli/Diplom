using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MonkeyShop.Controllers
{
    public class MainPageController : Controller
    {
       
        /*   public IActionResult AdvertisementView()
           {
               return View("AdvertisementView");
           }*/

        [HttpPost]
        public async Task<IActionResult> AdvertisementView(string curTitle)
        {
            return RedirectToAction("MainPage", "Home");
        }

    }
}
