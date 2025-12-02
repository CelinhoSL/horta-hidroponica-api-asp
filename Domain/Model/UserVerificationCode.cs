using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Horta.Domain.Model
{
    [Table("user_verification_codes")]
    public class UserVerificationCode
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(255)] 
        public string Email { get; set; }

        [Required]
        [StringLength(10)] 

        [Range(0, 999999)]
        public int Code { get; set; }

        [Column(TypeName = "timestamp with time zone")]
        public DateTime? ExpiresAt { get; set; }

        [Column(TypeName = "timestamp with time zone")]
        public DateTime? CreatedAt { get; set; }

        [Column(TypeName = "timestamp with time zone")]
        public DateTime? UpdatedAt { get; set; }

        public bool IsUsed { get; set; } = false;
        


        public static UserVerificationCode Create(string email, int code)
        {
            var now = DateTime.UtcNow;
            return new UserVerificationCode
            {
                Email = email,
                Code = code,
                CreatedAt = now,
                UpdatedAt = now,
                ExpiresAt = now.AddMinutes(3),
                IsUsed = false
            };


        }

        public void MarkAsUsed()
        {
            IsUsed = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}


