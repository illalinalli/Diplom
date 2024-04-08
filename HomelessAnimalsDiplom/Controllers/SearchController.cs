using MonkeyShop.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using static MonkeyShop.Views.Shared.Components.SearchComponent;

namespace MonkeyShop.Controllers
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
            SearchItem searchItem = new SearchItem() { 
                Type = animalType.Id,
                Breed = breed.Id,
                Color = propertyValue.Id
            };
            return View("SearchView", searchItem);
        }
    }
}
