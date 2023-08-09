using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorkingWithAPIApplication.Contracts;
using WorkingWithAPIApplication.Dto.TopicDTO;
using WorkingWithAPIApplication.Dto.UserDTO;
using WorkingWithAPIApplication.Repository;

namespace WorkingWithAPIApplication.Controllers
{
    [Route("api/topics")]
    [ApiController]
    public class TopicController : ControllerBase
    {
        private readonly ITopicRepository topicRepository;
        public TopicController(ITopicRepository topicRepository)
        {
            this.topicRepository = topicRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetTopics()
        {
            var topics = await topicRepository.GetTopics();
            return Ok(topics);
        }
        [HttpGet("{id}", Name = "TopicById")]
        public async Task<IActionResult> GetTopic(int id)
        {
            var topic = await topicRepository.GetTopic(id);
            if (topic == null)
                return NotFound();
            return Ok(topic);
        }

       
        [HttpPost("Create")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CreateTopic([FromBody] TopicForCreation TopicID)
        {
            var createdTopic = await topicRepository.CreateTopic(TopicID);
            return Ok(new { Id = createdTopic });
        }
    }
}
