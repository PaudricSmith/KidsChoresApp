using KidsChoresApp.Models;
using System.Collections.ObjectModel;


namespace KidsChoresApp.Pages
{
    public partial class HomePage : ContentPage
    {
        public ObservableCollection<Child> Children { get; set; }


        public HomePage()
        {
            InitializeComponent();

            Children = new ObservableCollection<Child>();

            BindingContext = this;

            Loaded += OnPageLoaded;
        }


        private async void OnPageLoaded(object? sender, EventArgs e)
        {
            await LoadData();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadData();
        }

        private async Task LoadData()
        {
            // Simulate loading data
            await Task.Delay(1000);

            var children = new ObservableCollection<Child>
            {
                new()
                {
                    Id = 1,
                    Name = "John Doe",
                    Money = 10.5m,
                    WeeklyEarnings = 5.0m,
                    LifetimeEarnings = 50.0m,
                    Image = "batboy" 
                },
                new()
                {
                    Id = 2,
                    Name = "Jane Doe",
                    Money = 15.75m,
                    WeeklyEarnings = 7.0m,
                    LifetimeEarnings = 70.0m,
                    Image = "batgirl" 
                },
                new()
                {
                    Id = 3,
                    Name = "Tom Doe",
                    Money = 150.05m,
                    WeeklyEarnings = 7.0m,
                    LifetimeEarnings = 70.0m,
                    Image = "flashboy" 
                },
                new()
                {
                    Id = 4,
                    Name = "Tanya Doe",
                    Money = 15.75m,
                    WeeklyEarnings = 7.0m,
                    LifetimeEarnings = 70.0m,
                    Image = "flashgirl" 
                },
                new()
                {
                    Id = 5,
                    Name = "Mick Doe",
                    Money = 1500.70m,
                    WeeklyEarnings = 7.0m,
                    LifetimeEarnings = 70.0m,
                    Image = "hulkboy" 
                },
                new()
                {
                    Id = 6,
                    Name = "Michelle Doe",
                    Money = 15.75m,
                    WeeklyEarnings = 7.0m,
                    LifetimeEarnings = 70.0m,
                    Image = "hulkgirl" 
                },
                new()
                {
                    Id = 7,
                    Name = "Ben Doe",
                    Money = 15000.75m,
                    WeeklyEarnings = 7.0m,
                    LifetimeEarnings = 70.0m,
                    Image = "spiderboy" 
                },
                new()
                {
                    Id = 8,
                    Name = "Brenda Doe",
                    Money = 15.75m,
                    WeeklyEarnings = 7.0m,
                    LifetimeEarnings = 70.0m,
                    Image = "spidergirl" 
                },
                new()
                {
                    Id = 9,
                    Name = "Frank Doe",
                    Money = 15.75m,
                    WeeklyEarnings = 7.0m,
                    LifetimeEarnings = 70.0m,
                    Image = "superboy" 
                },
                new()
                {
                    Id = 10,
                    Name = "Francis Doe",
                    Money = 15.75m,
                    WeeklyEarnings = 7.0m,
                    LifetimeEarnings = 70.0m,
                    Image = "supergirl" 
                }

            };

            Children.Clear();
            foreach (var child in children)
            {
                Children.Add(child);
            }
        }

        private async void OnAddChildClicked(object sender, EventArgs e)
        {

        }

        private async void OnAssignChoresClicked(object sender, EventArgs e)
        {

        }
    }
}
