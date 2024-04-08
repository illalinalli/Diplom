using MonkeyShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace MonkeyShop.Views.Home
{
    public class MainPageModel : PageModel
    {
        public User? CurUser { get; set; }

        public void OnGet()
        {

        }
    }
}
