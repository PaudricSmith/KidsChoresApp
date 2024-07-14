using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace KidsChoresApp.Models
{
    public class Child
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Name { get; set; }

        public string? Image { get; set; }

        [MaxLength(4)]
        public string? Passcode { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue)]
        public decimal Money { get; set; } = 0m;

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue)]
        public decimal WeeklyEarnings { get; set; } = 0m;

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue)]
        public decimal LifetimeEarnings { get; set; } = 0m;

        [Required]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))] // Navigation property
        public User User { get; set; }


        // Navigation property
        public ICollection<Chore> Chores { get; set; } = new List<Chore>();
    }
}
