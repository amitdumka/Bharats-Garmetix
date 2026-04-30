using Garmetix.Core.Session;

namespace Garmetix.Core.Interfaces
{
    public interface ISessionService
    {
        static abstract string CompanyCode { get; }
        static abstract CurrentSession? CurrentSession { get; }
        static abstract bool IsAdminUser { get; }
        static abstract bool IsUserLoggedIn { get; }

        static abstract event Action OnSessionChanged;

        static abstract bool CanAddOrSaveRecords();

        static abstract bool CanDeleteRecords();

        static abstract bool CanEditRecords();

        static abstract bool CanViewRecords();

        static abstract bool CanViewReports();

        static abstract string? City();

        static abstract void ClearSession();

        static abstract string? CompanyName();

        static abstract string? CompanyStoreCode();

        static abstract string? Email();

        static abstract Task EndSessionAsync();

        static abstract Task<string> Get_Secure_Preference(string key);

        static abstract string? GroupName();

        static abstract string? GstNumber();

        static abstract bool IsAdmin();

        static abstract void NotifySessionChanged();

        static abstract string? Phone();

        static abstract Task StartSessionAsync(CurrentSession sessionInfo);

        static abstract string? StoreAddress();

        static abstract string? StoreName();

        static abstract Task<bool> TryLoadSessionAsync();

        static abstract Task UpdateSession();
    }
}