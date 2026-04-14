using System.Security.Cryptography;
using System.Text;
using Garmetix.AI.Billing.Models;
using Garmetix.Billing.AIBased.Helpers;

namespace Garmetix.AI.Billing.Services
{
    public class AuthService
    {
        private static AuthService _instance;
        public static AuthService Instance => _instance ??= new AuthService();

        public User CurrentUser { get; private set; }

        public string HashString(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public async Task<bool> AuthenticateAsync(string username, string password)
        {
            var db = await DatabaseHelper.GetDatabaseAsync();
            string hashedPwd = HashString(password);

            var user = await db.Table<User>()
                .Where(u => u.Username.ToLower() == username.ToLower() && u.PasswordHash == hashedPwd && u.IsActive)
                .FirstOrDefaultAsync();

            if (user != null)
            {
                CurrentUser = user;
                await SecureStorage.Default.SetAsync("ActiveUserId", user.Id.ToString());
                return true;
            }
            return false;
        }

        public async Task<bool> SetPinAsync(string pin)
        {
            if (CurrentUser == null || pin.Length != 4) return false;

            var db = await DatabaseHelper.GetDatabaseAsync();
            CurrentUser.PinHash = HashString(pin);
            await db.UpdateAsync(CurrentUser);

            await SecureStorage.Default.SetAsync("HasPin", "true");
            return true;
        }

        public async Task<bool> ValidatePinAsync(string pin)
        {
            string storedUserId = await SecureStorage.Default.GetAsync("ActiveUserId");
            if (string.IsNullOrEmpty(storedUserId)) return false;

            var db = await DatabaseHelper.GetDatabaseAsync();
            var user = await db.Table<User>().Where(u => u.Id == Guid.Parse(storedUserId)).FirstOrDefaultAsync();

            if (user != null && user.PinHash == HashString(pin))
            {
                CurrentUser = user;
                return true;
            }
            return false;
        }

        public void Logout()
        {
            CurrentUser = null;
            SecureStorage.Default.Remove("ActiveUserId");
            SecureStorage.Default.Remove("HasPin");
        }
    }
}
