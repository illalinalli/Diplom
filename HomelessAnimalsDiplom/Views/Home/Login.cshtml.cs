
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace MonkeyShop.Views.Home
{
    public class LoginModel : PageModel
    {

        [BindProperty]
        [Required]
        public string? Login { get; set; }

        [BindProperty]
        [Required]
        public string? Password { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }

        private readonly HttpContext? _httpContext;
        public void OnGet()
        {
        }
        public LoginModel(HttpContext httpContext)
        {
            _httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
        }

        public LoginModel()
        {
            Login = string.Empty;
            Password = string.Empty;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                //var result = await UserManager.Login(Login, Password, HttpContext);
                //if (result.Success)
                //    return Redirect(ReturnUrl ?? "/");

                //ModelState.AddModelError("", result.ErrorMessage);
            }
            return Page();
        }
    }
}
