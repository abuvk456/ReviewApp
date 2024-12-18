using System;
namespace ReviewApp.Model
{
    public class User
    {
        public int ID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public int UpvoteCount { get; set; } = 1;
        public int DownvoteCount { get; set; } = 0;
        public string Country { get; set; }
        public string Language { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime LastLoginDate { get; set; }
    }
}

