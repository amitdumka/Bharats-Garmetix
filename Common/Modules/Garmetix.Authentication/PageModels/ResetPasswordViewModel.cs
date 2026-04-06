

using Garmetix.Authentication.Models;

namespace Garmetix.Modules.Auth.PageModels
{
    class ResetPasswordViewModel
    {
        public ResetPasswordInfo ResetPasswordInfo { get; set; }

        #region Constructor
        public ResetPasswordViewModel()
        {
            this.ResetPasswordInfo = new ResetPasswordInfo();
        }

        #endregion
    }


}
