using KidsChoresApp.Models;
using KidsChoresApp.Pages.ChildPages;
using KidsChoresApp.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace KidsChoresApp.Pages.ChorePages
{
    [QueryProperty(nameof(ChildId), "childId")]
    public partial class ChoresPage : ContentPage
    {
        private readonly UserService _userService;
        private readonly ChildService _childService;
        private readonly ChoreService _choreService;

        private Frame? _previouslySelectedFrame;
        private DateTime _selectedDate;
        private User? _currentUser;
        private Child? _child;
        private WeekDay _currentDay;
        private string? _choresForText;
        private int _childId;
        private bool _loading = true;

        public ObservableCollection<Chore> Chores { get; set; } = new ObservableCollection<Chore>();
        public ObservableCollection<Chore> FilteredChores { get; set; } = new ObservableCollection<Chore>();
        public ObservableCollection<WeekDay> WeekDays { get; set; } = new ObservableCollection<WeekDay>();

        public bool Loading
        {
            get => _loading;
            set
            {
                _loading = value;
                OnPropertyChanged();
            }
        }
        public User? CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged();
            }
        }
        public Child? Child
        {
            get => _child;
            set
            {
                _child = value;
                OnPropertyChanged();
            }
        }
        public int ChildId
        {
            get => _childId;
            set
            {
                _childId = value;
            }
        }
        public string? ChoresForText
        {
            get => _choresForText;
            set
            {
                _choresForText = value;
                OnPropertyChanged();
            }
        }
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (_selectedDate != value)
                {
                    _selectedDate = value;
                    OnPropertyChanged(nameof(SelectedDate));  
                    UpdateSelectedDay();
                }
            }
        }
        public WeekDay CurrentDay
        {
            get => _currentDay;
            set
            {
                if (_currentDay != value)
                {
                    _currentDay = value;
                    SelectedDate = _currentDay.Date; 
                }
            }
        }


        public ChoresPage(UserService userService, ChildService childService, ChoreService choreService)
        {
            InitializeComponent();

            _userService = userService;
            _childService = childService;
            _choreService = choreService;

            // Set the default date to today
            SelectedDate = DateTime.Today;

            BindingContext = this;

            Loaded += ChoresPage_Loaded;
        }


        private async void ChoresPage_Loaded(object? sender, EventArgs e)
        {
            await Task.Delay(300);

            await LoadData();
            await LoadChores();

            GenerateWeekDates();

            UpdateSelectedDay();

            ScrollToSelectedDay();
        }

        private async Task LoadData()
        {
            if (ChildId != 0)
            {
                Child = await _childService.GetChildByIdAsync(ChildId);
                CurrentUser = await _userService.GetUserByIdAsync(Child.UserId);
            }
        }

        private async Task LoadChores()
        {
            var chores = await _choreService.GetChoresByChildIdAsync(ChildId);
            if (chores.Count == 0)
            {
                Loading = false;
                return;
            }

            Dispatcher.Dispatch(() =>
            {
                Chores.Clear();
                foreach (var chore in chores)
                {
                    Chores.Add(chore);
                }

                FilterChoresByDate();
                UpdateChoresForText();

                Loading = false;
            });
        }

        private void GenerateWeekDates()
        {
            var today = DateTime.Today;
            int dayValue = today.DayOfWeek == 0 ? 6 : (int)today.DayOfWeek - 1;
            var startOfWeek = today.AddDays(-dayValue);

            WeekDays.Clear();
            for (int i = 0; i < 7; i++)
            {
                var date = startOfWeek.AddDays(i);
                var weekDay = new WeekDay
                {
                    Date = date,
                    IsSelected = date.Date == SelectedDate.Date  
                };

                WeekDays.Add(weekDay);

                if (weekDay.Date == today)
                    CurrentDay = weekDay;
            }
        }

        private void UpdateSelectedDay()
        {
            foreach (var weekDay in WeekDays)
            {
                if (weekDay.IsSelected != (weekDay.Date.Date == SelectedDate.Date))
                {
                    weekDay.IsSelected = weekDay.Date.Date == SelectedDate.Date;
                }
            }
        }

        private void ScrollToSelectedDay()
        {
            if (CurrentDay != null)
            {
                var itemIndex = WeekDays.IndexOf(CurrentDay);
                if (itemIndex != -1)
                {
                    WeekDaysCollectionView.ScrollTo(itemIndex, position: ScrollToPosition.Center, animate: true);
                }
            }
        }

        private void FilterChoresByDate()
        {
            FilteredChores.Clear();

            var dayOfWeek = SelectedDate.DayOfWeek;
            foreach (var chore in Chores)
            {
                if (chore.DayOfWeek == dayOfWeek)
                    FilteredChores.Add(chore);
            }
        }

        private void UpdateChoresForText()
        {
            var today = DateTime.Today;
            if (SelectedDate.Date == today)
            {
                ChoresForText = "Chores for Today";
            }
            else if (SelectedDate.Date == today.AddDays(1))
            {
                ChoresForText = "Chores for Tomorrow";
            }
            else if (SelectedDate.Date == today.AddDays(-1))
            {
                ChoresForText = "Chores for Yesterday";
            }
            else
            {
                ChoresForText = $"Chores for {SelectedDate:dddd}";
            }
        }

        private void OnDaySelectionTapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is WeekDay selectedDay)
            {
                // Deselect all days first
                foreach (var day in WeekDays)
                {
                    day.IsSelected = false;
                }

                // Select the tapped day
                selectedDay.IsSelected = true;

                // Set the current day
                CurrentDay = selectedDay;

                // Scroll to the selected day
                var itemIndex = WeekDays.IndexOf(selectedDay);
                WeekDaysCollectionView.ScrollTo(itemIndex, position: ScrollToPosition.Center, animate: true);

                // Filter chores by the selected day
                FilterChoresByDate();
                UpdateChoresForText();
            }
        }

        private async void OnChoreCompleteButtonTapped(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Chore chore)
            {
                chore.IsComplete = !chore.IsComplete;

                if (chore.IsComplete)
                {
                    Child.LifetimeEarnings += chore.Worth;
                    Child.WeeklyEarnings += chore.Worth;
                }
                else
                {
                    Child.LifetimeEarnings -= chore.Worth;
                    Child.WeeklyEarnings -= chore.Worth;
                }

                await _choreService.SaveChoreAsync(chore);
                await _childService.SaveChildAsync(Child);
            }
        }

        private void OnExpandButtonTapped(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Chore chore)
            {
                chore.IsDetailsVisible = !chore.IsDetailsVisible;
            }
        }

        private void OnBackButtonTapped(object sender, EventArgs e)
        {
            Shell.Current.GoToAsync($"///{nameof(ChildPage)}?childId={ChildId}");
        }

        private async void OnDeleteChoreButtonTapped(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Chore chore)
            {
                bool isConfirmed = await DisplayAlert("Confirm Delete", "Are you sure you want to delete this chore?", "Yes", "No");
                if (isConfirmed)
                {
                    if (chore.IsComplete)
                    {
                        Child.LifetimeEarnings -= chore.Worth;
                        Child.WeeklyEarnings -= chore.Worth;
                        await _childService.SaveChildAsync(Child);
                    }

                    Chores.Remove(chore);
                    FilteredChores.Remove(chore);
                    await _choreService.DeleteChoreAsync(chore);
                }
            }
        }
    }



    public class WeekDay : INotifyPropertyChanged
    {
        private bool _isSelected;
        public DateTime Date { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }



        public new event PropertyChangedEventHandler? PropertyChanged;
        protected new void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
