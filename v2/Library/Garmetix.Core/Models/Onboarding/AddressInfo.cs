using System.ComponentModel.DataAnnotations;

namespace Garmetix.Models.Onboarding
{
    public class AddressInfo
    {
        [Display(Prompt = "Street Address")]
        [Required(ErrorMessage = "Street Address is required")]
        [MinLength(5, ErrorMessage = "Street Address must be at least 5 characters long.")]
        public string StreetAddress { get; set; }

        [Required(ErrorMessage = "City is required")]
        [MinLength(2, ErrorMessage = "City must be at least 2 characters long.")]
        [MaxLength(50, ErrorMessage = "City must be at most 50 characters long.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "City must contain only letters and spaces.")]
        [Display(Prompt = "City")]
        public string City { get; set; }

        [Display(Prompt = "State")]
        [MinLength(2, ErrorMessage = "State must be at least 2 characters long.")]
        [Required(ErrorMessage = "State or Province is required")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "State or Province must contain only letters and spaces.")]
        [MaxLength(50, ErrorMessage = "State or Province must be at most 50 characters long.")]
        public string StateOrProvince { get; set; }

        [Display(Prompt = "Postal Code")]
        [MinLength(5, ErrorMessage = "Postal Code must be at least 5 characters long.")]
        [MaxLength(6, ErrorMessage = "Postal Code must be at most 6 characters long.")]
        [Required(ErrorMessage = "Postal Code is required")]
        [DataType(DataType.PostalCode)]
        [RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = "Invalid Postal Code format.")]

        public string PostalCode { get; set; }

        [Display(Prompt = "Country")]
        [MinLength(2, ErrorMessage = "Country must be at least 2 characters long.")]
        [Required(ErrorMessage = "Country is required")]
        [MaxLength(50, ErrorMessage = "Country must be at most 50 characters long.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Country must contain only letters and spaces.")]
        public string Country { get; set; }
    }
}