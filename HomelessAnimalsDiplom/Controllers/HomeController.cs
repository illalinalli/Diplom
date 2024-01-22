﻿using HomelessAnimalsDiplom.Models;
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
                var s = UserCollection?.Find(new BsonDocument()).ToList();
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}