﻿using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PROJECT_Reddit_wanna_be_.Models;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using RestSharp;

namespace PROJECT_Reddit_wanna_be_.Controllers
{
    public class AccountController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }
        private Guid GetCurrentUserToken()
        {
            var claim = HttpContext.User.Claims;
            var userTokenClaim = claim.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userTokenClaim != null && Guid.TryParse(userTokenClaim.Value, out Guid userToken))
            {
                return userToken;
            }
            return Guid.Empty;
        }

        [HttpPost]
        public async Task<IActionResult> Login(string UserName, string Password)
        
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7030/");
                var requestBody = new { UserName, Password };
                var response = await client.PostAsJsonAsync("api/Login", requestBody);

                if (response.IsSuccessStatusCode)
                {
                    var token = await response.Content.ReadAsStringAsync();
                    var options = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict
                    };
                    Response.Cookies.Append("jwt", token, options);
                    return RedirectToAction("MainPage", "Main");
                }
                else
                {
                    Console.WriteLine($"Authentication failed with status code: {response.StatusCode}");
                }
            }
            return View(null); 
        }
        [HttpGet]
        public IActionResult Logout()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogoutConfirmed()
        {
            Response.Cookies.Delete("jwt");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (true)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7030/");
                    HttpResponseMessage response = await client.PostAsync("api/users/create", new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        var user = JsonConvert.DeserializeObject<PROJECT_Reddit_wanna_be_.Project.Data.Entities.User>(responseBody);
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                    }
                }
            }
            return View(null);
        }

    }
}