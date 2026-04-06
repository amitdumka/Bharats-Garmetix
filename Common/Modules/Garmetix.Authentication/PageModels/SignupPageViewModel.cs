using Garmetix.Authentication.Models;

namespace Garmetix.Authentication.PageModels
{
    public class SignupPageViewModel
    {
        public SignUpInfo SignUpInfo { get; set; }

        #region Constructor

        public SignupPageViewModel()
        {
            SignUpInfo = new SignUpInfo();
        }

        #endregion Constructor
    }
}