using Bharat.ToolKits.Helpers;
using Bharat.ToolKits.Notifications;
using Garmetix.Authentication.Models;
using Garmetix.Core.Enums;
using Garmetix.Core.Models.Authentication;
using Garmetix.Core.Session;
using Garmetix.Core.Sessions;
using Garmetix.Core.Settings;
using Garmetix.Databases;
using Garmetix.Databases.Services;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;


namespace Garmetix.Authentication
{
    public class AuthenticationService
    {
        private DatabaseService _dataService;
        private static AuthenticationService? _instance;
        
        public static AuthenticationService Instance => _instance ??= new AuthenticationService();

        public AppUser? CurrentUser => _dataService.CurrentUser;

        public AuthenticationService()
        {

            _dataService = DatabaseService.Instance;
            _instance = this;
        }

        public AuthenticationService(DatabaseService databaseService)
        {
            _dataService = databaseService;
            _instance = this;
        }

        public bool DoLogout()
        {
            _dataService.CurrentUser = null;
            SecureStorage.Default.Remove("ActiveUserId");
            SecureStorage.Default.Remove("HasPin");

            // Simulate logout
            return true;
        }

        //Pin Unloacl
        public string HashString(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public async Task<bool> SetPinAsync(string pin)
        {
            if (CurrentUser == null || pin.Length != 4) return false;            
            CurrentUser.PinHash = HashString(pin);
            _dataService.ApplicationDB.AppUsers.Update(CurrentUser);
            await _dataService.ApplicationDB.SaveChangesAsync();
            await SecureStorage.Default.SetAsync("HasPin", "true");
            return true;
        }

        public async Task<bool> ValidatePinAsync(string pin)
        {
            string storedUserId = await SecureStorage.Default.GetAsync("ActiveUserId");
            if (string.IsNullOrEmpty(storedUserId)) return false;
            var user = await _dataService.ApplicationDB.AppUsers.Where(x => x.Id == Guid.Parse(storedUserId)).FirstOrDefaultAsync();

            if (user != null && user.PinHash == HashString(pin))
            {
                _dataService.CurrentUser = user;
                return true;
            }
            return false;
        }

        public void Logout()
        {
            _dataService.CurrentUser = null;
            SecureStorage.Default.Remove("ActiveUserId");
            SecureStorage.Default.Remove("HasPin");
        }
        //End Pin Unloacl


        public AppUser DoLogin(LoginInfo loginInfo)
        {
            // string hashedPwd = HashString(loginInfo.Password);
            var user = _dataService.ApplicationDB.AppUsers.Where(x => x.Email == loginInfo.Email && x.Password == loginInfo.Password).FirstOrDefault();
            if (user != null)
            {
                _dataService.CurrentUser = user;

                //TODO: Set The Session
                if (user != null)
                {
                    _dataService.CurrentUser = user;
                     SecureStorage.Default.SetAsync("ActiveUserId", user.Id.ToString());
                     
                }
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
               // string hashedPwd = HashString(loginInfo.Password);
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
                    CurrentSession session = new CurrentSession
                    {
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
                            SettingsServices.SetCompanyInfo(store.Company?.Name!, store.Company?.GSTIN!, store.StoreCode, store?.StoreGroup?.Name!);
                            SettingsServices.SetStoreInfo(store!.Name, store.Address, store.City, store.State, store.ZipCode, store.Email, store.ContactNumber);
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
                    Password = HashString(registerInfo.Password),
                    Name = registerInfo.Name,
                    StoreId = storeid,
                    Admin = false,
                    CompanyId = companyId,
                    StoreGroupId = groupid,
                    EmployeeId = registerInfo.Employee
                    ,
                    AppOperation =  AppOperation.Store,
                    Role =  LoginRole.Member,
                    UserName = registerInfo.Email.Split('@')[0],
                    UserType =  UserType.Guest,
                    RemoteUserId = Guid.Empty,
                    Id = Guid.NewGuid()

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