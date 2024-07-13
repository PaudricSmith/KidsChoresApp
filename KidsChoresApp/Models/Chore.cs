using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace KidsChoresApp.Models
{
    public class Chore
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        public string? Image { get; set; }

        public string? AssignedTo { get; set; }

        public DateTime Deadline { get; set; }

        public decimal Worth { get; set; }

        public int Priority { get; set; }

        public bool IsComplete { get; set; }
        
        [Required]
        public int ChildId { get; set; }

        [ForeignKey(nameof(ChildId))]
        public Child Child { get; set; }
    }
}
