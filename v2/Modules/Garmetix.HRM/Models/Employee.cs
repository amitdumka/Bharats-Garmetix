using Garmetix.Models.HRM;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Garmetix.CoreBase.HRM.Models
{
    public class EmployeeEntry : CEntity
    {
        [Required, Display(Name = "Title")]
        [DefaultValue("Mr.")]
        [StringLength(10, ErrorMessage = "Title cannot exceed 10 characters.")]
        [RegularExpression(@"^[a-zA-Z.\s]+$", ErrorMessage = "Title can only contain letters and spaces.")]
        public string Title { get; set; } = "Mr.";

        [Required, Display(Name = "First Name")]
        [StringLength(50, ErrorMessage = "First Name cannot exceed 50 characters."), MinLength(3, ErrorMessage = "Minimum 3 letter required")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "First Name can only contain letters and spaces.")]
        [DataType(DataType.Text)]
        public string FirstName { get; set; } = string.Empty;

        [Required, Display(Name = "Last Name")]
        [StringLength(50, ErrorMessage = "Last Name cannot exceed 50 characters."), MinLength(3, ErrorMessage = "Minimum 3 letter required")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Last Name can only contain letters and spaces.")]
        [DataType(DataType.Text)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Gender")]
        [DefaultValue(Gender.Male)]
        [EnumDataType(typeof(Gender))]
        public Gender Gender { get; set; } = Gender.Male;

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date of Birth")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Date of Birth is required.")]
        public DateTime DateOfBirth { get; set; } = DateTime.Now.AddYears(-18);

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Joining Date")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Joining Date is required.")]
        public DateTime JoiningDate { get; set; } = DateTime.Today;

        [Display(Name = "Department/Category")]
        [DefaultValue(0)]
        [EnumDataType(typeof(EmployeeCategory))]
        [Required(ErrorMessage = "Employee category is required.")]
        public EmployeeCategory Category { get; set; } = EmployeeCategory.Salesman;

        [Display(Name = "Pan Number")]
        [RegularExpression(@"^[A-Z]{5}[0-9]{4}[A-Z]{1}$", ErrorMessage = "Invalid Pan Number")]
        [StringLength(10, ErrorMessage = "Pan Number must be exactly 10 characters.", MinimumLength = 10)]
        public string? Pan { get; set; } = string.Empty;

        [Display(Name = "Aadhar Number")]
        [StringLength(12, ErrorMessage = "Aadhar Number must be exactly 12 characters.", MinimumLength = 12)]
        [RegularExpression(@"^[0-9]{12}$", ErrorMessage = "Invalid Aadhar Number")]
        [DataType(DataType.Text)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Aadhar Number is required.")]
        public string Aadhar { get; set; } = string.Empty;

        [Phone]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Mobile Number")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Mobile Number can only contain numbers.")]
        [StringLength(15, ErrorMessage = "Mobile Number cannot exceed 15 characters.")]
        [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "Enter Mobile Number")]
        [Required(ErrorMessage = "Mobile Number is required."), MinLength(10), MaxLength(15)]
        public string Mobile { get; set; } = string.Empty;

        [MaxLength(100)]
        [EmailAddress]
        [Display(Name = "Email")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email format.")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Email is required.")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Address Line")]
        [MaxLength(100, ErrorMessage = "Address Line cannot exceed 100 characters.")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Address Line is required.")]
        public string AddressLine { get; set; } = "Dumka";

        [Display(Name = "Street Name"), MaxLength(50, ErrorMessage = "Street Name cannot exceed 50 characters.")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Street Name is required.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Street Name can only contain letters and spaces.")]
        [DataType(DataType.Text)]
        public string StreetName { get; set; } = string.Empty;

        [Display(Name = "City"), MaxLength(50, ErrorMessage = "City cannot exceed 50 characters.")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "City is required.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "City can only contain letters and spaces.")]
        [DataType(DataType.Text)]
        public string City { get; set; } = "Dumka";

        [Display(Name = "State"), MaxLength(50, ErrorMessage = "State cannot exceed 50 characters.")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "State is required.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "State can only contain letters and spaces.")]
        [DataType(DataType.Text)]
        public string State { get; set; } = "Jharkhand";

        [Display(Name = "Country"), MaxLength(50, ErrorMessage = "Country cannot exceed 50 characters.")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Country is required.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Country can only contain letters and spaces.")]
        [DataType(DataType.Text)]
        public string Country { get; set; } = "India";

        [Display(Name = "Zip Code"), MaxLength(10, ErrorMessage = "Zip Code cannot exceed 10 characters.")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Zip Code is required.")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Zip Code can only contain numbers.")]
        [DataType(DataType.Text)]
        public string ZipCode { get; set; } = "814101";

        [Display(Name = "Company")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Company is required.")]
        public Guid Company { get; set; }

        [Display(Name = "Division")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Division is required.")]
        public Guid StoreGroup { get; set; }

        [Display(Name = "Store")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Store is required.")]
        public Guid Store { get; set; }
    }
}