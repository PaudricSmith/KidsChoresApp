using SQLite;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace KidsChoresApp.Models
{
    public class Chore : INotifyPropertyChanged
    {
        private bool _isComplete;


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


        public bool IsComplete
        {
            get => _isComplete;
            set
            {
                if (_isComplete != value)
                {
                    _isComplete = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
