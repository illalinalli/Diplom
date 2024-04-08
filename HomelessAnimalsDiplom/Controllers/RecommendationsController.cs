using Microsoft.AspNetCore.Mvc;

namespace HomelessAnimalsDiplom.Controllers
{
    public class RecommendationsController : Controller
    {
        public IActionResult Recommendations()
        {
            return View("RecommendationsSection");
        }
    }
}
