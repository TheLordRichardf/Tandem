using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace TandemAPI.Models
{
    public class Person
    {
        [JsonProperty("id")]
        public string UserId { get; set; }
        
        [Required()]
        public string FirstName { get; set; }
       
        [Required()]
        public string MiddleName { get; set; }
        
        [Required()]
        public string LastName { get; set; }
       
        [Required()]
        public string PhoneNumber { get; set; }
        
        [Required()]
        [JsonProperty("emailAddress")]
        public string EmailAddress { get; set; }

        public string GetName()
        {
            return $"{this.FirstName} {this.MiddleName} {this.LastName}";
        }
    }
}
