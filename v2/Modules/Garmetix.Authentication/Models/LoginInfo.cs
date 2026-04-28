using Garmetix.Core.Enums;
using Syncfusion.Maui.DataForm;
using System.ComponentModel.DataAnnotations;

namespace Garmetix.Authentication.Models
{
    public class SignUpInfo
    {
        public SignUpInfo()
        {
            Name = string.Empty;
            Email = string.Empty;
            Password = string.Empty;
            ConfirmPassword = string.Empty;
            Employee = null;
        }

        [Display(Name = "Name")]
        [DataFormDisplayOptions(ColumnSpan = 3)]
        [Required(ErrorMessage = "Enter your name")]
        public string Name { get; set; }

        [Display(Name = "Email")]
        [DataFormDisplayOptions(ColumnSpan = 3)]
        [EmailAddress(ErrorMessage = "Enter your email")]
        public string Email { get; set; }

        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        [Required(ErrorMessage = "Enter the phone number")]
        [DataFormDisplayOptions(ColumnSpan = 3)]
        public string Password { get; set; }

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Enter the password")]
        [DataFormDisplayOptions(ColumnSpan = 3)]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Employee")]

        [Required(ErrorMessage = "Select Employee")]
        [DataFormDisplayOptions(ColumnSpan = 3)]

        public Guid? Employee { get; set; }
    }
    public class ResetPasswordInfo
    {
        public ResetPasswordInfo()
        {
            NewPassword = string.Empty;
            ConfirmPassword = string.Empty;
        }

        [Display(Name = "New Password")]
        [DataFormDisplayOptions(ColumnSpan = 3)]
        [EmailAddress(ErrorMessage = "Enter your new password")]
        public string NewPassword { get; set; }

        [Display(Name = "Confirm New Password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Enter the password")]
        [DataFormDisplayOptions(ColumnSpan = 3)]
        public string ConfirmPassword { get; set; }
    }
    public class ForgotPasswordInfo
    {
        public ForgotPasswordInfo()
        {
            Email = string.Empty;
        }

        [Display(Name = "Enter mail id")]
        [DataFormDisplayOptions(ColumnSpan = 3)]
        [EmailAddress(ErrorMessage = "Enter your email")]
        public string Email { get; set; }
    }
    public class ApproveUser
    {
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public LoginRole Role { get; set; }
    }
    public class LoginInfo
    {
        public LoginInfo()
        {
            Email = string.Empty;
            Password = string.Empty;
        }
        [Display(Name = "Email")]
        [DataFormDisplayOptions(ColumnSpan = 3)]
        [EmailAddress(ErrorMessage = "Enter your email")]
        public string Email { get; set; } = "admin@aadwikafashion.com";

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Enter the password")]
        [DataFormDisplayOptions(ColumnSpan = 3)]
        public string Password { get; set; } = "Admin@1234";

        public bool RememberMe { get; set; } = true;
    }
}
