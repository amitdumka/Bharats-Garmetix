/*
 * Garmetix
 * Author: Amit Kumar
 * https://garmetix.com/
 * Copyright (c) 2025. All rights reserved.
 * Version: 5.0.0
 * License: https://garmetix.com/license
 * Website: https://garmetix.com/
*/

using Garmetix.Models.Bases;
using Garmetix.Models.Stores;

namespace Garmetix.Models.DTO.Stores
{
    public class CompanyDto : CEntity
    {
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; } = DateTime.Now.Date;
        public DateTime? EndDate { get; set; }
        public bool Active { get; set; } = false;
        public string ContactNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = "Dumka";
        public string State { get; set; } = "Jharkhand";
        public string Country { get; set; } = "India";
        public string ZipCode { get; set; } = "814101";
        public string GSTIN { get; set; } = string.Empty;
        public string Pan { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public StoreCategory StoreCategory { get; set; } = StoreCategory.Retail;
        public string ContactPerson { get; set; } = string.Empty;
        public string ContactMobile { get; set; } = string.Empty;
        public string CIN { get; set; } = string.Empty;
        public string CompanyType { get; set; } = "Prop";
    }
    public class StoreGroupDto : CEntity
    {
        public string Name { get; set; } = string.Empty;
        public string GroupCode { get; set; } = string.Empty;
        public StoreCategory StoreCategory { get; set; } = StoreCategory.Retail;
        public DateTime StartDate { get; set; } = DateTime.Now.Date;
        public DateTime? EndDate { get; set; }
        public bool Active { get; set; } = false;

        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string GSTIN { get; set; } = string.Empty;
        public string Pan { get; set; } = string.Empty;
    }
    public class StoreDto : CEntity
    {
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; } = DateTime.Now.Date;
        public DateTime? EndDate { get; set; }
        public bool Active { get; set; } = false;
        public string StoreCode { get; set; } = string.Empty;
        public StoreCategory StoreCategory { get; set; } = StoreCategory.Retail;

        public string ContactNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = "Dumka";
        public string State { get; set; } = "Jharkhand";
        public string Country { get; set; } = "India";
        public string ZipCode { get; set; } = "814101";

        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string GSTIN { get; set; } = string.Empty;
        public string Pan { get; set; } = string.Empty;
        public string CompanyCode { get; set; } = string.Empty;

        public Guid StoreGroupId { get; set; }
        public string StoreGroupName { get; set; } = string.Empty;
        public string GroupCode { get; set; } = string.Empty;
    }
}