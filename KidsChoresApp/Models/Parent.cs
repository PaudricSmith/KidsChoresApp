﻿using SQLite;


namespace KidsChoresApp.Models
{
    public class Parent
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int UserId { get; set; }

        [NotNull, MaxLength(4)]
        public string Passcode { get; set; }
    }
}
