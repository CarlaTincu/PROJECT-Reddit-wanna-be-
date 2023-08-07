using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace PROJECT_Reddit_wanna_be_.Project.Data.Entities
{
    
    public class Topics
    {
        public int ID { get; set; }
        public Guid TopicID { get; set; }

        [Required]
        public string TopicName { get; set; }
    }
}
