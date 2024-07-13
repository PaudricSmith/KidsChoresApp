using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace KidsChoresApp.Models
{
    public class Parent
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Passcode { get; set; }

        [Required]
        public int UserId { get; set; } 

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
    }
}
