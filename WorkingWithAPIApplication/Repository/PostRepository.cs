using Dapper;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.Data.SqlClient;
using WorkingWithAPIApplication.Context;
using WorkingWithAPIApplication.Contracts;
using WorkingWithAPIApplication.Dto.PostDTO;
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
        public async Task<IEnumerable<Post>> GetPostsByTopicId(int TopicID)
        {
            var query = "SELECT P.*, U.Username\r\nFROM Posts P\r\nINNER JOIN Users U ON P.UserID = U.ID\r\nWHERE P.TopicID = @TopicID;";
            //var query = "SELECT * FROM Posts WHERE Posts.TopicID = @TopicID";
            using (var connection = _context.CreateConnection())
            {
                var posts = await connection.QueryAsync<Post>(query, new { TopicID }); 
                return posts.ToList();
            }
        }

        public async Task<Post> GetPost(int postId)
        {
            using (var connection = _context.CreateConnection())
            {
                var query = "SELECT Posts.*, Users.Username \r\nFROM Posts \r\nINNER JOIN Users ON Users.ID = Posts.UserID\r\nWHERE Posts.ID  = @ID;\r\n";
                var post = await connection.QueryFirstOrDefaultAsync<Post>(query, new { ID = postId });

                return post;
            }
        }
        public async Task UpdatePost(int id, PostForUpdate post)
        {
            var query = "UPDATE Posts SET Content = @Content";
            var parameters = new DynamicParameters();
            parameters.Add("Content", post.Content, System.Data.DbType.String);
            parameters.Add("UserId", post.UserID, System.Data.DbType.Int32);
            parameters.Add("TopicId", post.TopicID, System.Data.DbType.Int32);
            parameters.Add("PostedDate", post.PostedDate, System.Data.DbType.DateTime);

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

        public async Task DeletePost(int id)
        {
            var query = "DELETE FROM Posts WHERE Id = @ID";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { id });
            }
        }


        public async Task<int> CreatePost(PostForCreation post)
        {
            var query = @"INSERT INTO Posts(PostID, Content, PostedDate, UserID, TopicID) 
                 VALUES (@PostID, @Content, @PostedDate, @UserID, @TopicID) SELECT CAST(SCOPE_IDENTITY() as int)";

            bool validUser = await ValidateUserExists(post.UserId.ToString());
            bool validTopic = await ValidateTopicExists(post.TopicId.ToString());
            if (!validUser || !validTopic)
            {
                throw new InvalidOperationException("Invalid User or Topic ID.");
            }
            post.PostedDate = DateTime.Now;
            string DatetimeString = post.PostedDate.ToString();
            post.PostId = Guid.NewGuid();
            var parameters = new DynamicParameters();
            parameters.Add("PostID", post.PostId.ToString(), System.Data.DbType.String);
            parameters.Add("Content", post.Content, System.Data.DbType.String);
            parameters.Add("PostedDate", DatetimeString, System.Data.DbType.String);
            parameters.Add("UserID", post.UserId, System.Data.DbType.Int32);
            parameters.Add("TopicID", post.TopicId, System.Data.DbType.Int32);

            using (var connection = _context.CreateConnection())
            {
                var id = await connection.QuerySingleOrDefaultAsync<int>(query, parameters);
                return id;
            }
        }


        private async Task<bool> ValidateUserExists(string userId)
        {
            using (var connection = _context.CreateConnection())
            {
                string query = "SELECT COUNT(*) FROM Users WHERE ID = @ID";
                var parameters = new { ID = userId };
                int count = await connection.ExecuteScalarAsync<int>(query, parameters);
                return count > 0;
            }
        }
        private async Task<bool> ValidateTopicExists(string topicId)
        {
            using (var connection = _context.CreateConnection())
            {
                string query = "SELECT COUNT(*) FROM Topics WHERE ID = @ID";
                var parameters = new { ID = topicId };
                int count = await connection.ExecuteScalarAsync<int>(query, parameters);
                return count > 0;
            }
        }
    }

}