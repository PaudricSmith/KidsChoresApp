using SQLite;


namespace KidsChoresApp.Models
{
    public class Child
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int UserId { get; set; }

        [NotNull, MaxLength(20)]
        public string Name { get; set; }

        [NotNull, MaxLength(4)]
        public string Passcode { get; set; }

        public string Image { get; set; }
        
        public decimal Money { get; set; }
        public decimal WeeklyEarnings { get; set; }
        public decimal LifetimeEarnings { get; set; }
    }
}