/*
 * Garmetix
 * Author: Amit Kumar
 * https://garmetix.com/
 * Copyright (c) 2025. All rights reserved.
 * Version: 5.0.0
 * License: https://garmetix.com/license
 * Website: https://garmetix.com/
*/

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Garmetix.Models.Bases
{
    public class PagedResult<T>
    {
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public List<T> Items { get; set; }
    }


    public interface IEntity
    {
        [Key]
        public Guid Id { get; set; }
    }
    public class CEntity : IEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
    }
    public class BaseModel : CEntity
    {
        public bool Synced { get; set; } = false;
        public bool Deleted { get; set; } = false;
    }

    public class CompanyBase : BaseModel
    {
        [ForeignKey("Company")]
        public Guid CompanyId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
    }

    public class GroupBase : BaseModel
    {
        [ForeignKey("Company")]
        public Guid CompanyId { get; set; }
        [ForeignKey("StoreGroup")]
        public Guid StoreGroupId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
    }

    public class StoreBase : BaseModel
    {
        [ForeignKey("Company")]
        public Guid CompanyId { get; set; }
        [ForeignKey("StoreGroup")]
        public Guid StoreGroupId { get; set; }
        [ForeignKey("Store")]
        public Guid StoreId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
    }



}