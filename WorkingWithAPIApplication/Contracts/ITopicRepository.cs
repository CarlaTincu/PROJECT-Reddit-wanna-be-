using WorkingWithAPIApplication.Dto.TopicDTO;
using WorkingWithAPIApplication.Dto.UserDTO;
using WorkingWithAPIApplication.Entities;

namespace WorkingWithAPIApplication.Contracts
{
    public interface ITopicRepository
    {
        public Task<IEnumerable<Topic>> GetTopics();
        public Task<Topic> GetTopic(int id);
        public Task<Topic> CreateTopic(TopicForCreation topic);

    }
}
