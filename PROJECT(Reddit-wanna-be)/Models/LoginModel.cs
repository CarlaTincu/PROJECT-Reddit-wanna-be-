using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.Serialization;

namespace PROJECT_Reddit_wanna_be_.Models
{
    [DataContract]
    public class LoginModel
    {
        [Required]
        [DataMember]
        public string UserName;
        [Required]
        [DataMember]
        public string Password;
    }
}
