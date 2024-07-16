using SQLite;


namespace KidsChoresApp.Models
{
    public class Child
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int UserId { get; set; }

        [NotNull]
        public string Name { get; set; }
        public string Image { get; set; }
        public string Passcode { get; set; }
        public decimal Money { get; set; }
        public decimal WeeklyEarnings { get; set; }
        public decimal LifetimeEarnings { get; set; }
    }
}
