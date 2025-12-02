
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Horta.Domain.Model
{
    [Table("users")]
    public class User
    {
        [Key]
        [Required]
        public int UserId { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public string UserAgent { get; set; }
        [StringLength(45)]
        public string IpAddress { get; set; }
        [Column(TypeName = "timestamp with time zone")]
        public DateTime CreatedAt { get; set; }
        [Column(TypeName = "timestamp with time zone")]
        public DateTime UpdatedAt { get; set; }


        public static User Create(string email, string password, string username, string userAgent, string ipAddress)
        {
            var now = DateTime.UtcNow;
            return new User
            {
                Email = email,
                Password = password,
                Username = username,
                UserAgent = userAgent,
                IpAddress = ipAddress,
                CreatedAt = now,
                UpdatedAt = now
            };
        }

        public static bool HasEmail(User user, string email)
        {
            return user.Email.Equals(email, StringComparison.OrdinalIgnoreCase);
        }

    }

    



}


