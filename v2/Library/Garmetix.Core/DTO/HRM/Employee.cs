/*
 * Garmetix
 * Author: Amit Kumar
 * https://garmetix.com/
 * Copyright (c) 2025. All rights reserved.
 * Version: 5.0.0
 * License: https://garmetix.com/license
 * Website: https://garmetix.com/
*/

using Garmetix.Core.Models.Base;
using Garmetix.Core.Models.HRM;
using Garmetix.Models.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Garmetix.Core.DTO.HRM
{
    public class EmployeeDTo : CEntity
    {
        public string Title { get; set; } = "Mr.";

        [MaxLength(50)]
        public required string FirstName { get; set; }

        [MaxLength(50)]
        public required string LastName { get; set; }

        [JsonIgnore]
        public string FullName
        { get { return Title + " " + FirstName + " " + LastName; } }

        public Gender Gender { get; set; } // Enum Gender
        public DateTime DateOfBirth { get; set; } = DateTime.Now.AddYears(-18);

        public int EmpId { get; set; } // Temp Till full migratin is done.

        [JsonIgnore]
        [Display(Name = "Employee Name")]
        public string StaffName
        { get { return (FirstName + " " + LastName).Trim(); } }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Joining Date")]
        public DateTime JoiningDate { get; set; } = DateTime.Now;

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Leaving Date")]
        public DateTime? LeavingDate { get; set; } = null;

        [Display(Name = "Working")]
        public bool Working { get; set; } = true;

        [Display(Name = "Job Category")]
        [DefaultValue(0)]
        public EmployeeCategory Category { get; set; }

        [MaxLength(10), MinLength(10)]
        public string? PAN { get; set; }

        [Required, MaxLength(10), MinLength(10)]
        public required string Aadhar { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(15), MinLength(10)]
        public required string Mobile { get; set; }
        public Guid? StoreId { get; set; }
        public string? StoreName { get; set; } = string.Empty;
        public Guid? StoreGroupId { get; set; }
        public string? StoreGroupName { get; set; }
        public Guid? CompanyId { get; set; }
        public string? CompanyName
        {
            get; set;
        }
    }
    public class MonthlyAttendanceDto : CEntity
    {
        [Required]
        public Guid EmployeeId { get; set; }

        [Required]
        public DateTime OnDate { get; set; }

        [JsonIgnore]
        public virtual Employee? Employee { get; set; }

        //Postive
        public int Present { get; set; }

        public int HalfDay { get; set; }
        public int Sunday { get; set; }
        public int PaidLeave { get; set; }
        public int Holidays { get; set; }

        //Negative
        public int CasualLeave { get; set; }

        public int Absent { get; set; }
        public int WeeklyLeave { get; set; }
        public string Remarks { get; set; } = string.Empty;
        public int NoOfWorkingDays { get; set; }

        [JsonIgnore]
        public int DayInMonths
        { get { return DateTime.DaysInMonth(OnDate.Year, OnDate.Month); } }

        [JsonIgnore]
        public int Count
        { get { return Present + HalfDay + Sunday + PaidLeave + CasualLeave + Absent + WeeklyLeave + Holidays; } }

        [JsonIgnore]
        public decimal BillableDays => (HalfDay / 2.0m) + 0.0m + Present + Sunday + PaidLeave + Holidays + 0.0m;
        [JsonIgnore]
        public bool Valid
        { get { return Count == DayInMonths; } }

        public Guid? StoreId { get; set; }
        public string? StoreName { get; set; }
        public Guid? StoreGroupId { get; set; }
        public string? StoreGroupName { get; set; }
        public Guid? CompanyId { get; set; }
        public string? CompanyName
        {
            get; set;
        }
    }
    public class EmployeeDetailDto
    {
        [Key]
        public Guid EmployeeId { get; set; }

        public string Title { get; set; } = "Mr.";

        [MaxLength(50)]
        public required string FirstName { get; set; }

        [MaxLength(50)]
        public required string LastName { get; set; }
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Joining Date")]
        public DateTime JoiningDate { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Leaving Date")]
        public DateTime? LeavingDate { get; set; }

        [Display(Name = "Working")]
        public bool Working { get; set; }

        [Display(Name = "Job Category")]
        [DefaultValue(0)]
        public EmployeeCategory Category { get; set; }

        [MaxLength(10), MinLength(10)]
        public string? PAN { get; set; }

        [Required, MaxLength(10), MinLength(10)]
        public required string Aadhar { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

        public required string City { get; set; }

        [MaxLength(60)]
        public required string State { get; set; }

        [MaxLength(60)]
        public string Country { get; set; } = "India";

        [MaxLength(200)]
        public required string StreetName { get; set; }

        [MaxLength(10)]
        public required string ZipCode { get; set; }

        [MaxLength(200)]
        public required string AddressLine { get; set; }

        public required string FatherName { get; set; }
        public string? MotherName { get; set; } = string.Empty;
        public string? SpouseName { get; set; } = string.Empty;
        public string? EmergencyContact { get; set; } = string.Empty;

        public Guid? StoreId { get; set; }
        public string? StoreName { get; set; }
        public Guid? StoreGroupId { get; set; }
        public string? StoreGroupName { get; set; }
        public Guid? CompanyId { get; set; }
        public string? CompanyName
        {
            get; set;
        }
    }

    public class AttendanceDto : CEntity
    {
        // Foreign Key
        [Required]
        public Guid EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        [Required]
        public DateTime OnDate { get; set; }

        [Required]
        public AttendanceStatus Status { get; set; }

        public TimeSpan? CheckInTime { get; set; } = DateTime.Now.TimeOfDay;
        public TimeSpan? CheckOutTime { get; set; } = null;
        public string? EntryTime { get; set; } = DateTime.Now.TimeOfDay.ToString();

        [MaxLength(100)]
        public string? Remarks { get; set; }

        public Guid? StoreId { get; set; }
        public string? StoreName { get; set; }
        public Guid? StoreGroupId { get; set; }
        public string? StoreGroupName { get; set; }
        public Guid? CompanyId { get; set; }
        public string? CompanyName
        {
            get; set;
        }
    }

    public class SalaryPaymentDto : CEntity
    {
        // Foreign Key
        [Required]
        [Display(Name = "Employee")]
        public Guid EmployeeId { get; set; }
        public string? EmployeeName { get; set; }

        [Display(Name = "Salary/Year(021992)")]
        public int SalaryMonth { get; set; }

        [Required]
        [Display(Name = "Payment Date")]
        public DateTime OnDate { get; set; }

        [Display(Name = "Payment Reason")]
        public SalaryComponent SalaryComponent { get; set; }

        [Required]
        public decimal GrossSalary { get; set; }

        [Required]
        public decimal TotalDeductions { get; set; }

        [Required]
        public decimal NetSalary { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Display(Name = "Payment Mode")]
        public PaymentMode PaymentMode { get; set; }

        [MaxLength(200)]
        public string? Remarks { get; set; }

        public Guid? SalaryPaySlipId { get; set; }
        public string SalaryPaySlipName { get; set; } = string.Empty;

        public Guid? StoreId { get; set; }
        public string? StoreName { get; set; }
        public Guid? StoreGroupId { get; set; }
        public string? StoreGroupName { get; set; }
        public Guid? CompanyId { get; set; }
        public string? CompanyName
        {
            get; set;
        }
    }

    public class SalaryPaySlipDto : CEntity
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        [Required]
        public string MonthYear { get; set; } = DateTime.Now.ToString("MMMM yyyy");

        [Required]
        public DateTime PayPeriodStart { get; set; }

        [Required]
        public DateTime PayPeriodEnd { get; set; }

        // Earnings
        [Required]
        public decimal BasicSalary { get; set; } = 0;

        [Required]
        public decimal HRA { get; set; } = 0;

        [Required]
        public decimal SpecialAllowance { get; set; }

        [Required]
        public decimal ConveyanceAllowance { get; set; }

        [Required]
        public decimal Incentives { get; set; }

        public decimal OtherEarnings { get; set; } = 0;

        // Deductions
        [Required]
        public decimal ProvidentFund { get; set; }

        public decimal Gratuity { get; set; }
        public decimal Deductions { get; set; }

        [Required]
        public decimal ProfessionalTax { get; set; }

        [Required]
        public decimal IncomeTax { get; set; }

        public decimal OtherDeductions { get; set; }

        [JsonIgnore]
        // Total Calculations
        public decimal TotalEarnings { get => BasicSalary + HRA + SpecialAllowance + ConveyanceAllowance + Incentives + OtherEarnings; }
        [JsonIgnore]
        public decimal TotalDeductions { get => ProvidentFund + Gratuity + ProfessionalTax + Deductions + IncomeTax + OtherDeductions; }

        [MaxLength(200)]
        public string Remarks { get; set; }
        public Guid? SalaryPaymentId { get; set; } = null;
        public string? SalaryPaymentDetails { get; set; }

        [JsonIgnore]
        public decimal NetSalary
        {
            get
            {
                return BasicSalary + HRA + SpecialAllowance + ConveyanceAllowance + Incentives + OtherEarnings - (ProvidentFund + Gratuity + Deductions + IncomeTax + ProfessionalTax + OtherDeductions);
            }
        }

        public Guid? ComapyId { get; set; }
        public string CompanyName { get; set; }
    }

    public class SalaryStructure : CEntity
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }

        [Required]
        public DateTime FromDate { get; set; } = DateTime.Now;

        public DateTime? ToDate { get; set; } = null;
        [JsonIgnore]
        public bool IsCurrent { get { return ToDate == null; } }

        [Required]
        public decimal BasicSalary { get; set; } = 0;

        [Required]
        public decimal HRA { get; set; } = 0; // House Rent Allowance

        [Required]
        public decimal SpecialAllowance { get; set; } = 0;

        [Required]
        public decimal ConveyanceAllowance { get; set; } = 0;

        public decimal Incentives { get; set; } = 0;

        // Deductions
        [Required]
        public decimal ProvidentFund { get; set; }

        [Required]
        public decimal Gratuity { get; set; }

        public decimal ProfessionalTax { get; set; }
        public decimal Deductions { get; set; }

        //Bonus
        public decimal YearlyBonus { get; set; } = 0;
        public Guid? CompanyId { get; set; }
        public string CompanyName
        {
            get; set;
        }
        [JsonIgnore]
        public decimal NetSalary
        {
            get
            {
                return BasicSalary + HRA + SpecialAllowance + ConveyanceAllowance + Incentives - (ProvidentFund + Gratuity + Deductions + ProfessionalTax);
            }
        }
        [JsonIgnore]
        public decimal GrossSalary
        {
            get
            {
                return BasicSalary + HRA + SpecialAllowance + ConveyanceAllowance + Incentives;
            }
        }
        [JsonIgnore]
        public decimal TotalDeductions
        {
            get
            {
                return ProvidentFund + Gratuity + Deductions + ProfessionalTax;
            }
        }

        // Methods to calculate components
        //[JsonIgnore]
        public override string ToString()
        {
            if (!string.IsNullOrEmpty(EmployeeName))
            {
                return $"{EmployeeName}'s Net Salary is {NetSalary:C}";
            }
            else
            {
                return $"{EmployeeId}'s Net Salary is {NetSalary:C}";
            }
        }

        //[JsonIgnore]
        public decimal CalculateGrossSalary()
        {
            return BasicSalary + HRA + SpecialAllowance + ConveyanceAllowance + Incentives;
        }
        //[JsonIgonre]
        public decimal CalculateTotalDeductions()
        {
            return ProvidentFund + Gratuity + ProfessionalTax;
        }
        //[JsonIgonre]
        public decimal CalculateNetSalary()
        {
            return CalculateGrossSalary() - CalculateTotalDeductions();
        }
    }
    public class TimeSheet : CEntity
    {
        [Required]
        public Guid EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public DateTime OutTime { get; set; }
        public DateTime? InTime { get; set; }

        [Required]
        public string Reason { get; set; }

        public virtual Employee? Employee { get; set; }

        public double Duration
        { get { return ((InTime ?? DateTime.Now) - OutTime).TotalMinutes; } }

        public Guid? CompanyId { get; set; }
        public string? CompanyName
        {
            get; set;
        }
        public Guid? StoreId { get; set; }
        public string? StoreName
        {
            get; set;
        }
        public Guid? StoreGroupId { get; set; }
        public string? StoreGroupName
        {
            get; set;
        }
    }
}