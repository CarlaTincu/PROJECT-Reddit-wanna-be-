using Dapper;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Mvc;
using WorkingWithAPIApplication.Context;
using WorkingWithAPIApplication.Contracts;
using WorkingWithAPIApplication.Dto.TopicDTO;
using WorkingWithAPIApplication.Entities;

namespace WorkingWithAPIApplication.Repository
{
    public class TopicRepository : ITopicRepository
    {
        private readonly DapperContext _context;
        public TopicRepository(DapperContext context)
        {
            _context = context;
        }

        [HttpPost("Create")]
        public async Task<int> CreateTopic(TopicForCreation topic)
        {
            var query = "INSERT INTO Topics(TopicID,TopicName) VALUES (@TopicID,@TopicName)";
            var parameters = new DynamicParameters();
            topic.TopicID = Guid.NewGuid();
            string TopicIdAsString = topic.TopicID.ToString();
            parameters.Add("TopicId", TopicIdAsString, System.Data.DbType.String);
            parameters.Add("TopicName", topic.TopicName, System.Data.DbType.String);
            using (var connection = _context.CreateConnection())
            {
                var id = await connection.QuerySingleOrDefaultAsync<int>(query, parameters);
                var CreateTopic = new Topic
                {
                    ID = id,
                    TopicID = (Guid)topic.TopicID,
                    TopicName = topic.TopicName,
                };
                return CreateTopic.ID;
            }
        }

        public async Task<Topic> GetTopic(int id)
        {
            var query = "SELECT * FROM Topics WHERE Id = @ID";
            using (var connection = _context.CreateConnection())
            {
                var topic = await connection.QuerySingleOrDefaultAsync<Topic>(query, new { id });
                return topic;
            }
        }

        public async Task<IEnumerable<Topic>> GetTopics()
        {
            var query = "SELECT * FROM Topics";
            using (var connection = _context.CreateConnection())
            {
                var topics = await connection.QueryAsync<Topic>(query);
                return (IEnumerable<Topic>)topics.ToList();
            }
        }
    }
}
