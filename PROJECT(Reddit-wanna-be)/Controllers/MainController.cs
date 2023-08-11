using Azure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PROJECT_Reddit_wanna_be_.Models;
using PROJECT_Reddit_wanna_be_.Project.Data.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using WorkingWithAPIApplication.Entities;

namespace PROJECT_Reddit_wanna_be_.Controllers
{

    public class MainController : Controller
    {



        //TOPICS
        public async Task<IActionResult> MainPage()
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


        [HttpPost]
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

        //POSTS

        [HttpGet]
        public async Task<IActionResult> PostByTopic(int topicId)
        {
            ViewBag.TopicID = topicId;

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
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var token = tokenHandler.ReadJwtToken(jwtToken);
                        var userIdClaim = token.Claims.FirstOrDefault(c => c.Type == "UserID");

                        if (userIdClaim != null)
                        {
                            string userId = userIdClaim.Value;
                            ViewBag.UserID = int.Parse(userId);
                            client.BaseAddress = new Uri("https://localhost:7030/");
                            HttpResponseMessage response = await client.GetAsync($"api/posts/PostsByTopic/{topicId}");

                            if (response.IsSuccessStatusCode)
                            {
                                string responseBody = await response.Content.ReadAsStringAsync();
                                var posts = JsonConvert.DeserializeObject<List<PROJECT_Reddit_wanna_be_.Models.Posts>>(responseBody);
                                return View("PostByTopic", posts);
                            }
                        }
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

                                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

                                var tokenHandler = new JwtSecurityTokenHandler();
                                var token = tokenHandler.ReadJwtToken(jwtToken);


                                var userIdClaim = token.Claims.FirstOrDefault(c => c.Type == "UserID");
                                if (userIdClaim != null)
                                {
                                    string userId = userIdClaim.Value;
                                    post.UserID = int.Parse(userId);
                                    string json = JsonConvert.SerializeObject(post);
                                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                                    HttpResponseMessage response = await client.PostAsync("api/posts/Create", content);

                                    if (response.IsSuccessStatusCode)
                                    {

                                        string responseBody = await response.Content.ReadAsStringAsync();
                                        var createdPost = JsonConvert.DeserializeObject<PROJECT_Reddit_wanna_be_.Models.Posts>(responseBody);
                                        int postId = createdPost.Id;

                                        HttpResponseMessage getPostResponse = await client.GetAsync($"api/posts/{postId}");

                                        if (getPostResponse.IsSuccessStatusCode)
                                        {
                                            string getPostResponseBody = await getPostResponse.Content.ReadAsStringAsync();
                                            var retrievedPost = JsonConvert.DeserializeObject<PROJECT_Reddit_wanna_be_.Models.Posts>(getPostResponseBody);

                                            return RedirectToAction("PostByTopic", "Main", new { topicId = topicId });
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
            return View(null);
        }
        [HttpGet]
        public async Task<IActionResult> EditPost(int postId)
        {

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7030/");
                HttpResponseMessage response = await client.GetAsync($"api/posts/{postId}");
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();

                    var post = JsonConvert.DeserializeObject<PROJECT_Reddit_wanna_be_.Models.Posts>(responseBody);

                    return View(post);
                }
                else
                {
                    // Handle error
                    return View("Error");
                }
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
                                return RedirectToAction("PostByTopic", "Main", new { topicId = UpdatedPost.TopicID });
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
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

                            HttpResponseMessage response = await client.DeleteAsync($"api/comments/DeleteCommentsByPostID/{postId}");

                            if (response.IsSuccessStatusCode)
                            {
                                // Deletion was successful
                                return RedirectToAction("DeletePost", "Main", new {postId = postId});   
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

            return RedirectToAction("MainPage");
        }
        public async Task<IActionResult> DeletePost(int postId, int topicId)
        {

            using (HttpClient client = new HttpClient())
            {
                await DeleteCommentsByPost(postId);
                client.BaseAddress = new Uri("https://localhost:7030/");
                string authToken = Request.Cookies["jwt"];
                if (!string.IsNullOrEmpty(authToken))
                {
                    JObject authTokenObject = JObject.Parse(authToken);
                    string jwtToken = authTokenObject["token"]?.ToString();
                    if (!string.IsNullOrEmpty(jwtToken))
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var token = tokenHandler.ReadJwtToken(jwtToken);
                        var userIdClaim = token.Claims.FirstOrDefault(c => c.Type == "UserID");
                        HttpResponseMessage response = await client.DeleteAsync($"api/posts/DELETE/{postId}");
                        if (response.IsSuccessStatusCode)
                        {
                            return RedirectToAction("PostByTopic", "Main", new { topicId = topicId });
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
                else
                {
                    Console.WriteLine("No JWT token cookie found.");
                }
            }
            return View(null);
        }




        //COMMENTS

        [HttpPost]
        public async Task<IActionResult> CreateComment(Comments comment)
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

                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

                            var tokenHandler = new JwtSecurityTokenHandler();
                            var token = tokenHandler.ReadJwtToken(jwtToken);


                            var userIdClaim = token.Claims.FirstOrDefault(c => c.Type == "UserID");
                            if (userIdClaim != null)
                            {
                                string userId = userIdClaim.Value;
                                comment.UserID = int.Parse(userId);
                                string json = JsonConvert.SerializeObject(comment);
                                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                                HttpResponseMessage response = await client.PostAsync("api/comments/Create", content);

                                if (response.IsSuccessStatusCode)
                                {

                                    string responseBody = await response.Content.ReadAsStringAsync();
                                    var createdComment = JsonConvert.DeserializeObject<PROJECT_Reddit_wanna_be_.Models.Comments>(responseBody);
                                    int commentId = createdComment.ID;

                                    HttpResponseMessage getCommentResponse = await client.GetAsync($"api/comments/{commentId}");

                                    if (getCommentResponse.IsSuccessStatusCode)
                                    {
                                        string getCommentResponseBody = await getCommentResponse.Content.ReadAsStringAsync();
                                        var retrievedComment = JsonConvert.DeserializeObject<PROJECT_Reddit_wanna_be_.Models.Comments>(getCommentResponseBody);

                                        return RedirectToAction("GetComments", "Main", new { postId = retrievedComment.PostID });
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
            return View(null);
        }
        [HttpGet]
        public async Task<IActionResult> GetComments(int postId)
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
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var token = tokenHandler.ReadJwtToken(jwtToken);
                        var userIdClaim = token.Claims.FirstOrDefault(c => c.Type == "UserID");

                        if (userIdClaim != null) 
                        {
                            string userId = userIdClaim.Value;
                            ViewBag.UserID = int.Parse(userId);
                            HttpResponseMessage response = await client.GetAsync($"api/comments/PostID/{postId}");

                            if (response.IsSuccessStatusCode)
                            {
                                string responseBody = await response.Content.ReadAsStringAsync();
                                var comments = JsonConvert.DeserializeObject<List<PROJECT_Reddit_wanna_be_.Models.Comments>>(responseBody);
                                ViewBag.PostId = postId;
                                return View("GetComments", comments);
                            }
                            else
                            {
                                Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                            }
                        }
                            
                    }
                        
                }
                    
            }
            return View(new List<PROJECT_Reddit_wanna_be_.Models.Comments>());
        }
 
        public async Task<IActionResult> DeleteComment(int commentId, int postId)
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
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                           

                            HttpResponseMessage response = await client.DeleteAsync($"api/comments/DELETE/{commentId}");

                            if (response.IsSuccessStatusCode)
                            {
                                // Deletion was successful
                                return RedirectToAction("GetComments", "Main", new { postId = postId });
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
                        // Handle error
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
                if(!string.IsNullOrEmpty(authToken))
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
                                return RedirectToAction("GetComments", "Main", new { postId = UpdatedComment.PostID });
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

