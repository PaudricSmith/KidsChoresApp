using KidsChoresApp.Models;
using KidsChoresApp.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace KidsChoresApp.Pages.ChildPages
{
    [QueryProperty(nameof(ChildId), "childId")]
    public partial class ChildPage : ContentPage, INotifyPropertyChanged
    {
        private readonly ChildService _childService;
        private readonly ChoreService _choreService;

        private Child _child;
        private int _childId;

        public ObservableCollection<Chore> Chores { get; set; } = [];

        public Child Child
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
                LoadChildData(value);
            }
        }


        public ChildPage(ChildService childService, ChoreService choreService)
        {
            InitializeComponent();

            _childService = childService;
            _choreService = choreService;

            BindingContext = this;
        }


        private async void LoadChildData(int childId)
        {
            Child = await _childService.GetChildAsync(childId);

            if (Child != null)
            {
                LoadChores(childId);
            }
        }

        private async void LoadChores(int childId)
        {
            var chores = await _choreService.GetChoresByChildIdAsync(childId);

            Chores.Clear();
            foreach (var chore in chores)
            {
                Chores.Add(chore);
            }
        }

        private async void OnChoreCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.BindingContext is Chore chore)
            {
                if (checkBox.IsChecked)
                {
                    Child.LifetimeEarnings += chore.Worth;
                    Child.WeeklyEarnings += chore.Worth;
                }
                else
                {
                    Child.LifetimeEarnings -= chore.Worth;
                    Child.WeeklyEarnings -= chore.Worth;
                }

                chore.IsComplete = checkBox.IsChecked;

                await _choreService.SaveChoreAsync(chore); 
                await _childService.SaveChildAsync(Child); 
            }
        }

        private async void OnChangeImageClicked(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet("Choose an image", "Cancel", null, "Choose from library", "Take a photo", "Select from avatars");
            switch (action)
            {
                case "Choose from library":
                    // Implement photo library selection
                    break;
                case "Take a photo":
                    // Implement photo capture
                    break;
                case "Select from avatars":
                    // Implement avatar selection
                    break;
            }
        }

        private void OnNameFrameTapped(object sender, EventArgs e)
        {
            NameLabel.IsVisible = false;
            NameEntry.IsVisible = true;
            NameEntry.Focus();
        }

        private void OnNameEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue.Length > 9)
            {
                //AdjustFontSize((Entry)sender);
            }
        }

        private void AdjustFontSize(Entry entry)
        {
            if (entry.Text.Length > 9)
            {
                entry.FontSize = 20;
            }
            else
            {
                entry.FontSize = 24;
            }
        }

        private async void OnNameEntryCompleted(object sender, EventArgs e)
        {
            if (Child != null)
            {
                await _childService.SaveChildAsync(Child);
                OnPropertyChanged(nameof(Child));
            }

            NameLabel.IsVisible = true;
            NameEntry.IsVisible = false;
        }



        public new event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
