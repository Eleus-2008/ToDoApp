using System;
using Newtonsoft.Json;

namespace ToDoApp.Model.Models
{
    public class AccessToken
    {
        public Guid Id { get; set; } 
        public string Token { get; set; }
        [JsonProperty("expiration")]
        public DateTime ExpirationTimeUtc { get; set; }
        
        public int UserId { get; set; }
        public User User { get; set; }
    }
}