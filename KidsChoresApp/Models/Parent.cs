using SQLite;


namespace KidsChoresApp.Models
{
    public class Parent
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int UserId { get; set; }

        public bool IsPadlockUnlocked { get; set; }

        [MaxLength(4)]
        public string? Passcode { get; set; }
    }
}
