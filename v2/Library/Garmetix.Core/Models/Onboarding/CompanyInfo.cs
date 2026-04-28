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
 * CompanyInfo.cs
 * 
 * This file defines the CompanyInfo class, which represents the company information required during the onboarding process. 
 * It includes properties such as CompanyName, GSTIN, PAN, CompanyType, CompanyEmail, CompanyPhoneNumber, DateOfIncorporation, and StoreCategory.
 * Each property is decorated with data annotations to enforce validation rules and provide user-friendly error messages.
 * The class is part of the Garmetix.Core.Models.Onboarding namespace and utilizes enums from Garmetix.Models.Stores for CompanyType and StoreCategory.
 */





using Garmetix.Core.Enums;
using Garmetix.Core.Models.Stores;
using System.ComponentModel.DataAnnotations;

namespace Garmetix.Core.Models.Onboarding
{
    public class CompanyInfo
    {
        public CompanyInfo()
        {


            CompanyName = string.Empty;
            GSTIN = string.Empty;
            PAN = string.Empty;
            CompanyEmail = string.Empty;
            CompanyPhoneNumber = string.Empty;
            DateOfIncorporation = DateTime.Now;
            CompanyType = CompanyType.Proprietorship;
            StoreCategory = StoreCategory.Retail;

        }


        [Required(ErrorMessage = "Company Name is required")]
        [MinLength(10, ErrorMessage = "Company Name must be at least 10 characters long.")]
        [MaxLength(200, ErrorMessage = "Company Name must be at most 200 characters long.")]
        [Display(Prompt = "Company Name")]
        [RegularExpression(@"^[a-zA-Z0-9\s.,'-]+$", ErrorMessage = "Company Name can only contain letters, numbers, spaces, and certain punctuation marks (.,'-).")]
        public string CompanyName { get; set; }
        [Display(Prompt = "Company GSTIN")]
        [RegularExpression(@"^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[1-9A-Z]{1}Z[0-9A-Z]{1}$", ErrorMessage = "Invalid GSTIN format.")]
        [Required(ErrorMessage = "GSTIN is required")]
        [MinLength(15, ErrorMessage = "GSTIN must be exactly 15 characters long.")]
        [MaxLength(15, ErrorMessage = "GSTIN must be exactly 15 characters long.")]

        public string GSTIN { get; set; }

        [Display(Prompt = "Company PAN")]
        [RegularExpression(@"^[A-Z]{5}[0-9]{4}[A-Z]{1}$", ErrorMessage = "Invalid PAN format.")]
        [Required(ErrorMessage = "PAN is required")]
        [MinLength(10, ErrorMessage = "PAN must be exactly 10 characters long.")]
        [MaxLength(10, ErrorMessage = "PAN must be exactly 10 characters long.")]
        public string PAN { get; set; }

        [Required(ErrorMessage = "Company Type is required")]
        [Display(Prompt = "Company Type")]
        [EnumDataType(typeof(CompanyType), ErrorMessage = "Invalid Company Type.")]
        [Range(0, 5, ErrorMessage = "Company Type must be a valid value between 0 and 5.")]
        public CompanyType CompanyType { get; set; }

        [Display(Prompt = "Company Email")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [Required(ErrorMessage = "Company Email is required")]
        [MaxLength(200, ErrorMessage = "Company Email must be at most 200 characters long.")]
        [MinLength(5, ErrorMessage = "Company Email must be at least 5 characters long.")]
        public string CompanyEmail { get; set; }

        [Display(Prompt = "Company Phone Number")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        [Required(ErrorMessage = "Company Phone Number is required")]
        [MaxLength(15, ErrorMessage = "Company Phone Number must be at most 15 characters long.")]
        [MinLength(10, ErrorMessage = "Company Phone Number must be at least 10 characters long.")]
        [RegularExpression(@"^\+?[0-9\s-]+$", ErrorMessage = "Company Phone Number can only contain numbers, spaces, dashes, and an optional leading plus sign.")]
        public string CompanyPhoneNumber { get; set; }

        [Display(Prompt = "Company Date of Incorporation")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Company Date of Incorporation is required")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Range(typeof(DateTime), "1947-01-01", "2100-12-31", ErrorMessage = "Company Date of Incorporation must be between 1900 and 2100.")]
        public DateTime? DateOfIncorporation { get; set; }

        [Display(Prompt = "Store Category")]
        [EnumDataType(typeof(StoreCategory), ErrorMessage = "Invalid Store Category.")]
        [Required(ErrorMessage = "Store Category is required")]
        [Range(0, 5, ErrorMessage = "Store Category must be a valid value between 0 and 5.")]
        public StoreCategory StoreCategory { get; set; }
    }
}
