using SQLite;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


namespace KidsChoresApp.Models
{
    public class Chore : INotifyPropertyChanged
    {
        private bool _isComplete;
        private bool _isDetailsVisible;


        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int ChildId { get; set; }

        [NotNull, MaxLength(20)]
        public string Name { get; set; }

        public string? Description { get; set; }
        public string? Image { get; set; }
        public string AssignedTo { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
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
        public bool IsDetailsVisible
        {
            get => _isDetailsVisible;
            set
            {
                if (_isDetailsVisible != value)
                {
                    _isDetailsVisible = value;
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
