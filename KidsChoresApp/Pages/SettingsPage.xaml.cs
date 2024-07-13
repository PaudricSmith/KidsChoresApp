

namespace KidsChoresApp.Pages
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();

            BindingContext = this;
        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();
        }

        private async void OnCurrencySelected(object sender, EventArgs e)
        {

        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {

        }

        private async void OnChangePasscodeClicked(object sender, EventArgs e)
        {
            
        }
    }
}
