using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace KidsChoresApp.Models
{
    public class Child
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Image { get; set; }

        public string? Passcode { get; set; }

        public decimal Money { get; set; }

        public decimal WeeklyEarnings { get; set; }

        public decimal LifetimeEarnings { get; set; }

        [Required]
        public int UserId { get; set; } 
        
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
    }
}
