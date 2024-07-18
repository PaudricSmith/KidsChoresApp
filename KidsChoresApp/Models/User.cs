using SQLite;


namespace KidsChoresApp.Models
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Unique, NotNull]
        public string Email { get; set; }

        [NotNull]
        public string PasswordHash { get; set; }

        [NotNull, MaxLength(3)]
        public string PreferredCurrency { get; set; }
        public bool IsSetupCompleted { get; set; }
    }
}