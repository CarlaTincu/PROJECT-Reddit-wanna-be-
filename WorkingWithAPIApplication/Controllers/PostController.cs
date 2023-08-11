using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetPosts()
        {
            var posts = await postRepository.GetPosts();
            return Ok(posts);
        }

        [HttpGet("{postId}", Name = "PostById")]
        public async Task<IActionResult> GetPost(int postId)
        {
            var post = await postRepository.GetPost(postId);
            if (post == null)
                return NotFound();
            return Ok(post);
        }
        [HttpPost("Create")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CreatePost([FromBody] PostForCreation PostID)
        {
            try
            {
                var createPost = await postRepository.CreatePost(PostID);
                return Ok(new { Id = createPost });
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

            
        }

        [HttpPut("EDIT/{postId}")]
        public async Task<IActionResult> UpdatePost(int postId, [FromBody] PostForUpdate PostU)
        {
            var Post = await postRepository.GetPost(postId);
            if (Post == null)
                return NotFound();
            await postRepository.UpdatePost(postId, PostU);
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


        [HttpGet("PostsByTopic/{TopicID}")] // Use the correct parameter name
        public async Task<IActionResult> GetPostsByTopicId(int TopicID)
        {
            var posts = await postRepository.GetPostsByTopicId(TopicID);
            return Ok(posts);
        }

    }
}
