using Horta.Domain.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Horta_Api.Domain.Model
{
    [Table("user_reset_password_codes")]
    public class UserResetPasswordCode
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



        public static UserResetPasswordCode Create(string email, int code)
        {
            var now = DateTime.UtcNow;
            return new UserResetPasswordCode
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
