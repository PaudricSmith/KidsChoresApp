using SQLite;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace KidsChoresApp.Models
{
    public class Parent : INotifyPropertyChanged
    {
        private bool _isParentLockEnabled;


        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int UserId { get; set; }

        [MaxLength(4)]
        public string? Passcode { get; set; }


        public bool IsParentLockEnabled
        {
            get => _isParentLockEnabled;
            set
            {
                if (_isParentLockEnabled != value)
                {
                    _isParentLockEnabled = value;
                    OnPropertyChanged();
                }
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
