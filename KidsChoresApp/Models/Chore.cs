using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace KidsChoresApp.Models
{
    public class Chore
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        public string? Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(200)]
        public string? Image { get; set; }

        [MaxLength(50)]
        public string? AssignedTo { get; set; }

        [Required]
        public DateTime Deadline { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue)]
        public decimal Worth { get; set; } = 0m;

        [Range(1, 10)]
        public int Priority { get; set; } = 1;

        public bool IsComplete { get; set; } = false;

        [Required]
        public int ChildId { get; set; }

        [ForeignKey(nameof(ChildId))] // Navigation property
        public Child Child { get; set; }
    }
}
