﻿using Microsoft.AspNetCore.Mvc;
using WorkingWithAPIApplication.Contracts;
using WorkingWithAPIApplication.Dto.PostDTO;
using WorkingWithAPIApplication.Dto.UserDTO;
using WorkingWithAPIApplication.Repository;

namespace WorkingWithAPIApplication.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class PostController : ControllerBase
    {
        
        private readonly IPostRepository postRepository;
        public PostController(IPostRepository postRepository)
        {
            this.postRepository = postRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetPosts()
        {
            var posts = await postRepository.GetPosts();
            return Ok(posts);
        }
        [HttpGet("{Id}", Name = "PostById")]
        public async Task<IActionResult> GetPost(int id)
        {
            var post = await postRepository.GetPost(id);
            if (post == null)
                return NotFound();
            return Ok(post);
        }
        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] PostForCreation PostID)
        {
            var createPost = await postRepository.CreatePost(PostID);
            return Ok(new { Id = createPost});
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(int id, [FromBody] PostForUpdate PostU)
        {
            var Post = await postRepository.GetPost(id);
            if (Post == null)
                return NotFound();
            await postRepository.UpdatePost(id, PostU);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var User = await postRepository.GetPost(id);
            if (User == null)
                return NotFound();
            await postRepository.DeletePost(id);
            return NoContent();
        }

    }
}