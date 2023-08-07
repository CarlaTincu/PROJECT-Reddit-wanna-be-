using WorkingWithAPIApplication.Dto.CommentDTO;
using WorkingWithAPIApplication.Dto.PostDTO;
using WorkingWithAPIApplication.Entities;

namespace WorkingWithAPIApplication.Contracts
{
    public interface ICommentRepository
    {
        public Task<IEnumerable<Comment>> GetComments(int postId);
        public Task<Comment> GetComment(int id);
        public Task<int> CreateComment(CommentForCreation comment);
        public Task UpdateComment(int id, CommentForUpdate comment);
        public Task DeleteComment(int id);
    }
}
