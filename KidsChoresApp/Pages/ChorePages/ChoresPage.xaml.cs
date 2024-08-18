using KidsChoresApp.Models;
using KidsChoresApp.Pages.ChildPages;
using KidsChoresApp.Services;
using System.Collections.ObjectModel;


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
        private string? _choresForText;
        private int _childId;
        private bool _loading = true;

        public ObservableCollection<Chore> Chores { get; set; } = new ObservableCollection<Chore>();
        public ObservableCollection<Chore> FilteredChores { get; set; } = new ObservableCollection<Chore>();
        public ObservableCollection<DateTime> WeekDates { get; set; } = new ObservableCollection<DateTime>();

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
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
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
            MoneySummary.IsVisible = true;
        }

        private async Task LoadData()
        {
            if (ChildId != 0)
            {
                await Task.Run(async () =>
                {
                    Child = await _childService.GetChildByIdAsync(ChildId);
                    CurrentUser = await _userService.GetUserByIdAsync(Child.UserId);
                });
            }
        }

        private async Task LoadChores()
        {
            await Task.Run(async () =>
            {
                var chores = await _choreService.GetChoresByChildIdAsync(ChildId);

                if (chores.Count == 0)
                {
                    Loading = false;
                    return;
                }

                // Update the collection on the main thread
                Dispatcher.Dispatch(() =>
                {
                    Chores.Clear();
                    foreach (var chore in chores)
                    {
                        Chores.Add(chore);
                    }

                    FilterChoresByDate();
                    UpdateChoresForText();
                });

                Loading = false;
            });
        }

        //protected override bool OnBackButtonPressed()
        //{
        //    // Explicitly navigate back to the previous page
        //    Shell.Current.GoToAsync($".."); // Go up one level in the Shell navigation hierarchy
        //    return true;
        //}

        private void GenerateWeekDates()
        {
            var today = DateTime.Today;

            int dayValue = today.DayOfWeek == 0 ? 6 : (int)today.DayOfWeek - 1;

            var startOfWeek = today.AddDays(-dayValue);

            WeekDates.Clear();
            for (int i = 0; i < 7; i++)
            {
                WeekDates.Add(startOfWeek.AddDays(i));
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
            if (sender is Frame frame && frame.BindingContext is DateTime date)
            {
                if (_previouslySelectedFrame != null)
                    _previouslySelectedFrame.BackgroundColor = Colors.CornflowerBlue;

                SelectedDate = date;
                frame.BackgroundColor = Colors.DarkSlateBlue;
                _previouslySelectedFrame = frame;

                // Scroll to the selected day
                var itemIndex = WeekDates.IndexOf(date);
                WeekDaysCollectionView.ScrollTo(itemIndex, position: ScrollToPosition.Center, animate: true);

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
}
