using System.ComponentModel.DataAnnotations;


namespace KidsChoresApp.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [MaxLength(3)]
        public string? PreferredCurrency { get; set; }

        public bool IsSetupCompleted { get; set; }


        // Navigation properties
        public Parent Parent { get; set; }
        public ICollection<Child> Children { get; set; } = new List<Child>();
    }
}