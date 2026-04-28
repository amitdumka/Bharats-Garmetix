using Garmetix.Core.Enums;
using Garmetix.Core.Models.Base;
using Syncfusion.Maui.DataForm;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Garmetix.CoreBase.Stores.Models
{

    public class StoreGroupEntry : CEntity
    {
        public string Name { get; set; } = string.Empty;
        public string GroupCode { get; set; } = string.Empty;
        public StoreCategory StoreCategory { get; set; } = StoreCategory.Retail;
        public DateTime StartDate { get; set; } = DateTime.Now.Date;
        public DateTime? EndDate { get; set; }
        public bool Active { get; set; } = false;

        public Guid Company { get; set; }

    }
    public class CompanyEntry : CEntity
    {

        [Required]
        public string Name { get; set; } = string.Empty;

        [Display(GroupName = "Date")]
        [DataFormDisplayOptions(RowOrder = 0, ItemsOrderInRow = 0)]
        public DateTime StartDate { get; set; } = DateTime.Now.Date;

        [Display(GroupName = "Date")]
        [DataFormDisplayOptions(RowOrder = 0, ItemsOrderInRow = 0)]
        public DateTime? EndDate { get; set; }

        public bool Active { get; set; } = false;

        [Display(GroupName = "Contact", ShortName = "Phone Number", Prompt = "Enter Phone Number")]
        [DataType(DataType.PhoneNumber)]
        [DataFormDisplayOptions(RowOrder = 0, ItemsOrderInRow = 0)]
        public string ContactNumber { get; set; } = string.Empty;

        [Display(GroupName = "Contact")]
        [DataFormDisplayOptions(RowOrder = 0, ItemsOrderInRow = 1)]
        public string Email { get; set; } = string.Empty;

        [DataType(DataType.MultilineText)]
        [Display(GroupName = "Address")]
        [DataFormDisplayOptions(RowOrder = 0, ItemsOrderInRow = 0)]
        public string Address { get; set; } = string.Empty;

        [Display(GroupName = "Address")]
        [DataFormDisplayOptions(RowOrder = 0, ItemsOrderInRow = 1)]
        public string City { get; set; } = "Dumka";

        [Display(GroupName = "Address")]
        [DataFormDisplayOptions(RowOrder = 1, ItemsOrderInRow = 0)]
        public string State { get; set; } = "Jharkhand";

        [Display(GroupName = "Address")]
        [DataFormDisplayOptions(RowOrder = 1, ItemsOrderInRow = 1)]
        public string Country { get; set; } = "India";

        [Display(GroupName = "Address")]
        [DataFormDisplayOptions(RowOrder = 2, ItemsOrderInRow = 0)]
        public string ZipCode { get; set; } = "814101";

        [Display(GroupName = "Tax Info")]
        [DataFormDisplayOptions(RowOrder = 0, ItemsOrderInRow = 0)]
        public string GSTIN { get; set; } = string.Empty;

        [Display(GroupName = "Tax Info")]
        [DataFormDisplayOptions(RowOrder = 0, ItemsOrderInRow = 1)]
        public string Pan { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public StoreCategory StoreCategory { get; set; } = StoreCategory.Retail;

        [Display(GroupName = "Company Details")]
        [DataFormDisplayOptions(RowOrder = 0, ItemsOrderInRow = 0)]
        public string ContactPerson { get; set; } = string.Empty;

        [Display(GroupName = "Company Details")]
        [DataFormDisplayOptions(RowOrder = 0, ItemsOrderInRow = 1)]
        public string ContactMobile { get; set; } = string.Empty;

        [Display(GroupName = "Company Details")]
        [DataFormDisplayOptions(RowOrder = 1, ItemsOrderInRow = 0)]
        public string CIN { get; set; } = string.Empty;

        [Display(GroupName = "Company Details")]
        [DataFormDisplayOptions(RowOrder = 1, ItemsOrderInRow = 1)]
        public CompanyType CompanyType { get; set; } = CompanyType.Proprietorship;
    }
    public class StoreEntry : CEntity
    {

        [Display(Name = "Store Name")]
        [Required(ErrorMessage = "Store name is required.")]
        [MinLength(2, ErrorMessage = "Store name must be at least 2 characters.")]
        [MaxLength(50, ErrorMessage = "Store name must be at most 50 characters.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Store name can only contain letters.")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Start Date")]
        [Required(ErrorMessage = "Start Date is requried")]
        public DateTime StartDate { get; set; } = DateTime.Now.Date;

        public DateTime? EndDate { get; set; }

        [Required]
        public string StoreCode { get; set; } = string.Empty;
        public bool Active { get; set; } = false;

        [Display(GroupName = "Contact", ShortName = "Phone Number", Prompt = "Enter Phone Number")]
        [DataType(DataType.PhoneNumber)]
        [DataFormDisplayOptions(RowOrder = 0, ItemsOrderInRow = 0)]
        public string ContactNumber { get; set; } = string.Empty;

        [Display(GroupName = "Contact")]
        [DataFormDisplayOptions(RowOrder = 0, ItemsOrderInRow = 1)]
        public string Email { get; set; } = string.Empty;

        [DataType(DataType.MultilineText)]
        [Display(GroupName = "Address")]
        [DataFormDisplayOptions(RowOrder = 0, ItemsOrderInRow = 0)]
        public string Address { get; set; } = string.Empty;

        [Display(GroupName = "Address")]
        [DataFormDisplayOptions(RowOrder = 0, ItemsOrderInRow = 1)]
        public string City { get; set; } = "Dumka";

        [Display(GroupName = "Address")]
        [DataFormDisplayOptions(RowOrder = 1, ItemsOrderInRow = 0)]
        public string State { get; set; } = "Jharkhand";

        [Display(GroupName = "Address")]
        [DataFormDisplayOptions(RowOrder = 1, ItemsOrderInRow = 1)]
        public string Country { get; set; } = "India";

        [Display(GroupName = "Address")]
        [DataFormDisplayOptions(RowOrder = 2, ItemsOrderInRow = 0)]
        public string ZipCode { get; set; } = "814101";


        public StoreCategory StoreCategory { get; set; } = StoreCategory.Retail;

        //[Display(GroupName = "Company")]
        //[DataFormDisplayOptions(RowOrder = 0, ItemsOrderInRow = 0)]
        [ForeignKey("Company")]
        public Guid Company { get; set; }

        //[Display(GroupName = "Company")]
        //[DataFormDisplayOptions(RowOrder = 0, ItemsOrderInRow = 1)]
        [ForeignKey("StoreGroup")]
        public Guid StoreGroup { get; set; }
    }
}
