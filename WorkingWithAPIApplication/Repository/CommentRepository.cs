using Dapper;
using WorkingWithAPIApplication.Context;
using WorkingWithAPIApplication.Contracts;
using WorkingWithAPIApplication.Dto.CommentDTO;
using WorkingWithAPIApplication.Entities;

namespace WorkingWithAPIApplication.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly DapperContext _context;
        public CommentRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<int> CreateComment(CommentForCreation comment)
        {
            var query = @"INSERT INTO Comments(CommentID, UserID, PostID, Content, PostedDate)
                 VALUES (@CommentID, @UserID, @PostID, @Content, @PostedDate) SELECT CAST(SCOPE_IDENTITY() as int)";
            comment.PostedDate = DateTime.Now;
            string DatetimeString = comment.PostedDate.ToString();
            comment.CommmentID = Guid.NewGuid();
            var parameters = new DynamicParameters();

            parameters.Add("CommentId", comment.CommmentID.ToString(), System.Data.DbType.String);
            parameters.Add("UserID", comment.UserID, System.Data.DbType.Int32);
            parameters.Add("PostID", comment.PostID, System.Data.DbType.Int32);
            parameters.Add("Content", comment.Content, System.Data.DbType.String);
            parameters.Add("PostedDate", DatetimeString, System.Data.DbType.String);

            using (var connection = _context.CreateConnection())
            {
                var id = await connection.QuerySingleOrDefaultAsync<int>(query, parameters);
                return id;
            }
        }

        public async Task DeleteComment(int id)
        {
            var query = "DELETE FROM Comments WHERE Id = @ID";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { id });
            }
        }

        public async Task<Comment> GetComment(int id)
        {
            using (var connection = _context.CreateConnection())
            {
                var query = "SELECT Comments.*, Users.Username \r\nFROM Comments \r\nINNER JOIN Users ON Users.ID = Comments.UserID\r\nWHERE Comments.ID  = @ID\r\n;\r\n\r\n";
                var comment = await connection.QueryFirstOrDefaultAsync<Comment>(query, new { ID = id });

                return comment;
            }
        }

        public async Task<IEnumerable<Comment>> GetComments(int postId)
        {
            var query = "SELECT Comments.*, Users.Username " +
                "FROM Comments " +
                "INNER JOIN Users ON Users.ID = Comments.UserID " +
                "WHERE Comments.PostID = @PostID";

            using (var connection = _context.CreateConnection())
            {
                var comments = await connection.QueryAsync<Comment>(query, new { PostID = postId });
                return comments.ToList();
            }
        }

        public async Task UpdateComment(int id, CommentForUpdate comment)
        {
            var query = "UPDATE Comments SET Content = @Content";
            var parameters = new DynamicParameters();
            comment.PostedDate = DateTime.Now;
            string DatetimeString = comment.PostedDate.ToString();
            comment.CommmentID = Guid.NewGuid();
            parameters.Add("CommentId", comment.CommmentID.ToString(), System.Data.DbType.String);
            parameters.Add("UserID", comment.UserID, System.Data.DbType.Int32);
            parameters.Add("PostID", comment.PostID, System.Data.DbType.Int32);
            parameters.Add("Content", comment.Content, System.Data.DbType.String);
            parameters.Add("PostedDate", DatetimeString, System.Data.DbType.String);

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }
    }
}
