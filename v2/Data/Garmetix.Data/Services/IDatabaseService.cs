using Garmetix.Data.Databases;

namespace Garmetix.Databases.Services
{
    public interface IDatabaseService
    {
        private static DatabaseService _instance;

        ApplicationDatabaseContext ApplicationDB { get; }
        DatabaseContext LocalDB { get;  }
        public static DatabaseService Instance
        { get { return _instance; } }


        void SetCurrentUser(string userName, Guid companyId);
        bool ChangeCompanyDatabase(Guid companyId);
        Task<int> AddRange<T>(IEnumerable<T> items);
    }
}