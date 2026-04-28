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
 * KeyPersonalInfo.cs
 * This code defines a C# class named `KeyPersonalInfo` within the `Garmetix.Models.Onboarding` namespace. 
 * The class contains properties for storing key personal information about a store manager and an accountant, 
 * including their names, email addresses, and phone numbers. Each property is decorated with data annotations 
 * to enforce validation rules such as required fields, string length limits, regular expression patterns, and 
 * specific formats for email addresses and phone numbers.
 * 
 */


using System.ComponentModel.DataAnnotations;

namespace Garmetix.Core.Models.Onboarding
{
    public class KeyPersonalInfo
    {
        //Write the Constructor to initialize the properties with default values
        public KeyPersonalInfo()
        {
            StoreManagerName = string.Empty;
            StoreManagerEmail = string.Empty;
            StoreManagerPhoneNumber = string.Empty;
            AccountantName = string.Empty;
            AccountantEmail = string.Empty;
            AccountantPhoneNumber = string.Empty;
        }



        [Display(Prompt = "Store Manager Name")]
        [Required(ErrorMessage = "Store Manager Name is required")]
        [MinLength(2, ErrorMessage = "Store Manager Name must be at least 2 characters long.")]
        [MaxLength(100, ErrorMessage = "Store Manager Name must be at most 100 characters long.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Store Manager Name can only contain letters and spaces.")]
        public string StoreManagerName { get; set; }

        [Required(ErrorMessage = "Store Manager Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [MaxLength(200, ErrorMessage = "Store Manager Email must be at most 200 characters long.")]
        [MinLength(5, ErrorMessage = "Store Manager Email must be at least 5 characters long.")]
        [Display(Prompt = "Store Manager Email")]
        public string StoreManagerEmail { get; set; }

        [Display(Prompt = "Store Manager Phone Number")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        [Required(ErrorMessage = "Store Manager Phone Number is required")]
        [MaxLength(15, ErrorMessage = "Store Manager Phone Number must be at most 15 characters long.")]
        [MinLength(10, ErrorMessage = "Store Manager Phone Number must be at least 10 characters long.")]
        [RegularExpression(@"^\+?[0-9\s-]+$", ErrorMessage = "Store Manager Phone Number can only contain numbers, spaces, dashes, and an optional leading plus sign.")]
        public string StoreManagerPhoneNumber { get; set; }

        [Display(Prompt = "Accountant Name")]
        [Required(ErrorMessage = "Accountant Name is required")]
        [MinLength(2, ErrorMessage = "Accountant Name must be at least 2 characters long.")]
        [MaxLength(100, ErrorMessage = "Accountant Name must be at most 100 characters long.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Accountant Name can only contain letters and spaces.")]
        public string AccountantName { get; set; }

        [Required(ErrorMessage = "Accountant Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [MaxLength(200, ErrorMessage = "Accountant Email must be at most 200 characters long.")]
        [MinLength(5, ErrorMessage = "Accountant Email must be at least 5 characters long.")]
        [Display(Prompt = "Accountant Email")]
        public string AccountantEmail { get; set; }

        [Display(Prompt = "Accountant Phone Number")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        [Required(ErrorMessage = "Accountant Phone Number is required")]
        [MaxLength(15, ErrorMessage = "Accountant Phone Number must be at most 15 characters long.")]
        [MinLength(10, ErrorMessage = "Accountant Phone Number must be at least 10 characters long.")]
        [RegularExpression(@"^\+?[0-9\s-]+$", ErrorMessage = "Accountant Phone Number can only contain numbers, spaces, dashes, and an optional leading plus sign.")]
        public string AccountantPhoneNumber { get; set; }
    }
}