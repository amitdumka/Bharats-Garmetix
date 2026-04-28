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
 * AppUser.cs
 * 
 * 
 * This file defines the AppUser class, which represents a user in the application. It includes properties for user identification, authentication, and authorization. The class is designed to be used across all platforms supported by the application.
 */


using Garmetix.Core.Enums;
using System.ComponentModel.DataAnnotations;





namespace Garmetix.Core.Models.Authentication
{
    // All the code in this file is included in all platforms.
    public class AppUser
    {
        //Write the Constructor for AppUser class that initializes the Id property with a new Guid and sets the default values for Admin and AppOperation properties.
        public AppUser()
        {
            Id = Guid.NewGuid();
            Admin = false;
            AppOperation = AppOperation.Store;
            // Initialize all properties with default values to avoid null reference exceptions.
            Name = string.Empty;
            UserName = string.Empty;
            Email = string.Empty;
            Password = string.Empty;
            PinHash = string.Empty;
        }


        [Display(Name = "Id")] public Guid Id { get; set; } = Guid.NewGuid();
        [Display(Name = "Name")] public string Name { get; set; }
        [Display(Name = "User Name")] public string UserName { get; set; }
        [Display(Name = "Email")] public string Email { get; set; }
        [Display(Name = "Password")] public string Password { get; set; }
        [Display(Name = "PIN Hash")] public string PinHash { get; set; } = string.Empty; // The 4-digit PIN
        [Display(Name = "Role")] public LoginRole Role { get; set; }
        [Display(Name = "User Type")] public UserType UserType { get; set; }
        [Display(Name = "Remote User Id")] public Guid? RemoteUserId { get; set; }
        [Display(Name = "Employee Id")] public Guid? EmployeeId { get; set; }
        [Display(Name = "Company Id")] public Guid? CompanyId { get; set; }
        [Display(Name = "Store Group Id")] public Guid? StoreGroupId { get; set; }
        [Display(Name = "Store Id")] public Guid? StoreId { get; set; }
        [Display(Name = "Admin")] public bool Admin { get; set; } = false;
        [Display(Name = "App Operation")] public AppOperation AppOperation { get; set; } = AppOperation.Store;
    }
}
