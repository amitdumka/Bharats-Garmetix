using Garmetix.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Garmetix.Onboarding.Models
{
    public class CompanyConfigInfo
    {
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
