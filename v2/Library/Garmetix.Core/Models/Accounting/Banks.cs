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
using Garmetix.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Garmetix.Core.Models.Accounting
{

    public class Bank : CEntity
    {
        public string Name { get; set; } = string.Empty;
    }

    public class BankAccount : CompanyBase
    {
        [Display(Name = "Account Number")] public string AccountNumber { get; set; } = string.Empty;
        [Display(Name = "Account Holder Name")] public string AccountHolderName { get; set; } = string.Empty;
        [Display(Name = "Bank", AutoGenerateField = false)] public Guid BankId { get; set; }
        [Display(Name = "Bank", AutoGenerateField = false)] public virtual Bank? Bank { get; set; }
        [Display(Name = "Account Type")] public AccountType AccountType { get; set; } = AccountType.Current;
        public string? Branch { get; set; }
        public string? IFSCode { get; set; }
        [Display(Name = "Opening Date")] public DateTime OpeningDate { get; set; } = DateTime.Now;
        [Display(Name = "Active")] public bool Active { get; set; } = true;
        [Display(Name = "Closing Date")] public DateTime? ClosingDate { get; set; } = null;


        [Display(Name = "Opening Balance")] public decimal OpeningBalance { get; set; } = 0;
        [Display(Name = "Closing Balance")] public decimal ClosingBalance { get; set; } = 0;
        [Display(Name = "Ledger", AutoGenerateField = false)] public Guid LedgerId { get; set; }
        [Display(Name = "Ledger", AutoGenerateField = false)] public virtual Ledger? Ledger { get; set; }
    }

    public class BankAccountDetail : CompanyBase
    {
        [Display(Name = "Bank Account", AutoGenerateField = false)] public Guid BankAccountId { get; set; }
        [Display(Name = "Bank Account", AutoGenerateField = false)] public virtual BankAccount? BankAccount { get; set; }

        [Display(Name = "Customer ID")] public string? CustomerId { get; set; }
        [Display(Name = "User Name")] public string? UserName { get; set; }
        [Display(Name = "Password")] public string? Password { get; set; }
        [Display(Name = "Transaction Password")] public string? TranscationPassword { get; set; }
        [Display(Name = "Extra Password")] public string? ExtraPassword { get; set; }

        [Display(Name = "ATM Pin")] public int ATMPin { get; set; }
        [Display(Name = "M Pin")] public int MPin { get; set; }
        [Display(Name = "T Pin")] public int TPIN { get; set; }
        [Display(Name = "E Pin")] public int EPIN { get; set; }
        [Display(Name = "ATM Card")] public string? ATMCard { get; set; }
        [Display(Name = "Expire Date")] public DateTime? ExpireDate { get; set; }
        [Display(Name = "CVV")] public string? CVV { get; set; }
        [Display(Name = "Status")] public string? Status { get; set; }
    }

    public class VendorBankAccount : CompanyBase
    {
        [Display(Name = "Account Number")] public string AccountNumber { get; set; } = string.Empty;
        [Display(Name = "Account Holder Name")] public string AccountHolderName { get; set; } = string.Empty;
        [Display(Name = "Bank", AutoGenerateField = false)] public Guid BankId { get; set; }
        [Display(Name = "Bank", AutoGenerateField = false)] public virtual Bank? Bank { get; set; }
        [Display(Name = "Account Type")] public AccountType AccountType { get; set; } = AccountType.Current;
        [Display(Name = "Branch")] public string? Branch { get; set; }
        [Display(Name = "IFSC Code")] public string? IFSCode { get; set; }
        [Display(Name = "Opening Date")] public DateTime OpeningDate { get; set; } = DateTime.Now;
        [Display(Name = "Active")] public bool Active { get; set; } = true;
        [Display(Name = "Closing Date")] public DateTime? ClosingDate { get; set; } = null;
        [Display(Name = "Opening Balance")] public decimal OpeningBalance { get; set; } = 0;
        [Display(Name = "Closing Balance")] public decimal ClosingBalance { get; set; } = 0;
        [Display(Name = "Ledger", AutoGenerateField = false)] public Guid LedgerId { get; set; }
        [Display(Name = "Ledger", AutoGenerateField = false)] public virtual Ledger? Ledger { get; set; }
        [Display(Name = "Vendor", AutoGenerateField = false)] public Guid? VendorId { get; set; }
    }

    public class BankAccountList : CompanyBase
    {
        [Display(Name = "Account Number")] public string AccountNumber { get; set; } = string.Empty;
        [Display(Name = "Account Holder Name")] public string AccountHolderName { get; set; } = string.Empty;
        [Display(Name = "Bank Name")] public string? BankName { get; set; }
        [Display(Name = "Branch")] public string? Branch { get; set; }
        [Display(Name = "IFSC Code")] public string? IFSCode { get; set; }
        [Display(Name = "Account Type")] public AccountType AccountType { get; set; }
    }

    

    public class BankTransaction : CompanyBase
    {
     [Display(Name = "Bank Account", AutoGenerateField = false)] public Guid BankAccountId { get; set; }
      [Display(Name = "Bank Account", AutoGenerateField = false)] public virtual BankAccount? BankAccount { get; set; }
     [Display(Name = "On Date")] public DateTime OnDate { get; set; } = DateTime.Now;
        [Display(Name = "Transaction Type")] public TransactionType TransactionType { get; set; } = TransactionType.Deposit;
        [Display(Name = "Transaction Mode")] public TransactionMode TransactionMode { get; set; } = TransactionMode.Cash;
        [Display(Name = "Narration")] public string? Narration { get; set; } = string.Empty;
        [Display(Name = "Reference")] public string? Reference { get; set; } = string.Empty;
        [Display(Name = "Amount")] public decimal Amount { get; set; } = decimal.Zero;
        [Display(Name = "Person Name")] public string? PersonName { get; set; } = string.Empty;

    }
    public class ChequeLog : CompanyBase
    {
        [Display(Name = "Bank Account", AutoGenerateField = false)] public Guid BankAccountId { get; set; }
        [Display(Name = "Bank Account", AutoGenerateField = false)] public virtual BankAccount? BankAccount { get; set; }
        [Display(Name = "Cheque Number")] public string ChequeNumber { get; set; } = string.Empty;
        [Display(Name = "On Date")] public DateTime OnDate { get; set; } = DateTime.Now;
        [Display(Name = "Cheque Date")] public DateTime? ChequeDate { get; set; }
        [Display(Name = "Narration")] public string? Narration { get; set; } = string.Empty;
        [Display(Name = "Cheque Bank")] public string? ChequeBank { get; set; } = string.Empty;
        [Display(Name = "Amount")] public decimal Amount { get; set; } = decimal.Zero;
        [Display(Name = "Person Name")] public string? PersonName { get; set; } = string.Empty;
        [Display(Name = "Cheque Number")] public string CheequeNumber { get; set; } = string.Empty;
        [Display(Name = "Status")] public string? Status { get; set; } = string.Empty;
        [Display(Name = "In House")] public bool InHouse { get; set; } = false;
    }
}