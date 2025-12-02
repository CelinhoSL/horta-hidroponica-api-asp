using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Horta.Domain.Model
{
    [Table("user_logs")]
    public class UserLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        [StringLength(45)]
        public string IpAddress { get; set; }

        [Column(TypeName = "timestamp with time zone")]
        public DateTime CreatedAt { get; set; }

        [Column(TypeName = "timestamp with time zone")]
        public DateTime UpdatedAt { get; set; }

        public string UserAgent { get; set; }

        public virtual User User { get; set; }


        public static UserLog Create(string IpAddress, int UserId, string UserAgent)
        {
            var now = DateTime.UtcNow;
            return new UserLog
            {
                UserId = UserId,
                IpAddress = IpAddress,
                CreatedAt = now,
                UpdatedAt = now,
                UserAgent = UserAgent,
            };
        }

    }
}