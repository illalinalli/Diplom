using HomelessAnimalsDiplom.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace HomelessAnimalsDiplom.Views.Home
{
    public class MainPageModel : PageModel
    {
        public User? CurUser { get; set; }

        public void OnGet()
        {

        }
    }
}
