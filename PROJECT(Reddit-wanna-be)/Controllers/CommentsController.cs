using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using PROJECT_Reddit_wanna_be_.Models;
using WorkingWithAPIApplication.Contracts;
using WorkingWithAPIApplication.Repository;

namespace PROJECT_Reddit_wanna_be_.Controllers
{
    public class CommentsController : Controller
    {

        private readonly HttpClient _client;
        private readonly MethodController method;
        public CommentsController(HttpClient client, MethodController method)
        {
            _client = client;
            this.method = method;
        }
    
        [HttpPost]
        public async Task<IActionResult> CreateComment(Comments comment)
        {
            string jwtToken = await method.GetJwtTokenAsync(HttpContext);
            if (!string.IsNullOrEmpty(jwtToken))
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7030/");
                    method.ConfigureHttpClient(client, jwtToken);
                    string userId = method.GetUserIdFromJwtToken(jwtToken);
                    if (userId != null)
                    {
                        comment.UserID = int.Parse(userId);
                        string json = JsonConvert.SerializeObject(comment);
                        StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                        HttpResponseMessage response = await client.PostAsync("api/comments/Create", content);
                        var createdComment = await method.GetApiResponseAsync<PROJECT_Reddit_wanna_be_.Models.Comments>(response);
                        if (createdComment != null)
                        {
                            HttpResponseMessage getCommentResponse = await client.GetAsync($"api/comments/{createdComment.ID}");
                            var retrievedComment = await method.GetApiResponseAsync<PROJECT_Reddit_wanna_be_.Models.Comments>(getCommentResponse);
                            if (retrievedComment != null)
                            {
                                return RedirectToAction("GetComments", "Main", new { postId = retrievedComment.PostID });
                            }
                            else
                            {
                                Console.WriteLine($"Failed to retrieve the created comment.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Failed to create the comment.");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("No JWT token cookie found.");
            }
            return View(null);
        }

        [HttpGet]
        public async Task<IActionResult> GetCommentss(int postId)
        {
            ViewBag.PostId = postId;
            _client.BaseAddress = new Uri("https://localhost:7030/");
            string jwtToken = await method.GetJwtTokenAsync(HttpContext);
            if (!string.IsNullOrEmpty(jwtToken))
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7030/");
                    method.ConfigureHttpClient(client, jwtToken);
                    string userId = method.GetUserIdFromJwtToken(jwtToken);
                    if (userId != null)
                    {
                        ViewBag.UserID = int.Parse(userId);
                        HttpResponseMessage response = await client.GetAsync($"api/comments/PostID/{postId}");
                        var comments = await method.GetApiResponseAsync<List<PROJECT_Reddit_wanna_be_.Models.Comments>>(response);
                        if (comments != null)
                        {
                            return View("GetCommentss", comments);
                        }
                        else
                        {
                            Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("No JWT token cookie found.");
            }

            return View(new List<PROJECT_Reddit_wanna_be_.Models.Comments>());
        }


        public async Task<IActionResult> DeleteComment(int commentId, int postId)
        {
            string jwtToken = await method.GetJwtTokenAsync(HttpContext);
            if (!string.IsNullOrEmpty(jwtToken))
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7030/");
                    method.ConfigureHttpClient(client, jwtToken);
                    HttpResponseMessage response = await client.DeleteAsync($"api/comments/DELETE/{commentId}");
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("GetComments", "Main", new { postId = postId });
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
            return RedirectToAction("GetComments", "Main", new { postId = postId });
        }

        [HttpGet]
        public async Task<IActionResult> EditComment(int commentId)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7030/");
                HttpResponseMessage response = await client.GetAsync($"api/comments/{commentId}");
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var comment = JsonConvert.DeserializeObject<PROJECT_Reddit_wanna_be_.Models.Comments>(responseBody);
                    return View(comment);
                }
                else
                {
                    return View("Error");
                }
            }
        }

        public async Task<IActionResult> EditComment(int commentId, string Content)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7030/");
                string authToken = Request.Cookies["jwt"];
                if (!string.IsNullOrEmpty(authToken))
                {
                    JObject authTokenObject = JObject.Parse(authToken);
                    string jwtToken = authTokenObject["token"]?.ToString();

                    if (!string.IsNullOrEmpty(jwtToken))
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                        HttpResponseMessage response = await client.GetAsync($"api/comments/{commentId}");
                        if (response.IsSuccessStatusCode)
                        {
                            var responseBody = await response.Content.ReadAsStringAsync();
                            var comment = JsonConvert.DeserializeObject<PROJECT_Reddit_wanna_be_.Models.Comments>(responseBody);
                            Comments updatedComment = new Comments
                            {
                                Content = Content
                            };
                            HttpResponseMessage UpdatedCommentResponse = await client.PutAsJsonAsync($"api/comments/EDIT/{commentId}", updatedComment);
                            if (UpdatedCommentResponse.IsSuccessStatusCode)
                            {
                                var UpdatedCommentResponseBody = await response.Content.ReadAsStringAsync();
                                var UpdatedComment = JsonConvert.DeserializeObject<PROJECT_Reddit_wanna_be_.Models.Comments>(UpdatedCommentResponseBody);
                                return RedirectToAction("GetCommentss", "Comments", new { postId = UpdatedComment.PostID });
                            }
                        }
                        else
                        {
                            return View("Error");
                        }
                    }
                }
            }
            return View(null);
        }
    }
}
