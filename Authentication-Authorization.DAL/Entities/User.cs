﻿using System;

namespace Authentication_Authorization.DAL.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string SecretId { get; set; }
        public string Password { get; set; }
        public int Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
