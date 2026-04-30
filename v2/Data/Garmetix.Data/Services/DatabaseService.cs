using Garmetix.Core.Models.Authentication;
using Garmetix.Core.Session;
using Garmetix.Data.Databases;
using Garmetix.Databases.Seeds; 

namespace Garmetix.Databases.Services
{
    //TODO: need to implements neccessary  changes and additional to meet the requirements of the application
    public class DatabaseService: IDatabaseService
    {
        public static Guid CompanyId;
        public static Guid StoreGroupId;
        public static Guid StoreId;

        //public static void UpdateSession()
        //{
        //    DatabaseService.CompanyId = CurrentSession.CompanyId.Value;
        //    DatabaseService.StoreGroupId = CurrentSession.GroupId.Value;
        //    DatabaseService.StoreId = CurrentSession.StoreId.Value;
        //    DatabaseService.Instance.SetCurrentUser(CurrentSession.UserName, DatabaseService.CompanyId);

        //    System.Diagnostics.Debug.WriteLine($"Session loaded for user: {CurrentSession.UserName}");
        //    _ = SessionService.StartSessionAsync(CurrentSession);
        //}


        public AppUser CurrentUser { get; set; } = null!; // Initialize with a non-null default value
        public DatabaseContext LocalDB { get; private set; }
        public ApplicationDatabaseContext ApplicationDB { get; private set; }

        private static DatabaseService? _instance;

        public static DatabaseService Instance
        { get { return _instance!; } }

        public void SetCurrentUser(string userName, Guid companyId)
        {
            if (CurrentUser != null)
            {
                return;
            }

            var user = ApplicationDB.AppUsers.Where(c => c.UserName == userName && c.CompanyId == companyId).FirstOrDefault();
            if (user == null)
            { return; }
            CurrentUser = user;
        }

        public DatabaseService(ApplicationDatabaseContext adb, DatabaseContext ldb)
        {
            LocalDB = ldb;
            ApplicationDB = adb;
            //TODO: VerifyCompany();
            _instance = this;
        }

        /// <summary>
        /// Change Company Database
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public bool ChangeCompanyDatabase(Guid companyId)
        {
            var companyName = ApplicationDB.Companies.Find(companyId)!.Code.Trim().ToLower();
            if (companyName != null)
            {
                Constants.CompanyDatabaseFileName = $"{companyName}{Constants.DatabaseFilename}";
                LocalDB.Reconfigure(Constants.CompanyDatabaseFileName);
                CompanyId = companyId;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Verify Company Database exists or not
        /// </summary>
        /// <returns></returns>

        public async Task<bool> VerifyCompanyAsync()
        {
            if (!CheckInDatabase(ApplicationDB))
            {
                return (await new Seeder().InitAadwikaFashionAmitKumarAsync()).Contains("Company Database Created") ? true : false;
            }
            return true;
        }

        private static bool CheckInDatabase(ApplicationDatabaseContext db)
        {
            db ??= new ApplicationDatabaseContext();

            try
            {
                int count = db.Companies.Count();
                count += db.StoreGroups.Count();
                count += db.Stores.Count();
                return count >= 3;
            }
            catch (Exception)
            {
                // TODO: Implement to fetch database details from Cloud Server for restoration of data.
                return false;
            }
        }

        public static async Task SeedDatabaseAsync(string selectedSeed)
        {
            if (selectedSeed.Contains("Amit Kumar"))
            {
                await Task.Run(() => new Seeder().InitAadwikaFashionAmitKumarAsync());
            }
            else if (selectedSeed.Contains("Shalini Kumari"))
            {
                await Task.Run(() => new Seeder().InitAadwikaFashionShaliniKumari());
            }
            else
            {
                throw new ArgumentException("Invalid seed option selected.");
            }
        }

        public async Task<int> AddRange<T>(IEnumerable<T> items)
        {
            LocalDB.AddRange(items);
            return await LocalDB.SaveChangesAsync();
        }


         
    }
}