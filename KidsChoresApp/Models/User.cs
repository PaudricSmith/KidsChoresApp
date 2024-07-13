using System.ComponentModel.DataAnnotations;


namespace KidsChoresApp.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public string? PreferredCurrency { get; set; }

        public bool IsSetupCompleted { get; set; }
    }
}