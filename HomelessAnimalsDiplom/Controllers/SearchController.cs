using HomelessAnimalsDiplom.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Drawing;
using static HomelessAnimalsDiplom.Views.Shared.Components.SearchComponent;

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
    }
}
