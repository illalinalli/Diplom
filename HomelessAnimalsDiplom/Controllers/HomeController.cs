using HomelessAnimalsDiplom.Models;
using HomelessAnimalsDiplom.Views.Home;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Diagnostics;
using System.Security.Claims;
using static HomelessAnimalsDiplom.Models.Database;

namespace HomelessAnimalsDiplom.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        //private SignInManager<User>? _signInManager;
        public static User CurUser; 
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult Login()
        {
            LoginModel model = new();
            return View("Login", model);
        }
        public IActionResult MainPage()
        {
            return View("MainPage");
        }

        [HttpPost]
        public async Task<IActionResult> Login(string Login, string Password)
        {
            if (ModelState.IsValid)
            {
                MainPageModel mainPageModel = new();
                
                var httpContext = HttpContext;
                //var s = UserCollection?.Find(new BsonDocument()).ToList();
                var user = UserCollection?.Find(x => x.Login == Login).FirstOrDefault();
                mainPageModel.CurUser = user;
                CurUser = user;
                // проверяем на правильность логина и пароля пользователя
                if (user != null)
                {
                    var hash = GetHash(Password);
                    if (user.Password == hash)
                    {
                        var claims = new List<Claim>() { new Claim(ClaimTypes.Name, user.Login) };
                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        AuthenticationProperties authProps = new()
                        {
                            IsPersistent = true
                        };
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), authProps);
                        
                        return RedirectToAction("MainPage", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Неправильный пароль.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Такого пользователя не существует.");
                }
            }
            return View();
        }
        
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult SignUp()
        { 
            return View("SignUp");
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(string Name, string Login, string Password)
        {
            if (ModelState.IsValid && Login != null && Name != null && Password != null)
            {
                var httpContext = HttpContext;
                var hash = GetHash(Password);

                User user = new()
                {
                    Id = ObjectId.GenerateNewId(),
                    Name = Name,
                    Login = Login,
                    Password = hash,
                    Favorites = new()
                };
                // добавляем пользователя в БД
                var filter = Builders<User>.Filter.Eq("_id", user.Id);

                // выполнение операции upsert
                UserCollection?.ReplaceOneAsync(filter, user, ReplaceOptionsUpsert);

                CurUser = user;

                var claims = new List<Claim>() { new Claim(ClaimTypes.Name, user.Login) };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                AuthenticationProperties authProps = new()
                {
                    IsPersistent = true
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), authProps);
                return RedirectToAction("MainPage", "Home");
            }
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            CurUser = new();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("Login");
        }
        
    }
}