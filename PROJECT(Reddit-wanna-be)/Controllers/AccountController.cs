using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PROJECT_Reddit_wanna_be_.Models;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using RestSharp;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using WorkingWithAPIApplication.Entities;
using System.Net;

namespace PROJECT_Reddit_wanna_be_.Controllers
{
    public class AccountController : Controller
    {

        private readonly HttpClient _client;
        private readonly MethodController method;
        public AccountController(HttpClient client, MethodController method)
        {
            _client = client;
            this.method = method;
        }
       
        public IActionResult Login()
        {
            return View();
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

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7030/");
                HttpResponseMessage response = await client.GetAsync($"username/{model.UserName}");
                if(response.StatusCode != HttpStatusCode.NotFound)
                {
                    ModelState.AddModelError("UserName", "An account with this username already exists.");
                    return View();
                }
                else 
                {
                    response = await client.PostAsync("api/users/create", new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        var user = JsonConvert.DeserializeObject<PROJECT_Reddit_wanna_be_.Models.User>(responseBody);
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
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var userCookie = HttpContext.Request.Cookies["jwt"];
            var cookieValue = Uri.UnescapeDataString(userCookie);
            var userIdClaim = method.GetUserIdFromJwtToken(cookieValue);

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return View("Error");
            }
            method.ConfigureHttpClient(_client, await method.GetJwtTokenAsync(HttpContext));
            HttpResponseMessage response = await _client.GetAsync($"api/users/{userIdClaim}");
            if (response.IsSuccessStatusCode)
            {
                var user = await method.GetApiResponseAsync<PROJECT_Reddit_wanna_be_.Models.User>(response);
                return View(user);
            }
            else
            {
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(int ID, string username,string password, string email)
        {
            string jwtToken = await method.GetJwtTokenAsync(HttpContext);

            if (!string.IsNullOrEmpty(jwtToken))
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7030/");
                    method.ConfigureHttpClient(client, jwtToken);
                    HttpResponseMessage response = await client.GetAsync($"api/users/{ID}");
                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        var retrievedUser = JsonConvert.DeserializeObject<PROJECT_Reddit_wanna_be_.Models.User>(responseBody);
                        retrievedUser.Username = username;
                        retrievedUser.Password = password;
                        retrievedUser.Email = email;
                        HttpResponseMessage updatedUserResponse = await client.PutAsJsonAsync($"api/users/UPDATE/{retrievedUser.ID}", retrievedUser);
                        if (updatedUserResponse.IsSuccessStatusCode)
                        {
                            return RedirectToAction("MainPage", "Main");
                        }
                        else
                        {
                            return View("Error");
                        }
                    }
                }
            }

            return View("Error");
        }

        public async Task<IActionResult> DeleteProfile()
        {
            var userCookie = HttpContext.Request.Cookies["jwt"];
            var cookieValue = Uri.UnescapeDataString(userCookie);
            var userIdClaim = method.GetUserIdFromJwtToken(cookieValue);
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return View("Error");
            }
            string jwtToken = await method.GetJwtTokenAsync(HttpContext);
            if (!string.IsNullOrEmpty(jwtToken))
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7030/");
                    method.ConfigureHttpClient(client, jwtToken);
                    HttpResponseMessage response = await client.DeleteAsync($"api/users/DELETE/{userIdClaim}");
                    if (response.IsSuccessStatusCode)
                    {
                        Response.Cookies.Delete("jwt");
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                    }
                }
            }
            else
            {
                Console.WriteLine("No JWT token cookie found.");
            }
            return View(null);
        }

    }
}