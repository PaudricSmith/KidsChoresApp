using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace KidsChoresApp.Models
{
    public class Parent
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(4)]
        public string Passcode { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))] // Navigation property
        public User User { get; set; }
    }
}
