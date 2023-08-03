using Azure.Identity;
using Dapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using WorkingWithAPIApplication.Context;
using WorkingWithAPIApplication.Contracts;
using WorkingWithAPIApplication.Dto.UserDTO;
using WorkingWithAPIApplication.Entities;

namespace WorkingWithAPIApplication.Repository
{

    public class UserRepository : IUserRepository
    {
        private readonly DapperContext _context;
        public UserRepository(DapperContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<User>> GetUsers()
        {
            var query = "SELECT * FROM Users";
            using(var connection = _context.CreateConnection())
            {         
                var users = await connection.QueryAsync<User>(query);
                return (IEnumerable<User>)users.ToList();
            }
        }
        public async Task<User> GetUser(int id)
        {
            var query = "SELECT * FROM Users WHERE Id = @ID";
            using (var connection = _context.CreateConnection())
            {
                var user = await connection.QuerySingleOrDefaultAsync<User>(query, new { id });
                return user;
            }
        }

        public async Task<int> CreateUser(UserForCreation user)
        {
            var query = "INSERT INTO Users(UserId,Username,Email,Password) VALUES (@UserId,@Username,@Email,@Password)";
            var parameters = new DynamicParameters();
            user.UserId = Guid.NewGuid();
            string UserIdAsString = user.UserId.ToString();
            parameters.Add("UserId", UserIdAsString , System.Data.DbType.String);
            parameters.Add("Username", user.Username, System.Data.DbType.String);
            parameters.Add("Email", user.Email, System.Data.DbType.String);
            parameters.Add("Password", user.Password, System.Data.DbType.String);
            using (var connection = _context.CreateConnection())
            {
                var id = await connection.QuerySingleOrDefaultAsync<int>(query, parameters);
                var CreatedUser = new User
                {
                    ID = id,
                    UserID = (Guid)user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    Password = user.Password,
                };
                return CreatedUser.ID;
            }
        }

        public async Task UpdateUser(int id, UserForUpdate user)
        {
            var query = "UPDATE Users SET Username = @Username, Email = @Email,Password = @Password where Id = @ID";
            var parameters = new DynamicParameters();
            parameters.Add("Id", user.ID, System.Data.DbType.Int32);
            parameters.Add("Username", user.Username, System.Data.DbType.String);
            parameters.Add("Email", user.Email, System.Data.DbType.String);
            parameters.Add("Password", user.Password, System.Data.DbType.String);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

        public async Task DeleteUser(int id)
        {
            var query = "DELETE FROM Users WHERE Id = @ID";
            using (var connection = _context.CreateConnection())
            {
                 await connection.ExecuteAsync(query, new { id });
            }
        }

        public async Task<User> GetUserForLogin(string Username, string Password)
        {
            var query = "SELECT Username, Password FROM Users WHERE Username = @Username AND Password = @Password;";
            using (var connection = _context.CreateConnection())
            {
                var user = await connection.QuerySingleOrDefaultAsync<User>(query, new { Username = Username, Password = Password });
                return user;
            }
        }
    }
}
