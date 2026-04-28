/*
 * Garmetix
 * Author: Amit Kumar
 * https://garmetix.com/
 * Copyright (c) 2026. All rights reserved.
 * Version: 6.0.0
 * License: https://garmetix.com/license
 * Website: https://garmetix.com/
*/
/* * ClientInfo.cs
 * 
 * This class represents the client information required during the onboarding process.
 * It includes properties for first name, last name, email, password, phone number,
 * date
 */
using Garmetix.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Garmetix.Core.Models.Onboarding
{
    public class ClientInfo
    {
        public ClientInfo()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            Email = string.Empty;
            Password = string.Empty;
            PhoneNumber = string.Empty;
        }

        [Display(Prompt = "First Name", Name = "First Name")]
        [Required(ErrorMessage = "First name is required.")]
        [MinLength(2, ErrorMessage = "First name must be at least 2 characters.")]
        [MaxLength(50, ErrorMessage = "First name must be at most 50 characters.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "First name can only contain letters.")]
        public string FirstName { get; set; }

        [Display(Prompt = "Last Name")]
        [Required(ErrorMessage = "Last name is required.")]
        [MinLength(2, ErrorMessage = "Last name must be at least 2 characters.")]
        [MaxLength(50, ErrorMessage = "Last name must be at most 50 characters.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Last name can only contain letters.")]
        public string LastName { get; set; }

        [Display(Prompt = "Email")]
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [MaxLength(200, ErrorMessage = "Email must be at most 200 characters.")]

        public string Email { get; set; }

        [Display(Prompt = "Password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        public string Password { get; set; } // Remember to handle password security appropriately

        [Display(Prompt = "Phone Number")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        [Required(ErrorMessage = "Phone number is required.")]
        [MinLength(10, ErrorMessage = "Phone number must be at least 10 digits."), MaxLength(13, ErrorMessage = "Phone number must be at most 13 digits.")]
        public string PhoneNumber { get; set; }

        [Display(Prompt = "Date of Birth")]
        [Required(ErrorMessage = "Date of birth is required.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateOfBirth { get; set; } = DateTime.Now.Date.AddYears(-18); // Default to 18 years ago

        [Display(Prompt = "Gender")]
        [Required(ErrorMessage = "Gender is required.")]

        public Gender Gender { get; set; } = Gender.Male;
    }
}