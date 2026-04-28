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
using Garmetix.Core.Models.HRM;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Garmetix.Core.Models.Accounting
{

    public class DueRecovery : StoreBase
    {
        [Display(Name = "Invoice Number")] public required string InvoiceNumber { get; set; }
        [Display(Name = "Date")] public DateTime OnDate { get; set; }
        [Display(Name = "Amount")] public decimal Amount { get; set; }
        [Display(Name = "Paid")] public bool Paid { get; set; } = false;
        [Display(Name = "Clearing Date")] public DateTime? ClearingDate { get; set; }
    }
    public class CustomerDue : StoreBase
    {
        [Display(Name = "Invoice Number")] public required string InvoiceNumber { get; set; }
        [Display(Name = "Date")] public DateTime OnDate { get; set; }
        [Display(Name = "Amount")] public decimal Amount { get; set; }
        [Display(Name = "Paid")] public bool Paid { get; set; } = false;
        [Display(Name = "Clearing Date")] public DateTime? ClearingDate { get; set; }
    }

    [method: SetsRequiredMembers]
    public class Party() : CompanyBase
    {
        [Display(Name = "Name")]
        public required string Name { get; set; } = string.Empty;
        [Display(Name = "Address")] public string? Address { get; set; }
        [Display(Name = "Email ID")] public string? EmailId { get; set; }
        [Display(Name = "Phone")] public string? Phone { get; set; }
        [Display(Name = "GSTIN")] public string? GSTIN { get; set; }
        [Display(Name = "PAN")] public string? PAN { get; set; }
        [Display(Name = "Category")] public PartyType Category { get; set; }
        [Display(Name = "Ledger", AutoGenerateField = false)] public Guid LedgerId { get; set; } = Guid.Empty;
        [JsonIgnore]
        [Display(Name = "Ledger", AutoGenerateField = false)] public virtual Ledger? Ledger { get; set; }
    }

    [method: SetsRequiredMembers]
    public class LedgerGroup() : CompanyBase
    {
        [Display(Name = "Name")] public required string Name { get; set; } = string.Empty;
        [Display(Name = "Category")] public LedgerCategory Category { get; set; }
        [Display(Name = "Remarks")] public string Remarks { get; set; } = string.Empty;
    }

    [method: SetsRequiredMembers]
    public class Ledger() : CompanyBase
    {
        [Display(Name = "Name")] public required string Name { get; set; } = string.Empty;

        [Display(Name = "Ledger Group", AutoGenerateField = false)] public Guid LedgerGroupId { get; set; }
        [JsonIgnore]
        [Display(Name = "Ledger Group", AutoGenerateField = false)] public virtual LedgerGroup? LedgerGroup { get; set; }
        [Display(Name = "Ledger Type")] public LedgerType LedgerType { get; set; }
        [Display(Name = "Opening Date")] public DateTime OpeningDate { get; set; }
        [Display(Name = "Opening Balance")] public decimal OpeningBalance { get; set; }
        [Display(Name = "Party")] public bool IsParty { get; set; } = false;
    }

    [method: SetsRequiredMembers]
    public class VoucherBase() : StoreBase
    {
        [Display(Name = "Voucher Number")] public required string VoucherNumber { get; set; } = string.Empty;
        [Display(Name = "Date")] public DateTime OnDate { get; set; }

        [Display(Name = "Voucher Type")] public VoucherType VoucherType { get; set; } = VoucherType.Payment;

        [Display(Name = "Party Name")] public required string PartyName { get; set; } = string.Empty;
        [Display(Name = "Particulars")] public required string Particulars { get; set; } = string.Empty;

        [Display(Name = "Amount")] public decimal Amount { get; set; }
        [Display(Name = "Remarks")] public string Remarks { get; set; } = string.Empty;

        [Display(Name = "Slip Number")] public string? SlipNumber { get; set; }


        [Display(Name = "Ledger")] public Guid? LedgerId { get; set; } = Guid.Empty;
        [JsonIgnore]
        [Display(Name = "Ledger", AutoGenerateField = false)] public virtual Ledger? Ledger { get; set; }
        [Display(Name = "Employee", AutoGenerateField = false)] public Guid? EmployeeId { get; set; } = Guid.Empty;
        [JsonIgnore]
        [Display(Name = "Employee", AutoGenerateField = false)] public virtual Employee? Employee { get; set; }
    }

    public class Transaction : CompanyBase
    {
        [Display(Name = "Name")] public required string Name { get; set; }
    }

    public class Voucher : VoucherBase
    {
        [SetsRequiredMembers]
        public Voucher()
        {
            VoucherNumber = string.Empty;
            PartyName = string.Empty;
            Particulars = string.Empty;
        }
        [Display(Name = "Payment Mode")] public PaymentMode PaymentMode { get; set; } = PaymentMode.Cash;
        [Display(Name = "Payment Details")] public string? PaymentDetails { get; set; }

        [Display(Name = "Is Party")] public bool IsParty { get; set; } = false;
        [Display(Name = "Party", AutoGenerateField = false)] public Guid? PartyId { get; set; } = Guid.Empty;

        [JsonIgnore]
        [Display(Name = "Party", AutoGenerateField = false)] public virtual Party? Party { get; set; }

        [ForeignKey("BankAccount")]
        [Display(Name = "Account Number", AutoGenerateField = false)] public Guid? AccountNumber { get; set; }

        [ForeignKey("AccountNumber")]
        [Display(Name = "Bank Account", AutoGenerateField = false)] public virtual BankAccount? BankAccount { get; set; }
    }

    public class CashVoucher : VoucherBase
    {
        [SetsRequiredMembers]
        public CashVoucher()
        {
            VoucherNumber = string.Empty;
            PartyName = string.Empty;
            Particulars = string.Empty;
        }
        [Display(Name = "Transaction", AutoGenerateField = false)] public Guid TransactionId { get; set; }
        [JsonIgnore]
        [Display(Name = "Transaction", AutoGenerateField = false)] public virtual Transaction? Transaction { get; set; }
    }


    // Trip Expense Voucher For Travel Management and input expenses for costing to purchase items.

    public class TripExpenseVoucher : BaseEntity
    {
        [Display(Name = "On Date")] public DateTime OnDate { get; set; } = DateTime.Now;
        [Display(Name = "Particulars")] public string Particulars { get; set; } = string.Empty;
        [Display(Name = "Expense Type")] public ExpenseType ExpenseType { get; set; } = ExpenseType.Travel;
        [Display(Name = "Amount")] public decimal Amount { get; set; } = 0;
        [Display(Name = "Payment Mode")] public PaymentMode PaymentMode { get; set; } = PaymentMode.Cash;
        [Display(Name = "Payment Details")] public string? PaymentDetails { get; set; } = string.Empty;
        [Display(Name = "Remarks")] public string Remarks { get; set; } = string.Empty;
        [Display(Name = "Employee", AutoGenerateField = false)] public Guid? EmployeeId { get; set; } = Guid.Empty;
        [JsonIgnore]
        [ForeignKey("EmployeeId")]
        [Display(Name = "Employee", AutoGenerateField = false)] public virtual Employee? Employee { get; set; }
        [Display(Name = "Ledger", AutoGenerateField = false)] public Guid? LedgerId { get; set; } = Guid.Empty;
        [JsonIgnore]
        [ForeignKey("LedgerId")]
        [Display(Name = "Ledger", AutoGenerateField = false)] public virtual Ledger? Ledger { get; set; }
        [Display(Name = "Slip Number")] public string? SlipNumber { get; set; } = string.Empty;
        [Display(Name = "Trip", AutoGenerateField = false)] public Guid? TripId { get; set; } = Guid.Empty;
        [JsonIgnore]
        [ForeignKey("TripId")]
        [Display(Name = "Trip", AutoGenerateField = false)] public virtual TravelTrip? Trip { get; set; }
        [Display(Name = "Billable")] public bool Biillable { get; set; } = false;
    }

    public class TravelTrip : BaseEntity
    {
        [Display(Name = "Trip Number")] public int TripNumber { get; set; } = 0;
        [Display(Name = "Trip Name")] public string TripName { get; set; } = string.Empty;
        [Display(Name = "From Date")] public DateTime FromDate { get; set; } = DateTime.Now;
        [Display(Name = "To Date")] public DateTime ToDate { get; set; } = DateTime.Now;
        [Display(Name = "Remarks")] public string? Remarks { get; set; } = string.Empty;
        [Display(Name = "Employee", AutoGenerateField = false)] public Guid? EmployeeId { get; set; } = Guid.Empty;

        [JsonIgnore]
        [ForeignKey("EmployeeId")]
        [Display(Name = "Employee", AutoGenerateField = false)] public virtual Employee? Employee { get; set; }

        [Display(Name = "Trip Type")] public TripType TripType { get; set; } = TripType.Travel;
        [Display(Name = "Trip Details")] public string? TripDetails { get; set; } = string.Empty;
        [Display(Name = "Trip Status")] public Status TripStatus { get; set; } = Status.Unknown;
        [Display(Name = "Is Approved")] public bool IsApproved { get; set; } = false;
        [Display(Name = "Total Expense")] public decimal TotalExpense { get; set; }
    }

}