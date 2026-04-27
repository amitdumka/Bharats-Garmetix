/*
 * Garmetix
 * Author: Amit Kumar
 * https://garmetix.com/
 * Copyright (c) 2026. All rights reserved.
 * Version: 6.0.0
 * License: https://garmetix.com/license
 * Website: https://garmetix.com/
*/

using Garmetix.Core.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Garmetix.Core.Models.Base
{
    // --- BASES ---
    public abstract class BaseEntity : IEntity
    {
        [Key][Display(AutoGenerateField = false)] public Guid Id { get; set; } = Guid.NewGuid();
        [Display(AutoGenerateField = false)] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Display(AutoGenerateField = false)] public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
        //[Display(AutoGenerateField = false)] public string? CreatedBy { get; set; }
        [Display(AutoGenerateField = false)] public bool Synced { get; set; } = false;
        [Display(AutoGenerateField = false)] public bool Deleted { get; set; } = false;
    }


    public class CEntity : IEntity
    {
        [Key]
        [Display(AutoGenerateField = false)]
        public Guid Id { get; set; } = Guid.NewGuid();
    }
    [Obsolete("Use BaseEntity instead of BaseModel for better performance and simplicity.")]
    public class BaseModel : CEntity
    {
        [Display(AutoGenerateField = false)] public bool Synced { get; set; } = false;
        [Display(AutoGenerateField = false)] public bool Deleted { get; set; } = false;

    }

    /// <summary>
    /// Company Base class representing the foundational properties for a company entity. This class includes a unique identifier for the company,
    /// </summary>
    public class CompanyBase : BaseEntity
    {
        [ForeignKey("Company")]
        [Display(AutoGenerateField = false)] public Guid CompanyId { get; set; }
        [Display(AutoGenerateField = false)] public string? CreatedBy { get; set; }
        [NotMapped] public string? CompanyName { get; set; } = string.Empty;
    }


    /// <summary>
    /// Group Base class representing the foundational properties for a group entity. This class includes a unique identifier for the group,
    /// </summary>
    public class GroupBase : CompanyBase
    {
        [ForeignKey("StoreGroup")]
        [Display(AutoGenerateField = false)] public Guid StoreGroupId { get; set; }
        [NotMapped] public string? StoreGroupName { get; set; } = string.Empty;
    }

    public class StoreBase : GroupBase
    {
        [ForeignKey("Store")]
        [Display(AutoGenerateField = false)] public Guid StoreId { get; set; }
        [NotMapped] public string? StoreName { get; set; } = string.Empty;
    }



}