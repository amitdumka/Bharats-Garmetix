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
using Garmetix.Models.HRM;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Garmetix.Models.Accounting
{
    public enum VoucherType
    {
        Payment,
        Receipt,
        Expense,
    }

    public enum LedgerType
    {
        Assest,
        Cash,
        BankAccount,
        Loan,
        Expenses,
        DirectExpenses,
        IndirectExpenses,
        Income,
        DirectIncome,
        InDirectExpenses,
        Purcahase,
        Sale,
        StockItem,
        Employee,
        CaptialAccount,
    }

    public enum LedgerCategory
    {
        Credit,
        Debit,
        Income,
        Expenses,
        Assets,
        Bank,
        Loan,
        Purchase,
        Sale,
        Vendor,
        Customer,
        UnCategory,
        Employees,
        Stock,
        Debitor,
        Creditor
    }

    public enum PartyType
    {
        Customer,
        Supplier,
        Employee,
        Vendor,
        Debitor,
        Creditor,
        Others,
    }

    public class DueRecovery : StoreBase
    {
        public required string InvoiceNumber { get; set; }
        public DateTime OnDate { get; set; }
        public decimal Amount { get; set; }
        public bool Paid { get; set; } = false;
        public DateTime? ClearingDate { get; set; }
    }
    public class CustomerDue : StoreBase
    {
        public required string InvoiceNumber { get; set; }
        public DateTime OnDate { get; set; }
        public decimal Amount { get; set; }
        public bool Paid { get; set; } = false;
        public DateTime? ClearingDate { get; set; }
    }

    public class Party : CompanyBase
    {
        public required string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? EmailId { get; set; }
        public string? Phone { get; set; }
        public string? GSTIN { get; set; }
        public string? PAN { get; set; }
        public PartyType Category { get; set; }
        public Guid LedgerId { get; set; } = Guid.Empty;
        [JsonIgnore]
        public virtual Ledger? Ledger { get; set; }
    }

    public class LedgerGroup : CompanyBase
    {
        public required string Name { get; set; }
        public LedgerCategory Category { get; set; }
        public string Remarks { get; set; } = string.Empty;
    }

    public class Ledger : CompanyBase
    {
        public required string Name { get; set; } = string.Empty;

        public Guid LedgerGroupId { get; set; }
        [JsonIgnore]
        public virtual LedgerGroup? LedgerGroup { get; set; }
        public LedgerType LedgerType { get; set; }
        public DateTime OpenningDate { get; set; }
        public decimal OpenningBalance { get; set; }
        public bool IsParty { get; set; } = false;
    }

    public class VoucherBase : StoreBase
    {
        public required string VoucherNumber { get; set; }
        public DateTime OnDate { get; set; }

        public VoucherType VoucherType { get; set; } = VoucherType.Payment;

        public required string PartyName { get; set; }
        public required string Particulars { get; set; }

        public decimal Amount { get; set; }
        public string Remarks { get; set; } = string.Empty;

        public string? SlipNumber { get; set; }


        public Guid? LedgerId { get; set; } = Guid.Empty;
        public virtual Ledger? Ledger { get; set; }
        public Guid? EmployeeId { get; set; } = Guid.Empty;
        public virtual Employee? Employee { get; set; }
    }

    public class Transaction : CompanyBase
    {
        public required string Name { get; set; }
    }

    public class Voucher : VoucherBase
    {
        public PaymentMode PaymentMode { get; set; } = PaymentMode.Cash;
        public string? PaymentDetails { get; set; }

        public bool IsParty { get; set; } = false;
        public Guid? PartyId { get; set; } = Guid.Empty;
        public virtual Party? Party { get; set; }

        [ForeignKey("BankAccount")]
        public Guid? AccountNumber { get; set; }

        [ForeignKey("AccountNumber")]
        public virtual BankAccount? BankAccount { get; set; }
    }

    public class CashVoucher : VoucherBase
    {
        public Guid TransactionId { get; set; }
        public virtual Transaction? Transaction { get; set; }
    }


    // Trip Expense Voucher For Travel Management and input expenses for costing to purchase items.
    public enum ExpenseType
    {
        Travel,
        Transport,
        Ticket,
        Food,
        Lodging,
        Entertainment,
        Medical,
        OtherExpenses,
        Fuel,
        Miscellanous,
        Others
    }
    public class TripExpenseVoucher : BaseModel
    {
        public DateTime OnDate { get; set; } = DateTime.Now;
        public string Particulars { get; set; } = string.Empty;
        public ExpenseType ExpenseType { get; set; } = ExpenseType.Travel;
        public decimal Amount { get; set; } = 0;
        public PaymentMode PaymentMode { get; set; } = PaymentMode.Cash;
        public string? PaymentDetails { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public Guid? EmployeeId { get; set; } = Guid.Empty;
        [JsonIgnore]
        [ForeignKey("EmployeeId")]
        public virtual Employee? Employee { get; set; }
        public Guid? LedgerId { get; set; } = Guid.Empty;
        [JsonIgnore]
        [ForeignKey("LedgerId")]
        public virtual Ledger? Ledger { get; set; }
        public string? SlipNumber { get; set; } = string.Empty;
        public Guid? TripId { get; set; } = Guid.Empty;
        [JsonIgnore]
        [ForeignKey("TripId")]
        public virtual TravelTrip? Trip { get; set; }
        public bool Biillable { get; set; } = false;
    }

    public class TravelTrip : BaseModel
    {
        public int TripNumber { get; set; } = 0;
        public string TripName { get; set; } = string.Empty;
        public DateTime FromDate { get; set; } = DateTime.Now;
        public DateTime ToDate { get; set; } = DateTime.Now;
        public string? Remarks { get; set; } = string.Empty;
        public Guid? EmployeeId { get; set; } = Guid.Empty;
        
        [JsonIgnore]
        [ForeignKey("EmployeeId")]
        public virtual Employee? Employee { get; set; }
        
        public TripType TripType { get; set; }= TripType.Travel;
        public string? TripDetails { get; set; }= string.Empty;
        public Status TripStatus { get; set; }= Status.Unknown;
        public bool IsApproved { get; set; } = false;
        public decimal TotalExpense { get; set; }
    }
    public enum TripType
    {
        Purchasing, Travel, Sales, Marketing, Service, Others,Project, Training, Conference, Meeting, Research, Development, Personal
    }
    public enum Status
    {
        Pending,Ongoing,Running,
        Approved,Success,Error,
        Failed, InProgress,Started,Ended,
        Processing, Waiting,
        Rejected, Completed, Cancelled, PartiallyApproved, PartiallyRejected, 
        PartiallyCompleted,Unknown
    }
}