

namespace KidsChoresApp.Services
{
    public class AuthService
    {
        private const string AuthStateKey = "AuthState";
        private const string UserIdKey = "UserId";
        private readonly UserService _userService;
        private readonly ParentService _parentService;

        public AuthService(UserService userService, ParentService parentService)
        {
            _userService = userService;
            _parentService = parentService;
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            await Task.Delay(1000);
            return Preferences.Default.Get<bool>(AuthStateKey, false);
        }

        public int? GetUserId()
        {
            if (Preferences.Default.Get<bool>(AuthStateKey, false))
            {
                return Preferences.Default.Get<int>(UserIdKey, -1) != -1 ? Preferences.Default.Get<int>(UserIdKey, -1) : (int?)null;
            }
            return null;
        }

        public void Login(int userId)
        {
            Preferences.Default.Set<bool>(AuthStateKey, true);
            Preferences.Default.Set<int>(UserIdKey, userId);
        }

        public void Logout()
        {
            Preferences.Default.Remove(AuthStateKey);
            Preferences.Default.Remove(UserIdKey);
        }
    }
}