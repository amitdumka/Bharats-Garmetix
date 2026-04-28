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
 * CompanyConfigInfo.cs
 *
 * This file defines the CompanyConfigInfo class, which represents the configuration information for a company during the onboarding process.
 * It includes properties for Client Code, Groud Code, Store Code, and Operation Mode, along with validation attributes to ensure data integrity.
 */
using Garmetix.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Garmetix.Core.Models.Onboarding
{
    public class CompanyConfigInfo
    {
        public CompanyConfigInfo()
        {
                ClientCode = string.Empty;
                GroudCode = string.Empty;
                StoreCode = string.Empty;
                OperationMode = AppOperation.Store; // Default to Store, can be changed as needed
        }

        [Display(Name = "Client Code")]
        [Required(ErrorMessage = "Client Code is required")]
        [MinLength(2, ErrorMessage = "Client Code must be at least 2 characters long.")]
        [MaxLength(4, ErrorMessage = "Client Code cannot exceed 4 characters.")]
        public string ClientCode { get; set; } = string.Empty;
        [Required(ErrorMessage = "Groud Code is required")]
        [MinLength(2, ErrorMessage = "Groud Code must be at least 2 characters long.")]
        [MaxLength(4, ErrorMessage = "Groud Code cannot exceed 4 characters.")]
        [Display(Name = "Groud Code")]
        public string GroudCode { get; set; } = string.Empty;
        [Required(ErrorMessage = "Store Code is required")]
        [MinLength(2, ErrorMessage = "Store Code must be at least 2 characters long.")]
        [MaxLength(4, ErrorMessage = "Store Code cannot exceed 4 characters.")]
        [Display(Name = "Store Code")]
        public string StoreCode { get; set; } = string.Empty;

        [Display(Name = "App Operation")]
        [Required(ErrorMessage = "Operation Mode is required")]
        [EnumDataType(typeof(AppOperation), ErrorMessage = "Invalid Operation Mode.")]
        public AppOperation OperationMode { get; set; } = AppOperation.Store;
    }
}
