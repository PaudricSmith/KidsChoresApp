using SQLite;


namespace KidsChoresApp.Models
{
    public class Chore
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int ChildId { get; set; }

        [NotNull, MaxLength(20)]
        public string Name { get; set; }

        public string Description { get; set; }
        public string Image { get; set; }
        public DateTime Deadline { get; set; }

        public decimal Worth { get; set; }
        public int Priority { get; set; }
        public bool IsComplete { get; set; }
    }
}
