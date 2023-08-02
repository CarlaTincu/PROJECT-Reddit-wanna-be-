using WorkingWithAPIApplication.Dto.PostDTO;
using WorkingWithAPIApplication.Dto.UserDTO;
using WorkingWithAPIApplication.Entities;

namespace WorkingWithAPIApplication.Contracts
{
    public interface IPostRepository
    {
        public Task<IEnumerable<Post>> GetPosts();
        public Task<Post> GetPost(int id);
        public Task<int> CreatePost(PostForCreation post);
        public Task UpdatePost(int id, PostForUpdate post);
        public Task DeletePost(int id);
    }
}
