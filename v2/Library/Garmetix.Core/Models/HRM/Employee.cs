/*
 * Garmetix
 * Author: Amit Kumar
 * https://garmetix.com/
 * Copyright (c) 2026. All rights reserved.
 * Version: 6.0.0
 * License: https://garmetix.com/license
 * Website: https://garmetix.com/
*/


using Garmetix.Core.Enums;
using Garmetix.Core.Models.Base;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Garmetix.Models.HRM
{


    public class Employee : StoreBase
    {
        [Display(Name = "Title")] public string? Title { get; set; }

        [MaxLength(50)]
        [Display(Name = "First Name")] public required string FirstName { get; set; }

        [MaxLength(50)]
        [Display(Name = "Last Name")] public required string LastName { get; set; }

        [Display(Name = "Full Name")] public string FullName { get { return Title + " " + FirstName + " " + LastName; } }

        [Display(Name = "Gender")] public Gender Gender { get; set; }
        [Display(Name = "Date of Birth")] public DateTime DateOfBirth { get; set; }

        [Display(Name = "Employee ID")] public int EmpId { get; set; } // Temp Till full migratin is done.

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
        [Display(Name = "PAN Number")]
        public string? PAN { get; set; }

        [Required, MaxLength(10), MinLength(10)]
        [Display(Name = "Aadhar Number")]
        public required string Aadhar { get; set; }

        [MaxLength(100)]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [MaxLength(15), MinLength(10)]
        [Display(Name = "Mobile Number")]
        public required string Mobile { get; set; }

        // Navigation Properties
        [JsonIgnore] public virtual ICollection<SalaryStructure>? SalaryStructures { get; set; }


        [JsonIgnore] public virtual ICollection<Attendance>? Attendances { get; set; }
        [JsonIgnore] public virtual ICollection<SalaryPayment>? SalaryPayments { get; set; }
        [JsonIgnore] public virtual EmployeeDetail? EmployeeDetails { get; set; }
    }

    public class MonthlyAttendance : StoreBase
    {
        [Required]
        [Display(Name = "Employee", AutoGenerateField = false)] public Guid EmployeeId { get; set; }

        [Required]
        [Display(Name = "Date")] public DateTime OnDate { get; set; }


        [JsonIgnore][Display(Name = "Employee", AutoGenerateField = false)] public virtual Employee? Employee { get; set; }

        //Postive
        [Display(Name = "Present")] public int Present { get; set; }

        [Display(Name = "Half Day")] public int HalfDay { get; set; }
        [Display(Name = "Sunday")] public int Sunday { get; set; }
        [Display(Name = "Paid Leave")] public int PaidLeave { get; set; }
        [Display(Name = "Holidays")] public int Holidays { get; set; }

        //Negative
        [Display(Name = "Casual Leave")] public int CasualLeave { get; set; }

        [Display(Name = "Absent")] public int Absent { get; set; }
        [Display(Name = "Weekly Leave")] public int WeeklyLeave { get; set; }
        [Display(Name = "Remarks")] public string? Remarks { get; set; }
        [Display(Name = "No Of Working Days")] public int NoOfWorkingDays { get; set; }
        [Display(Name = "No Of Absent Days")]
        public decimal NoOfAbsentDays
        {
            get
            {
                return (HalfDay * 0.5m) + Absent + CasualLeave;
            }
        }
        [Display(Name = "Day In Months")]
        public int DayInMonths
        { get { return DateTime.DaysInMonth(OnDate.Year, OnDate.Month); } }

        [Display(Name = "Count")]
        public int Count
        { get { return Present + HalfDay + Sunday + PaidLeave + CasualLeave + Absent + WeeklyLeave + Holidays; } }

        [Display(Name = "Billable Days")] public decimal BillableDays => (decimal)((HalfDay / 2.0m) + 0.0m) + Present + Sunday + PaidLeave + Holidays + 0.0m;

        [Display(Name = "Valid")]
        public bool Valid
        { get { return Count == DayInMonths; } }
    }

    public class EmployeeDetail : CEntity
    {
        [ForeignKey("Employee")]
        [Display(Name = "Employee", AutoGenerateField = false)] public Guid EmployeeId { get; set; }

        [Display(Name = "Employee", AutoGenerateField = false)] public virtual Employee? Employee { get; set; }
        [Display(Name = "City")] public string? City { get; set; }

        [MaxLength(60)]
        [Display(Name = "State")]
        public string? State { get; set; }

        [MaxLength(60)]
        [Display(Name = "Country")]
        public string? Country { get; set; }

        [MaxLength(200)]
        [Display(Name = "Street Name")]
        public string? StreetName { get; set; }

        [MaxLength(10)]
        [Display(Name = "Zip Code")]
        public string? ZipCode { get; set; }

        [MaxLength(200)]
        [Display(Name = "Address Line")]
        public string? AddressLine { get; set; }

        [MaxLength(200)][Display(Name = "Father Name")] public string? FatherName { get; set; }
        [MaxLength(200)][Display(Name = "Mother Name")] public string? MotherName { get; set; }
        [MaxLength(200)][Display(Name = "Spouse Name")] public string? SpouseName { get; set; }
        [MaxLength(200)][Display(Name = "Emergency Contact")] public string? EmergencyContact { get; set; }
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
        [Display(Name = "Employee", AutoGenerateField = false)] public Guid EmployeeId { get; set; }

        [Required]
        [Display(Name = "From Date")] public DateTime FromDate { get; set; } = DateTime.Now;

        [Display(Name = "To Date")] public DateTime? ToDate { get; set; } = null;

        [Display(Name = "Is Current")]
        public bool IsCurrent
        { get { return ToDate == null; } }

        [Display(Name = "Employee", AutoGenerateField = false)] public virtual Employee? Employee { get; set; }

        [Required]
        [Display(Name = "Basic Salary")] public decimal BasicSalary { get; set; } = 0;

        [Required]
        [Display(Name = "House Rent Allowance")] public decimal HRA { get; set; } = 0; // House Rent Allowance

        [Required]
        [Display(Name = "Special Allowance")] public decimal SpecialAllowance { get; set; } = 0;

        [Required]
        [Display(Name = "Conveyance Allowance")] public decimal ConveyanceAllowance { get; set; } = 0;

        [Display(Name = "Incentives")] public decimal Incentives { get; set; } = 0;

        // Deductions
        [Required]
        [Display(Name = "Provident Fund")] public decimal ProvidentFund { get; set; }

        [Required]
        [Display(Name = "Gratuity")] public decimal Gratuity { get; set; }

        [Display(Name = "Professional Tax")] public decimal ProfessionalTax { get; set; }
        [Display(Name = "Deductions")] public decimal Deductions { get; set; }

        //Bonus
        [Display(Name = "Yearly Bonus")] public decimal YearlyBonus { get; set; } = 0;
        [JsonIgnore]
        [Display(Name = "Net Salary")]
        public decimal NetSalary
        {
            get
            {
                return BasicSalary + HRA + SpecialAllowance + ConveyanceAllowance + Incentives - (ProvidentFund + Gratuity + Deductions + ProfessionalTax);
            }
        }
        [JsonIgnore]
        [Display(Name = "Gross Salary")]
        public decimal GrossSalary
        {
            get
            {
                return BasicSalary + HRA + SpecialAllowance + ConveyanceAllowance + Incentives;
            }
        }
        [JsonIgnore]

        [Display(Name = "Total Deductions")]
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
        [Display(Name = "Employee ID")] public Guid EmployeeId { get; set; }

        [Display(Name = "Out Time")] public DateTime OutTime { get; set; }
        [Display(Name = "In Time")] public DateTime? InTime { get; set; }

        [Required]
        [Display(Name = "Reason")] public required string Reason { get; set; } = string.Empty;
        [Display(Name = "Employee", AutoGenerateField = false)] public virtual Employee? Employee { get; set; }


        [Display(Name = "Duration", AutoGenerateField = false)]
        public double Duration
        { get { return ((InTime ?? DateTime.Now) - OutTime).TotalMinutes; } }
    }
}