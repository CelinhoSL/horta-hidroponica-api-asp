using Horta.Domain.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Horta_Api.Domain.Model
{
    [Table("cameras")]
    public class Camera
    {
            [Key]
            public int Id { get; set; }

            public int MainControllerId { get; set; }

            [ForeignKey(nameof(MainControllerId))]
            public MainController MainController { get; set; }
       

        public static Camera Create(int mainControllerId)
        {
            return new Camera { MainControllerId = mainControllerId };
        }


    }
}
