﻿using Dapper;
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
                 VALUES (@CommentID, @UserID, @PostID, @Content, @PostedDate)";
            comment.PostedDate = DateTime.Now;
            string DatetimeString = comment.PostedDate.ToString();
            comment.CommmentID = Guid.NewGuid();
            Console.WriteLine(comment.CommmentID);
            var parameters = new DynamicParameters();

            parameters.Add("CommentId", comment.CommmentID.ToString(), System.Data.DbType.String);
            parameters.Add("UserID", comment.UserID, System.Data.DbType.Int32);
            parameters.Add("PostID", comment.PostID, System.Data.DbType.Int32);
            parameters.Add("Content", comment.Content, System.Data.DbType.String);
            parameters.Add("PostedDate", DatetimeString, System.Data.DbType.String);

            using (var connection = _context.CreateConnection())
            {
                var id = await connection.QuerySingleOrDefaultAsync<int>(query, parameters);
                var CreatedComment = new Comment
                {
                    ID = id,
                    CommmentID = comment.CommmentID,
                    UserID = comment.UserID,
                    Content = comment.Content,
                    PostID = comment.PostID,
                    PostedDate = comment.PostedDate

                };
                return CreatedComment.ID;
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
                var query = "SELECT * FROM Comments WHERE ID = @ID";
                var comment = await connection.QueryFirstOrDefaultAsync<Comment>(query, new { ID = id });

                return comment;
            }
        }

        public async Task<IEnumerable<Comment>> GetComments()
        {
            var query = "SELECT * FROM Comments";
            using (var connection = _context.CreateConnection())
            {
                var comments = await connection.QueryAsync<Comment>(query);
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
