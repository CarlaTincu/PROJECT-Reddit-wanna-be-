﻿using Azure;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PROJECT_Reddit_wanna_be_.Models;
using PROJECT_Reddit_wanna_be_.Project.Data.Entities;
using RestSharp;
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

        private readonly HttpClient _client;
        private readonly MethodController method;

        public MainController(HttpClient client, MethodController method)
        {
            _client = client;
            this.method = method;
        }

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
                string jwtToken = await method.GetJwtTokenAsync(HttpContext);
                if (!string.IsNullOrEmpty(jwtToken))
                {
                    method.ConfigureHttpClient(_client, jwtToken);
                    string json = JsonConvert.SerializeObject(topic);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await _client.PostAsync("api/topics/Create", content);
                    var createdTopic = await method.GetApiResponseAsync<PROJECT_Reddit_wanna_be_.Project.Data.Entities.Topics>(response);
                    if (createdTopic != null)
                    {
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
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            return View(null);
        }
        //COMMENTS
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

