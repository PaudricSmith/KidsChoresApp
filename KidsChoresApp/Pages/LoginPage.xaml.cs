using KidsChoresApp.Services;
using System.Windows.Input;
using System.Text.RegularExpressions;
using KidsChoresApp.Models;


namespace KidsChoresApp.Pages
{
    public partial class LoginPage : ContentPage
    {
        private readonly UserService _userService;
        private readonly AuthService _authService;
        private readonly ParentService _parentService;
        private bool _isRegistering;
        private bool _isPasswordVisible;

        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public string ToggleLinkText => IsRegistering ? "or Sign In" : "or Sign Up";

        public bool IsRegistering
        {
            get => _isRegistering;
            set
            {
                _isRegistering = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ToggleLinkText));
                ConfirmPasswordEntry.IsVisible = _isRegistering;
            }
        }
        public bool IsPasswordVisible
        {
            get => _isPasswordVisible;
            set
            {
                _isPasswordVisible = value;
                OnPropertyChanged();
                PasswordEntry.IsPassword = !_isPasswordVisible;
                ConfirmPasswordEntry.IsPassword = !_isPasswordVisible;
            }
        }
       

        public ICommand ToggleRegisterLoginCommand { get; }
        public ICommand HowToUseCommand { get; }


        public LoginPage(AuthService authService, UserService userService, ParentService parentService)
        {
            InitializeComponent();
            _authService = authService;
            _userService = userService;
            _parentService = parentService;

            ToggleRegisterLoginCommand = new Command(OnToggleRegisterLogin);
            HowToUseCommand = new Command(OnHowToUseClicked);

            BindingContext = this;

            IsRegistering = false;
            IsPasswordVisible = false;
        }


        private void OnPasswordEyeToggleClicked(object sender, EventArgs e)
        {
            IsPasswordVisible = !IsPasswordVisible;
        }

        private async void OnSignUpButtonClicked(object sender, EventArgs e)
        {
            //// TESTING PURPOSES ONLY!!!!! ////////////////////////////////////////////////////////////////////
            Email = "testemail2@email.com";
            Password = "Password2!";
            ConfirmPassword = "Password2!";
            ////////////////////////////////////////////////////////////////////////////////////////////////////
            
            if (!ValidateInputs(Email, Password, ConfirmPassword))
            {
                return;
            }

            var success = await _userService.RegisterAsync(Email, Password);
            if (success)
            {
                // Get the newly created user
                var user = await _userService.GetUserByEmailAsync(Email);

                // Create a new parent account
                var parentAccount = new Parent { UserId = user.Id };

                await _parentService.SaveParentAsync(parentAccount);

                await DisplayAlert("Success", "Sign-Up successful", "OK");

                await Shell.Current.GoToAsync($"///{nameof(SetupPage)}?userId={user.Id}");
            }
            else
            {
                await DisplayAlert("Error", "Sign-Up failed. Email might already be taken.", "OK");
            }
        }

        private async void OnSignInButtonClicked(object sender, EventArgs e)
        {
            // TESTING PURPOSES ONLY!!!!! ////////////////////////////////////////////////////////////////////
            Email = "testemail2@email.com";
            Password = "Password2!";
            //////////////////////////////////////////////////////////////////////////////////////////////////

            if (!ValidateInputs(Email, Password))
            {
                return;
            }

            var success = await _userService.LoginAsync(Email, Password);
            if (success)
            {
                var user = await _userService.GetUserByEmailAsync(Email);
                if (!user.IsSetupCompleted)
                {
                    await Shell.Current.GoToAsync($"///{nameof(SetupPage)}?userId={user.Id}");
                }
                else
                {
                    _authService.Login(user.Id);

                    await Shell.Current.GoToAsync($"///{nameof(HomePage)}?userId={user.Id}");
                }
            }
            else
            {
                await DisplayAlert("Error", "Invalid login details. Please check your email and password.", "OK");
            }
        }

        private void OnToggleRegisterLogin()
        {
            IsRegistering = !IsRegistering;
        }

        private void OnHowToUseClicked()
        {
            DisplayAlert("How to use app with whole family",
                "The parent installs first on their phone and sets all the passwords, " +
                "then on the child's phone after they install, simply click on the child lock icon.",
                "OK");
        }

        private bool ValidateInputs(string email, string password, string? confirmPassword = null)
        {
            EmailErrorLabel.IsVisible = false;
            ConfirmPasswordErrorLabel.IsVisible = false;

            bool isValid = true;

            if (!ValidateEmail(email))
            {
                isValid = false;
            }

            if (!ValidatePassword(password))
            {
                isValid = false;
            }

            if (IsRegistering && password != confirmPassword)
            {
                ConfirmPasswordErrorLabel.Text = "Passwords do not match.";
                ConfirmPasswordErrorLabel.IsVisible = true;
                isValid = false;
            }

            return isValid;
        }

        private bool ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
            {
                EmailErrorLabel.Text = "Please enter a valid email address.";
                EmailErrorLabel.IsVisible = true;
                return false;
            }
            return true;
        }

        private bool ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || !IsValidPassword(password))
            {
                DisplayAlert("Error", "Password must be at least 8 characters long and include at least one capital letter, one number, and one special character.", "OK");
                return false;
            }
            return true;
        }

        private bool IsValidEmail(string email)
        {
            var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return regex.IsMatch(email);
        }

        private bool IsValidPassword(string password)
        {
            var regex = new Regex(@"^(?=.*[A-Z])(?=.*[!@#$%^&*(),.?""{}|<>])(?=.*[a-z])(?=.*[0-9]).{8,}$");
            return regex.IsMatch(password);
        }
    }
}
