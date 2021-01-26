using System;

namespace ToDoApp.Model.Models
{
    public class User
    {
        public int Id { get; set; }

        private string _username;
        public string Username
        {
            get => _username;
            set => _username = value?.ToLower();
        }
        public DateTime LastLogonTime { get; set; }

        public AccessToken Token { get; set; }
    }
}