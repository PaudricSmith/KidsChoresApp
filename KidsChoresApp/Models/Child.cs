using SQLite;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace KidsChoresApp.Models
{
    public class Child : INotifyPropertyChanged
    {
        private string _name;
        private string _passcode;
        private string _image;
        private decimal _money;
        private decimal _weeklyEarnings;
        private decimal _lifetimeEarnings;

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int UserId { get; set; }


        [NotNull, MaxLength(15)]
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        [NotNull, MaxLength(4)]
        public string Passcode
        {
            get => _passcode;
            set
            {
                if (_passcode != value)
                {
                    _passcode = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Image
        {
            get => _image;
            set
            {
                if (_image != value)
                {
                    _image = value;
                    OnPropertyChanged();
                }
            }
        }

        public decimal Money
        {
            get => _money;
            set
            {
                if (_money != value)
                {
                    _money = value;
                    OnPropertyChanged();
                }
            }
        }

        public decimal WeeklyEarnings
        {
            get => _weeklyEarnings;
            set
            {
                if (_weeklyEarnings != value)
                {
                    _weeklyEarnings = value;
                    OnPropertyChanged();
                }
            }
        }

        public decimal LifetimeEarnings
        {
            get => _lifetimeEarnings;
            set
            {
                if (_lifetimeEarnings != value)
                {
                    _lifetimeEarnings = value;
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
