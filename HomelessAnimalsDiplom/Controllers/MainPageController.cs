using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static HomelessAnimalsDiplom.Controllers.HomeController;

namespace HomelessAnimalsDiplom.Controllers
{
    public class MainPageController : Controller
    {

        [HttpPost]
        public IActionResult AdvertisementView(string curTitle)
        {
            return RedirectToAction("MainPage", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            CurUser = new();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("Login");
        }

    }
}
