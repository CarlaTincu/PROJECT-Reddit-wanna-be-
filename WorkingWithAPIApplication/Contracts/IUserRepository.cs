using WorkingWithAPIApplication.Dto.UserDTO;
using WorkingWithAPIApplication.Entities;

namespace WorkingWithAPIApplication.Contracts
{
    public interface IUserRepository
    {
        public Task<IEnumerable<User>> GetUsers();
        public Task<User> GetUser(int id);
        public Task<int> CreateUser(UserForCreation user);
        public Task UpdateUser(int id, UserForUpdate user);
        public Task DeleteUser(int id); 
        public Task<User> GetUserForLogin(string Username, string Password);
        public User GetUser(string userName);
        public Task<User> GetUserName(string userName);


    }
}
