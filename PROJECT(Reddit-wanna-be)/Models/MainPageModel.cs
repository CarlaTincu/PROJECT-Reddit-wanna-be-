using PROJECT_Reddit_wanna_be_.Project.Data.Entities;

namespace PROJECT_Reddit_wanna_be_.Models
{
    public class MainPageModel
    {
        public int ID { get; set; }
        public string UserFullName { get; set; }
        public List<Project.Data.Entities.Topics> TopicsList { get; set; }
    }
}
