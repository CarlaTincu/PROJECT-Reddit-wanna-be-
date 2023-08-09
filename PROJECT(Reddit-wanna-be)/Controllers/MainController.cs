using Azure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PROJECT_Reddit_wanna_be_.Models;
using PROJECT_Reddit_wanna_be_.Project.Data.Entities;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;

namespace PROJECT_Reddit_wanna_be_.Controllers
{
    public class MainController : Controller
    {
        
        public async Task<IActionResult> MainPage()
        {
            if (true)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7030/");
                    HttpResponseMessage response = await client.GetAsync("api/topics");
                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        var topicsList = JsonConvert.DeserializeObject<List<PROJECT_Reddit_wanna_be_.Project.Data.Entities.Topics>>(responseBody);
                        var model = new MainPageModel
                        {
                            TopicsList = topicsList
                        };

                        return View(model);
                    }
                    else
                    {
                        // Handle error
                        return View("Error");
                    }
                }
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> CreateTopic(Topics topic)
        //{
        //    if (true)
        //    {
        //        using (HttpClient client = new HttpClient())
        //        {
        //            client.BaseAddress = new Uri("https://localhost:7030/");

        //            string token = Request.Cookies["jwt"];
        //            var options = new CookieOptions
        //            {
        //                HttpOnly = true,
        //                Secure = true,
        //                SameSite = SameSiteMode.Strict
        //            };
        //            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //            Response.Cookies.Append("jwt", token, options);
        //            HttpResponseMessage response = await client.PostAsync("api/topics/Create", new StringContent(JsonConvert.SerializeObject(topic), Encoding.UTF8, "application/json"));
        //            if (response.IsSuccessStatusCode)
        //            {
        //                string responseBody = await response.Content.ReadAsStringAsync();
        //                var user = JsonConvert.DeserializeObject<PROJECT_Reddit_wanna_be_.Project.Data.Entities.Topics>(responseBody);
        //                return RedirectToAction("MainPage");
        //            }
        //            else
        //            {
        //                Console.WriteLine($"Request failed with status code: {response.StatusCode}");
        //            }
        //        }
        //    }
        //    return View(null);
        //}

        [HttpPost]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CreateTopic(Topics topic)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7030/");
                    string authToken = Request.Cookies["jwt"];

                    if (!string.IsNullOrEmpty(authToken))
                    {
                        try
                        {
                            JObject authTokenObject = JObject.Parse(authToken);
                            string jwtToken = authTokenObject["token"]?.ToString();

                            if (!string.IsNullOrEmpty(jwtToken))
                            {
                                // Use jwtToken as needed, for example:
                                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

                                string json = JsonConvert.SerializeObject(topic);
                                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                                HttpResponseMessage response = await client.PostAsync("api/topics/Create", content);

                                if (response.IsSuccessStatusCode)
                                {
                                    string responseBody = await response.Content.ReadAsStringAsync();
                                    var user = JsonConvert.DeserializeObject<PROJECT_Reddit_wanna_be_.Project.Data.Entities.Topics>(responseBody);
                                    return RedirectToAction("MainPage");
                                }
                                else
                                {
                                    Console.WriteLine($"Request failed with status code: {response.StatusCode}");

                                }
                            }
                            else
                            {
                                Console.WriteLine("JWT token value not found in the cookie.");
                            }
                        }
                        catch (JsonReaderException)
                        {
                            Console.WriteLine("Invalid JSON format in the cookie.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No JWT token cookie found.");
                    }
                 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
               
            }

            return View(null);
        }

        [HttpPost]
        public async Task<IActionResult> Topics() 
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> PostByTopic(int topicId)
        {
            ViewBag.TopicID = topicId;
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7030/");
                HttpResponseMessage response = await client.GetAsync($"api/posts/PostsByTopic/{topicId}");

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var posts = JsonConvert.DeserializeObject<List<PROJECT_Reddit_wanna_be_.Project.Data.Entities.Posts>>(responseBody);
                    return View("PostByTopic", posts); 
                }
                else
                {
                    Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                }
            }
            return View(new List<PROJECT_Reddit_wanna_be_.Project.Data.Entities.Posts>());
        }
        [HttpPost]
        public async Task<IActionResult> PostByTopic()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreatePost(Posts post,int topicId, int userId)
        {
            post.UserID = userId;
            if (true)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7030/");
                    HttpResponseMessage response = await client.PostAsync("api/posts/Create", new StringContent(JsonConvert.SerializeObject(post), Encoding.UTF8, "application/json"));

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        var user = JsonConvert.DeserializeObject<PROJECT_Reddit_wanna_be_.Project.Data.Entities.Topics>(responseBody);
                        return RedirectToAction("PostByTopic", new { topicId = topicId , userId = userId});
                    }
                    else
                    {
                        Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                    }
                }
            }
            return View(null);
        }


        [HttpPost]
        public async Task<IActionResult> CreateComment(Comments comment, int postId, int userId)
        {
           
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7030/");
                HttpResponseMessage response = await client.PostAsync("api/comments/Create", new StringContent(JsonConvert.SerializeObject(comment), Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var resultComment = JsonConvert.DeserializeObject<Comments>(responseBody);
                    return RedirectToAction("GetComments", new {postId = postId, userId = userId});
                }
                else
                {
                    Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                    return View(null);
                }
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetComments(int postId)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7030/");
                HttpResponseMessage response = await client.GetAsync($"api/comments/PostID/{postId}");

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var comments = JsonConvert.DeserializeObject<List<PROJECT_Reddit_wanna_be_.Project.Data.Entities.Comments>>(responseBody);
                    return View("GetComments", comments);
                }
                else
                {
                    Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                }
            }
            return View(new List<PROJECT_Reddit_wanna_be_.Project.Data.Entities.Comments>());
        }
     
    }
}
