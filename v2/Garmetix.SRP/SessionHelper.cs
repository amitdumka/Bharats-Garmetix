

using Garmetix.Core.Session;
using Garmetix.Databases.Services;
using System.Text.Json;

namespace Garmetix.Core.Sessions
{
    //TOOD: handle this some other way for database part

    public class SessionHelper
    {
        public static Task<CurrentSession?> TryLoadStoreSession()
        {
            return Task.Run(async delegate
            {
                CurrentSession? CurrentSession = null;
                string? pref = await SecureStorage.GetAsync(SessionService.SessionKey);
                if (pref != null)
                {
                    CurrentSession = JsonSerializer.Deserialize<CurrentSession>(pref);

                    if (CurrentSession != null)
                    {
                        // Optional: Validate session (e.g., token expiry if using tokens)
                        if (!CurrentSession.IsAutoLoginEnabled)
                        {
                            return null;
                        }
                        //TODO: what is can doCurrentSession.LoadFromXaml(SessionKey);

                        if (DateTime.Now > CurrentSession.LoginTime.AddHours(12))
                        {
                            CurrentSession.LoginTime = DateTime.Now;
                        }
                        //TODO: handle this some other way for database part

                        DatabaseService.CompanyId = CurrentSession.CompanyId.Value;
                        DatabaseService.StoreGroupId = CurrentSession.GroupId.Value;
                        DatabaseService.StoreId = CurrentSession.StoreId.Value;
                        DatabaseService.Instance.SetCurrentUser(CurrentSession.UserName, DatabaseService.CompanyId);

                        System.Diagnostics.Debug.WriteLine($"Session loaded for user: {CurrentSession.UserName}");
                        _ = SessionService.StartSessionAsync(CurrentSession);
                        return CurrentSession;
                    }
                }
                return CurrentSession;

            });
        }


    }
}