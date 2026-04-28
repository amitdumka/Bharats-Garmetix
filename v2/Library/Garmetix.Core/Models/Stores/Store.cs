/*
 * Garmetix
 * Author: Amit Kumar
 * https://garmetix.com/
 * Copyright (c) 2025. All rights reserved.
 * Version: 5.0.0
 * License: https://garmetix.com/license
 * Website: https://garmetix.com/
*/
/*
 * Store.cs
 * 
 * This file contains the data models for the Store module of Garmetix. The Store module is responsible for managing the stores and their related information in the application. 
 * The data models defined in this file include Company, StoreGroup, and Store. These models represent the structure of the data that will be stored in the database and used throughout the application. 
 * Each model contains properties that correspond to the fields in the database tables, as well as navigation properties to establish relationships between the models. 
 * The Company model represents a company that owns one or more stores, while the StoreGroup model represents a group of stores that belong to a company. 
 * The Store model represents an individual store that belongs to a company and may be part of a store group. 
 * These models are essential for managing the store-related data in Garmetix and will be used in various parts of the application, such as controllers, services, and views. 
 */

using Garmetix.Core.Enums;
using Garmetix.Core.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Garmetix.Core.Models.Stores
{

    public class Company : BaseEntity
    {
        [Display(Name = "Name")] public string Name { get; set; } = string.Empty;
        [Display(Name = "Start Date")] public DateTime StartDate { get; set; } = DateTime.Now.Date;
        [Display(Name = "End Date")] public DateTime? EndDate { get; set; }
        [Display(Name = "Active")] public bool Active { get; set; } = false;
        [Display(Name = "Contact Number")] public string ContactNumber { get; set; } = string.Empty;
        [Display(Name = "Email")] public string Email { get; set; } = string.Empty;
        [Display(Name = "Address")] public string Address { get; set; } = string.Empty;
        [Display(Name = "City")] public string City { get; set; } = "Dumka";
        [Display(Name = "State")] public string State { get; set; } = "Jharkhand";
        [Display(Name = "Country")] public string Country { get; set; } = "India";
        [Display(Name = "Zip Code")] public string ZipCode { get; set; } = "814101";
        [Display(Name = "GSTIN")] public string GSTIN { get; set; } = string.Empty;
        [Display(Name = "PAN")] public string Pan { get; set; } = string.Empty;
        [Display(Name = "Code")] public string Code { get; set; } = string.Empty;
        [Display(Name = "Store Category")] public StoreCategory StoreCategory { get; set; } = StoreCategory.Retail;
        [Display(Name = "Contact Person")] public string ContactPerson { get; set; } = string.Empty;
        [Display(Name = "Contact Mobile")] public string ContactMobile { get; set; } = string.Empty;
        [Display(Name = "CIN")] public string CIN { get; set; } = string.Empty;
        [Display(Name = "Company Type")] public CompanyType CompanyType { get; set; } = CompanyType.Proprietorship;

    }
    public class StoreGroup : BaseEntity
    {
        [Display(Name = "Name")] public string Name { get; set; } = string.Empty;
        [Display(Name = "Group Code")] public string GroupCode { get; set; } = string.Empty;
        [Display(Name = "Store Category")] public StoreCategory StoreCategory { get; set; } = StoreCategory.Retail;
        [Display(Name = "Start Date")] public DateTime StartDate { get; set; } = DateTime.Now.Date;
        [Display(Name = "End Date")] public DateTime? EndDate { get; set; }
        [Display(Name = "Active")] public bool Active { get; set; } = false;
        [ForeignKey("Company")]
        [Display(Name = "Company", AutoGenerateField = false)] public Guid CompanyId { get; set; }
        [Display(Name = "Company", AutoGenerateField = false)] public virtual Company? Company { get; set; }
    }
    public class Store : BaseEntity
    {

        [Display(Name = "Name")] public string Name { get; set; } = string.Empty;
        [Display(Name = "Start Date")] public DateTime StartDate { get; set; } = DateTime.Now.Date;
        [Display(Name = "End Date")] public DateTime? EndDate { get; set; }
        [Display(Name = "Active")] public bool Active { get; set; } = false;
        [Display(Name = "Store Code")] public string StoreCode { get; set; } = string.Empty;
        [Display(Name = "Store Category")] public StoreCategory StoreCategory { get; set; } = StoreCategory.Retail;
        [Display(Name = "Contact Number")] public string ContactNumber { get; set; } = string.Empty;
        [Display(Name = "Email")] public string Email { get; set; } = string.Empty;
        [Display(Name = "Address")] public string Address { get; set; } = string.Empty;
        [Display(Name = "City")] public string City { get; set; } = "Dumka";
        [Display(Name = "State")] public string State { get; set; } = "Jharkhand";
        [Display(Name = "Country")] public string Country { get; set; } = "India";
        [Display(Name = "Zip Code")] public string ZipCode { get; set; } = "814101";
        [ForeignKey("Company")]
        [Display(Name = "Company", AutoGenerateField = false)]     public Guid CompanyId { get; set; }

        [ForeignKey("StoreGroup")]
        [Display(Name = "Store Group", AutoGenerateField = false)] public Guid StoreGroupId { get; set; }

        [Display(Name = "Company", AutoGenerateField = false)] public virtual Company? Company { get; set; }
        [Display(Name = "Store Group", AutoGenerateField = false)] public virtual StoreGroup? StoreGroup { get; set; }
    }

}
