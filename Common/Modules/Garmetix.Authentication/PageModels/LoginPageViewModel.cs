
using Garmetix.Authentication.Models;

namespace Garmetix.Authentication.PageModels
{
    public class LoginPageViewModel
    {
        public LoginInfo LoginInfo { get; set; }

        #region Constructor

        public LoginPageViewModel()
        {
            LoginInfo = new LoginInfo
            {
                Email = "storemanager@aadwikafashion.in"
            ,
                Password = "StoreManager@1234",
                RememberMe = true
            };
        }

        #endregion Constructor
    }
}