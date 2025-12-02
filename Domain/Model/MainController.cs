
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Horta.Domain.Model
{
    [Table("main_controllers")]
    public class MainController
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [StringLength(45)]
        public string IpAddress { get; set; }
        public bool? LightStatus { get; set; }
        public bool? PumpRelayStatus { get; set; }
        public string DeviceSecreteKey { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        public virtual User User { get; set; }


        public static MainController Create(string ipAddress, int UserId)
        {
            return new MainController
            {
                IpAddress = ipAddress,
                LightStatus = false,
                UserId = UserId
            };
        }



    }

}


