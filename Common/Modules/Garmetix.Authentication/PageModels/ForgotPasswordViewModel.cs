

using Garmetix.Authentication.Models;

namespace Garmetix.Authentication.PageModels
{
    class ForgotPasswordViewModel
    {
        public ForgotPasswordInfo ForgotPasswordInfo { get; set; }

        #region Constructor
        public ForgotPasswordViewModel()
        {
            ForgotPasswordInfo = new ForgotPasswordInfo();
        }

        #endregion
    }


}
