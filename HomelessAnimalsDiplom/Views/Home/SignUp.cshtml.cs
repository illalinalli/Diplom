using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace HomelessAnimalsDiplom.Views.Home
{
    public class SignUpModel : PageModel
    {
        [BindProperty]
        [Required]
        public string? Name { get; set; }

        [BindProperty]
        [Required]
        public string? Login { get; set; }

        [BindProperty]
        [Required]
        public string? Password { get; set; }
        

        private readonly HttpContext? _httpContext;
        public void OnGet()
        {
        }
        public SignUpModel(HttpContext httpContext)
        {
            _httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
        }

        public SignUpModel()
        {
            Login = string.Empty;
            Password = string.Empty;
        }
    }
}
