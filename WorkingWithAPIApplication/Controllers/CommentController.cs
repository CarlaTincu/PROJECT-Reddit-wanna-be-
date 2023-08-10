using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WorkingWithAPIApplication.Contracts;
using WorkingWithAPIApplication.Dto.CommentDTO;
using WorkingWithAPIApplication.Dto.PostDTO;
using WorkingWithAPIApplication.Repository;

namespace WorkingWithAPIApplication.Controllers
{
    [Route("api/comments")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository commentRepository;
        public CommentController(ICommentRepository commentRepository)
        {
            this.commentRepository = commentRepository;
        }
        [HttpPost("Create")]
        [Authorize]
        public async Task<IActionResult> CreateComment([FromBody] CommentForCreation CommentID)
        {
            var createComment = await commentRepository.CreateComment(CommentID);
            return Ok(new { ID = createComment });
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(int id, [FromBody] CommentForUpdate commentUpdated)
        {
            var theComment = await commentRepository.GetComment(id);
            if (theComment == null)
                return NotFound();
            await commentRepository.UpdateComment(id, commentUpdated);
            return NoContent();
        }

        [HttpDelete("DELETE/{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var User = await commentRepository.GetComment(id);
            if (User == null)
                return NotFound();
            await commentRepository.DeleteComment(id);
            return NoContent();
        }
        [HttpGet("PostID/{postId}")]
        public async Task<IActionResult> GetComments(int PostId)
        {
            var comments = await commentRepository.GetComments(PostId);
            return Ok(comments);
        }
        [HttpGet("{Id}", Name = "CommentById")]
        public async Task<IActionResult> GetPost(int id)
        {
            var comment = await commentRepository.GetComment(id);
            if (comment == null)
                return NotFound();
            return Ok(comment);
        }
    }
}
