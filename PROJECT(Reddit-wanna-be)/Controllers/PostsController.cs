using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using PROJECT_Reddit_wanna_be_.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace PROJECT_Reddit_wanna_be_.Controllers
{
    public class PostsController : Controller
    {

        private readonly HttpClient _client;
        private readonly MethodController method;

        public PostsController(HttpClient client, MethodController method)
        {
            _client = client;
            this.method = method;
        }
        [HttpGet]
        public async Task<IActionResult> PostByTopic(int topicId)
        {
            ViewBag.TopicID = topicId;
            string jwtToken = await method.GetJwtTokenAsync(HttpContext);
            if (!string.IsNullOrEmpty(jwtToken))
            {
                method.ConfigureHttpClient(jwtToken);
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.ReadJwtToken(jwtToken);
                var userIdClaim = token.Claims.FirstOrDefault(c => c.Type == "UserID");

                if (userIdClaim != null)
                {
                    string userId = userIdClaim.Value;
                    ViewBag.UserID = int.Parse(userId);
                    _client.BaseAddress = new Uri("https://localhost:7030/");
                    HttpResponseMessage response = await _client.GetAsync($"api/posts/PostsByTopic/{topicId}");
                    var posts = await method.GetApiResponseAsync<List<PROJECT_Reddit_wanna_be_.Models.Posts>>(response);
                    if (posts != null)
                    {
                        return View("PostByTopic", posts);
                    }
                }
            }
            return View(new List<PROJECT_Reddit_wanna_be_.Models.Posts>());
        }
        [HttpPost]
        public async Task<IActionResult> PostByTopic()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost(Posts post, int topicId)
        {
            _client.BaseAddress = new Uri("https://localhost:7030/");
            string jwtToken = await method.GetJwtTokenAsync(HttpContext);
            if (!string.IsNullOrEmpty(jwtToken))
            {
                method.ConfigureHttpClient(jwtToken);
                string userId = method.GetUserIdFromJwtToken(jwtToken);
                if (userId != null)
                {
                    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                    post.UserID = int.Parse(userId);
                    string json = JsonConvert.SerializeObject(post);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await _client.PostAsync("api/posts/Create", content);
                    
                    var createdPost = await method.GetApiResponseAsync<PROJECT_Reddit_wanna_be_.Models.Posts>(response);
                    if (createdPost != null)
                    {
                        HttpResponseMessage getPostResponse = await _client.GetAsync($"api/posts/{createdPost.Id}");
                        if (getPostResponse.IsSuccessStatusCode)
                        {
                            var retrievedPost = await method.GetApiResponseAsync<PROJECT_Reddit_wanna_be_.Models.Posts>(getPostResponse);
                            return RedirectToAction("PostByTopic", "Posts", new { topicId = topicId });
                        }
                        else
                        {
                            Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Failed to create post.");
                    }
                }
            }
            else
            {
                Console.WriteLine("No JWT token found.");
            }
            return View(null);
        }



        [HttpGet]
        public async Task<IActionResult> EditPost(int postId)
        {
            method.ConfigureHttpClient(_client, await method.GetJwtTokenAsync(HttpContext));
            _client.BaseAddress = new Uri("https://localhost:7030/");
            HttpResponseMessage response = await _client.GetAsync($"api/posts/{postId}");
            if (response.IsSuccessStatusCode)
            {
                var post = await method.GetApiResponseAsync<PROJECT_Reddit_wanna_be_.Models.Posts>(response);
                return View(post);
            }
            else
            {
                return View("Error");
            }
        }
        public async Task<IActionResult> EditPost(int postId, string Content)
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
                        HttpResponseMessage response = await client.GetAsync($"api/posts/{postId}");
                        if (response.IsSuccessStatusCode)
                        {
                            var responseBody = await response.Content.ReadAsStringAsync();
                            var post = JsonConvert.DeserializeObject<PROJECT_Reddit_wanna_be_.Models.Posts>(responseBody);
                            Posts updatedPost = post;
                            updatedPost.Content = Content;
                            HttpResponseMessage UpdatedPostResponse = await client.PutAsJsonAsync($"api/posts/EDIT/{postId}", updatedPost);
                            if (UpdatedPostResponse.IsSuccessStatusCode)
                            {
                                var UpdatedPostResponseBody = await response.Content.ReadAsStringAsync();
                                var UpdatedPost = JsonConvert.DeserializeObject<PROJECT_Reddit_wanna_be_.Models.Posts>(UpdatedPostResponseBody);
                                return RedirectToAction("PostByTopic", "Posts", new { topicId = UpdatedPost.TopicID });
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

        public async Task<IActionResult> DeleteCommentsByPost(int postId)
        {
            string jwtToken = await method.GetJwtTokenAsync(HttpContext);
            if (!string.IsNullOrEmpty(jwtToken))
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7030/");
                    method.ConfigureHttpClient(client, jwtToken);
                    HttpResponseMessage response = await client.DeleteAsync($"api/comments/DeleteCommentsByPostID/{postId}");
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("DeletePost", "Posts", new { postId = postId });
                    }
                    else
                    {
                        Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                    }
                }
            }
            return RedirectToAction("Posts");
        }

        public async Task<IActionResult> DeletePost(int postId, int topicId)
        {
            await DeleteCommentsByPost(postId);
            string jwtToken = await method.GetJwtTokenAsync(HttpContext);
            if (!string.IsNullOrEmpty(jwtToken))
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7030/");
                    method.ConfigureHttpClient(client, jwtToken);
                    HttpResponseMessage response = await client.DeleteAsync($"api/posts/DELETE/{postId}");
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("PostByTopic", "Posts", new { topicId = topicId });
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
