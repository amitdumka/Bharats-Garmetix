/*
 * Garmetix
 * Author: Amit Kumar
 * https://garmetix.com/
 * Copyright (c) 2026. All rights reserved.
 * Version: 6.0.0
 * License: https://garmetix.com/license
 * Website: https://garmetix.com/
*/
/*
 * AddressInfo.cs
 * 
 * This class represents the address information of a user during the onboarding process.
 * It includes properties for street address, city, state or province, postal code, and country.
 * Each property is decorated with data annotations to enforce validation rules and provide display prompts.
 *
 */

using System.ComponentModel.DataAnnotations;

namespace Garmetix.Core.Models.Onboarding
{
    public class AddressInfo
    {
        [Display(Prompt = "Street Address")]
        [Required(ErrorMessage = "Street Address is required")]
        [MinLength(5, ErrorMessage = "Street Address must be at least 5 characters long.")]
        public string StreetAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required")]
        [MinLength(2, ErrorMessage = "City must be at least 2 characters long.")]
        [MaxLength(50, ErrorMessage = "City must be at most 50 characters long.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "City must contain only letters and spaces.")]
        [Display(Prompt = "City")]
        public string City { get; set; } = string.Empty;

        [Display(Prompt = "State")]
        [MinLength(2, ErrorMessage = "State must be at least 2 characters long.")]
        [Required(ErrorMessage = "State or Province is required")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "State or Province must contain only letters and spaces.")]
        [MaxLength(50, ErrorMessage = "State or Province must be at most 50 characters long.")]
        public string StateOrProvince { get; set; } = string.Empty;
        [Display(Prompt = "Postal Code")]
        [MinLength(5, ErrorMessage = "Postal Code must be at least 5 characters long.")]
        [MaxLength(6, ErrorMessage = "Postal Code must be at most 6 characters long.")]
        [Required(ErrorMessage = "Postal Code is required")]
        [DataType(DataType.PostalCode)]
        [RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = "Invalid Postal Code format.")]
        public string PostalCode { get; set; } = string.Empty;

        [Display(Prompt = "Country")]
        [MinLength(2, ErrorMessage = "Country must be at least 2 characters long.")]
        [Required(ErrorMessage = "Country is required")]
        [MaxLength(50, ErrorMessage = "Country must be at most 50 characters long.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Country must contain only letters and spaces.")]
        public string Country { get; set; } = string.Empty;
    }
}