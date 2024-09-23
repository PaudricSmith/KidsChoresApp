using KidsChoresApp.Models;
using KidsChoresApp.Services;


namespace KidsChoresApp.Pages
{
    public partial class SetupPage : ContentPage
    {
        private readonly AuthService _authService;
        private readonly UserService _userService;
        private readonly ParentService _parentService;

        private User? _user;
        private List<Entry> _passcodeEntries;

        public int UserId { get; set; }


        public SetupPage(AuthService authService, UserService userService, ParentService parentService)
        {
            InitializeComponent();

            _authService = authService;
            _userService = userService;
            _parentService = parentService;
            _passcodeEntries = new List<Entry> { Digit1, Digit2, Digit3, Digit4 };

            // Adding TapGestureRecognizers to ensure focus is properly managed
            foreach (var entry in _passcodeEntries)
            {
                entry.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(OnDigitEntryClicked)
                });
            }
        }


        private void OnDigitEntryClicked()
        {
            KeypadViewComponent.IsVisible = true;
        }

        private void OnKeypadNumClicked(object _, string digit)
        {
            foreach (var entry in _passcodeEntries)
            {
                if (string.IsNullOrEmpty(entry.Text))
                {
                    entry.Text = digit;
                    break;
                }
            }
        }

        private void OnBackspaceClicked(object _, EventArgs e)
        {
            for (int i = _passcodeEntries.Count - 1; i >= 0; i--)
            {
                if (!string.IsNullOrEmpty(_passcodeEntries[i].Text))
                {
                    _passcodeEntries[i].Text = string.Empty;
                    break;
                }
            }
        }

        private async void OnSavePasscodeClicked(object sender, EventArgs e)
        {
            string passcode = string.Concat(_passcodeEntries.Select(entry => entry.Text));

            if (passcode.Length != 4)
            {
                await DisplayAlert("Error", "Please enter a 4-digit passcode.", "OK");
                return;
            }

            UserId = _authService.GetUserId() ?? 0;
            _user = await _userService.GetUserByIdAsync(UserId);

            // Retrieve the existing parent record using the UserId
            var parent = await _parentService.GetParentByUserIdAsync(_user.Id);

            if (parent == null)
            {
                await DisplayAlert("Error", "Parent record not found.", "OK");
                return;
            }

            // Update the parent's passcode
            parent.Passcode = passcode;
            await _parentService.SaveParentAsync(parent);

            // Update the user's setup completion status
            _user.IsSetupCompleted = true;
            await _userService.SaveUserAsync(_user);

            await Shell.Current.GoToAsync($"///{nameof(HomePage)}");
        }
    }
}
