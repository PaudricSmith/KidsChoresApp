using KidsChoresApp.Models;
using KidsChoresApp.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;


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

        public ICommand ChoreCheckedChangedCommand { get; }

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

            ChoreCheckedChangedCommand = new Command<Chore>(OnChoreCheckedChanged);

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

        private async void OnChoreCheckedChanged(Chore chore)
        {
            if (chore == null) return;

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

            OnPropertyChanged(nameof(Child));
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

        private async void OnNameEntryCompleted(object sender, EventArgs e)
        {
            if (Child != null)
            {
                OnPropertyChanged(nameof(Child));

                await _childService.SaveChildAsync(Child);
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
