using Dapper;
using WorkingWithAPIApplication.Context;
using WorkingWithAPIApplication.Contracts;
using WorkingWithAPIApplication.Dto.UserDTO;
using WorkingWithAPIApplication.Entities;

namespace WorkingWithAPIApplication.Repository
{
    public class PostRepository : IPostRepository
    {
        private readonly DapperContext _context;
        public PostRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Post>> GetPosts()
        {
            var query = "SELECT * FROM Posts";
            using (var connection = _context.CreateConnection())
            {
                var posts = await connection.QueryAsync<Post>(query);
                return posts.ToList();
            }
        }

        public async Task<Post> GetPost(int postId)
        {
            var query = @"
                SELECT 
                    p.PostId,
                    p.Title,
                    p.Content,
                    p.CreatedAt,
                    p.UserId,
                    u.UserName,
                    p.TopicId,
                    t.TopicName
                FROM post p
                INNER JOIN [user] u ON p.UserId = u.UserId
                INNER JOIN topic t ON p.TopicId = t.TopicId
                WHERE p.PostId = @PostId;
            ";

            using (var connection = _context.CreateConnection())
            {
                var post = (await connection.QueryAsync<Post, User, Topic, Post>(
                    query,
                    (post, user, topic) =>
                    {
                        post.User = user;
                        post.Topic = topic;
                        return post;
                    },
                    new { PostId = postId },
                    splitOn: "UserId, TopicId"
                )).FirstOrDefault();

                return post;
            }
        
        }

    }
}
