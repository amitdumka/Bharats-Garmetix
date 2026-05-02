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
using Garmetix.Models.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Garmetix.Models.HRM
{
    public enum SalaryComponent
    {
        NetSalary,
        LastPcs,
        WOWBill,
        SundaySalary,
        Incentive,
        Others,
        Advance,
        PaidLeave,
        SickLeave,
        SalaryAdvance,
        Receipts,
    }

    public enum EmployeeCategory
    {
        Salesman,
        StoreManager,
        HouseKeeping,
        Owner,
        Accounts,
        TailorMaster,
        Tailors,
        TailoringAssistance,
        Others,
    }

    public enum AttendanceStatus
    {
        Present,
        Absent,
        HalfDay,
        Sunday,
        Holiday,
        StoreClosed,
        SundayHoliday,
        SickLeave,
        PaidLeave,
        CasualLeave,
        OnLeave,
        Leave,
        WorkFromHome
    }

    public class Employee : StoreBase
    {
        public string? Title { get; set; }

        [MaxLength(50)]
        public required string FirstName { get; set; }

        [MaxLength(50)]
        public required string LastName { get; set; }

        public string FullName
        { get { return Title + " " + FirstName + " " + LastName; } }

        public Gender Gender { get; set; } // Enum Gender
        public DateTime DateOfBirth { get; set; }

        public int EmpId { get; set; } // Temp Till full migratin is done.

        [Display(Name = "Employee Name")]
        public string StaffName
        { get { return (FirstName + " " + LastName).Trim(); } }

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

        [MaxLength(15), MinLength(10)]
        public required string Mobile { get; set; }

        // Navigation Properties
        public virtual ICollection<SalaryStructure>? SalaryStructures { get; set; }

        public virtual ICollection<Attendance>? Attendances { get; set; }
        public virtual ICollection<SalaryPayment>? SalaryPayments { get; set; }
        public virtual EmployeeDetail? EmployeeDetails { get; set; }
    }

    public class MonthlyAttendance : StoreBase
    {
        [Required]
        public Guid EmployeeId { get; set; }

        [Required]
        public DateTime OnDate { get; set; }

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
        public string? Remarks { get; set; }
        public int NoOfWorkingDays { get; set; }
        public decimal NoOfAbsentDays
        {
            get
            {
                return (HalfDay * 0.5m) + Absent + CasualLeave;
            }
        }
        public int DayInMonths
        { get { return DateTime.DaysInMonth(OnDate.Year, OnDate.Month); } }

        public int Count
        { get { return Present + HalfDay + Sunday + PaidLeave + CasualLeave + Absent + WeeklyLeave + Holidays; } }

        public decimal BillableDays => (decimal)((HalfDay / 2.0m) + 0.0m) + Present + Sunday + PaidLeave + Holidays + 0.0m;

        public bool Valid
        { get { return Count == DayInMonths; } }
    }

    public class EmployeeDetail : CEntity
    {
        [ForeignKey("Employee")]
        public Guid EmployeeId { get; set; }

        public virtual Employee? Employee { get; set; }
        public string? City { get; set; }

        [MaxLength(60)]
        public string? State { get; set; }

        [MaxLength(60)]
        public string? Country { get; set; }

        [MaxLength(200)]
        public string? StreetName { get; set; }

        [MaxLength(10)]
        public string? ZipCode { get; set; }

        [MaxLength(200)]
        public string? AddressLine { get; set; }

        public string? FatherName { get; set; }
        public string? MotherName { get; set; }
        public string? SpouseName { get; set; }
        public string? EmergencyContact { get; set; }

    }

    public class Attendance : StoreBase
    {
        // Foreign Key
        [Required]
        public Guid EmployeeId { get; set; }

        [Required]
        public DateTime OnDate { get; set; }

        [Required]
        public AttendanceStatus Status { get; set; }

        public TimeSpan? CheckInTime { get; set; } = DateTime.Now.TimeOfDay;
        public TimeSpan? CheckOutTime { get; set; } = null;
        public string? EntryTime { get; set; } = DateTime.Now.TimeOfDay.ToString();

        [MaxLength(100)]
        public string? Remarks { get; set; }

        public virtual Employee? Employee { get; set; }
    }

    public class SalaryPayment : StoreBase
    {
        // Foreign Key
        [Required]
        [Display(Name = "Employee")]
        public Guid EmployeeId { get; set; }
        public string VoucherNumber { get; set; } = "";

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

        public Employee? Employee { get; set; }

        [ForeignKey("SalaryPaySlip")]
        public Guid? SalaryPaySlipId { get; set; }

        // Navigation Property
        [ForeignKey("SalaryPaySlipId")]
        public virtual SalaryPaySlip? SalaryPaySlip { get; set; }
    }

    public class SalaryPaySlip : CompanyBase
    {
        [ForeignKey("Employee")]
        public Guid EmployeeId { get; set; }

        [Required]
        public string MonthYear { get; set; } = DateTime.Now.AddMonths(-1).ToString("MMMM yyyy");

        [Required]
        public DateTime PayPeriodStart { get; set; }


        public DateTime? PayPeriodEnd { get; set; }

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

        // Total Calculations
        public decimal TotalEarnings { get => BasicSalary + HRA + SpecialAllowance + ConveyanceAllowance + Incentives + OtherEarnings; }

        public decimal TotalDeductions { get => ProvidentFund + Gratuity + ProfessionalTax + Deductions + IncomeTax + OtherDeductions; }

        [MaxLength(200)]
        public string? Remarks { get; set; }

        public decimal NetSalary
        {
            get
            {
                return BasicSalary + HRA + SpecialAllowance + ConveyanceAllowance + Incentives + OtherEarnings - (ProvidentFund + Gratuity + Deductions + IncomeTax + ProfessionalTax + OtherDeductions);
            }
        }
    }

    public class SalaryStructure : CompanyBase
    {
        [ForeignKey("Employee")]
        public Guid EmployeeId { get; set; }

        [Required]
        public DateTime FromDate { get; set; } = DateTime.Now;

        public DateTime? ToDate { get; set; } = null;

        public bool IsCurrent
        { get { return ToDate == null; } }

        public virtual Employee? Employee { get; set; }

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

        public override string ToString()
        {
            if (Employee != null)
            {
                return $"{Employee.StaffName}'s Net Salary is {NetSalary:C}";
            }
            else
            {
                return $"{EmployeeId}'s Net Salary is {NetSalary:C}";
            }
        }

        public decimal CalculateGrossSalary()
        {
            return BasicSalary + HRA + SpecialAllowance + ConveyanceAllowance + Incentives;
        }

        public decimal CalculateTotalDeductions()
        {
            return ProvidentFund + Gratuity + ProfessionalTax;
        }

        public decimal CalculateNetSalary()
        {
            return CalculateGrossSalary() - CalculateTotalDeductions();
        }
    }

    public class TimeSheet : StoreBase
    {
        [Required]
        public Guid EmployeeId { get; set; }

        public DateTime OutTime { get; set; }
        public DateTime? InTime { get; set; }

        [Required]
        public required string Reason { get; set; }

        public virtual Employee? Employee { get; set; }

        public double Duration
        { get { return ((InTime ?? DateTime.Now) - OutTime).TotalMinutes; } }
    }
}