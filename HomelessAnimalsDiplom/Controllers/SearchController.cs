using HomelessAnimalsDiplom.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Drawing;
using static HomelessAnimalsDiplom.Views.Shared.Components.SearchComponent;
using static HomelessAnimalsDiplom.Controllers.HomeController;

namespace HomelessAnimalsDiplom.Controllers
{
    public class SearchItem {
        public ObjectId Type;
        public ObjectId Breed;
        public ObjectId Color;
    }
    public class SearchController : Controller
    {
        public IActionResult SearchView()
        {
            SearchItem searchItem = new();
            //Type = animalType.Id,
            //    Breed = breed.Id,
            //    Color = propertyValue.Id
            return View("SearchView", searchItem);
        }
        public async Task<IActionResult> Logout()
        {
            CurUser = new();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("~/Login", "Search");
        }
    }
}
