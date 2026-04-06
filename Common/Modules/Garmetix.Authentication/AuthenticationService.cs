using Bharat.ToolKits.Helpers;
using Bharat.ToolKits.Notifications;
using Garmetix.Authentication.Models; 
using Garmetix.Databases;
using Garmetix.Databases.Services;
using Garmetix.Models.Auth;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Garmetix.Authentication
{
    public class AuthenticationService
    {
        private DatabaseService _dataService;
        public static AuthenticationService  Instance { get; private set; }= new AuthenticationService();
        public AuthenticationService() { 
        
            _dataService = DatabaseService.Instance;
            Instance = this;
        }

        public AuthenticationService(DatabaseService databaseService)
        {
            _dataService = databaseService;
            Instance = this;
        }

        public bool DoLogout()
        {
            _dataService.CurrentUser = null;

            // Simulate logout
            return true;
        }

        public AppUser DoLogin(LoginInfo loginInfo)
        {
            var user = _dataService.ApplicationDB.AppUsers.Where(x => x.Email == loginInfo.Email && x.Password == loginInfo.Password).FirstOrDefault();
            if (user != null)
            {
                _dataService.CurrentUser = user;
                //TODO: Set The Session
                return user;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Login
        /// </summary>
        /// <param name="loginInfo"></param>
        /// <returns></returns>
        public AppUser LoginUser(LoginInfo loginInfo)
        {
            try
            {
                var user = _dataService.ApplicationDB.AppUsers.Where(x => x.Email == loginInfo.Email && x.Password == loginInfo.Password).FirstOrDefault();
            
               return user;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Login Error: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Posts the login. Do Post operation after login,
        /// Setting Session,  Setting Store Info and its settings
        /// Use Compay Based Database
        /// </summary>
        /// <param name="user"> Users Details</param>
        /// <param name="rememberMe"></param>
        /// <returns></returns>
        public Task PostLogin(AppUser user, bool rememberMe)
        {
            //TODO: move to Auth Service 
            return Task.Run(async delegate
            {
                try
                {
                    StorageOps.SetPref("EnableAutoLogin", rememberMe);
                    var store = _dataService.ApplicationDB.Stores.Include(x => x.Company).Include(x => x.StoreGroup).Where(x => x.Id == user.StoreId).FirstOrDefault();
                    CurrentSession session = new CurrentSession {
                        IsAutoLoginEnabled = rememberMe,
                        CompanyId = user.CompanyId,
                        UserName = user.UserName,
                        UserRole = user.Role,
                        StoreId = user.Id,
                        GroupId = user.StoreGroupId,
                        LoginTime = DateTime.Now,
                        EmployeeId = user.EmployeeId ?? Guid.Empty,

                    };

                    if (store != null)
                    {
                        session.CompanyDatabaseFileName = $"{store.Company?.Code}{Constants.DatabaseFilename}";
                        if (user.EmployeeId != Guid.Empty && user.EmployeeId != null)
                        {
                            session.EmployeeId = user.EmployeeId;
                            session.EmployeeName = _dataService.LocalDB.Employees.Find(user.EmployeeId)?.StaffName ?? string.Empty;
                        }
                    }
                    else
                    {
                        
                        await Notify.DisplayNotificationAsync("Company/Store Not found, Do Bussiness Registration", speak: true);
                    }
                    _ = SessionService.StartSessionAsync(session);
                    _ = Task.Run(delegate
                    {
                        if (store != null)
                        {
                            SettingsService.SetCompanyInfo(store.Company?.Name!, store.Company?.GSTIN!, store.StoreCode, store?.StoreGroup?.Name!);
                            SettingsService.SetStoreInfo(store!.Name, store.Address, store.City, store.State, store.ZipCode, store.Email, store.ContactNumber);
                        }
                        _dataService.CurrentUser = user;
                        DatabaseService.CompanyId = store?.CompanyId ?? user.CompanyId!.Value;
                        DatabaseService.StoreGroupId = store?.StoreGroupId ?? user.StoreGroupId!.Value;
                        _dataService.LocalDB.Reconfigure(session.CompanyDatabaseFileName!);
                        DatabaseService.StoreId = store!.Id;
                    });
                }
                catch (Exception ex)
                {
                    await Notify.DisplayNotificationAsync(ex.Message, speak: true);
                }
            });
        }

        public AppUser RegisterUser(SignUpInfo registerInfo, Guid storeid, Guid companyId, Guid groupid)
        {
            try
            {
                var existingUser = _dataService.ApplicationDB.AppUsers.FirstOrDefault(x => x.Email == registerInfo.Email);
                if (existingUser != null)
                {
                    Debug.WriteLine("User already exists with this email.");
                    return null;
                }
                var newUser = new AppUser
                {
                    Email = registerInfo.Email,
                    Password = registerInfo.Password,
                    Name = registerInfo.Name,
                    StoreId =storeid, Admin = false,
                    CompanyId = companyId,
                    StoreGroupId = groupid, EmployeeId = registerInfo.Employee
                    ,AppOperation =Garmetix.Models.Enums.AppOperation.Store, 
                    Role = Garmetix.Models.Enums.LoginRole.Member, 
                    UserName = registerInfo.Email.Split('@')[0],UserType = Garmetix.Models.Enums.UserType.Guest, 
                    RemoteUserId=Guid.Empty, Id = Guid.NewGuid()

                };
                _dataService.ApplicationDB.AppUsers.Add(newUser);
                _dataService.ApplicationDB.SaveChanges();
                return newUser;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Registration Error: " + ex.Message);
                return null;
            }
        }
    }
}