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

namespace Garmetix.Models.Accounting
{
    public enum AccountType
    {
        Saving,
        Current,
        CashCredit,
        OverDraft,
        Others,
        Loan,
        CF,
    }
    public class Bank : CEntity
    {
        public string Name { get; set; } = string.Empty;
    }

    public class BankAccount : CompanyBase
    {
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountHolderName { get; set; } = string.Empty;
        public Guid BankId { get; set; }
        public virtual Bank? Bank { get; set; }
        public AccountType AccountType { get; set; } = AccountType.Current;
        public string? Branch { get; set; }
        public string? IFSCode { get; set; }
        public DateTime OpeningDate { get; set; } = DateTime.Now;
        public bool Active { get; set; } = true;
        public DateTime? ClosingDate { get; set; } = null;


        public decimal OpeningBalance { get; set; } = 0;
        public decimal ClosingBalance { get; set; } = 0;
        public Guid LedgerId { get; set; }
        public virtual Ledger? Ledger { get; set; }
    }

    public class BankAccountDetail : CompanyBase
    {
        public Guid BankAccountId { get; set; }
        public virtual BankAccount? BankAccount { get; set; }

        public string? CustomerId { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? TranscationPassword { get; set; }
        public string? ExtraPassword { get; set; }

        public int ATMPin { get; set; }
        public int MPin { get; set; }
        public int TPIN { get; set; }
        public int EPIN { get; set; }
        public string? ATMCard { get; set; }
        public DateTime? ExpireDate { get; set; }
        public string? CVV { get; set; }
        public string? Status { get; set; }
    }

    public class VendorBankAccount : CompanyBase
    {
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountHolderName { get; set; } = string.Empty;
        public Guid BankId { get; set; }
        public virtual Bank? Bank { get; set; }
        public AccountType AccountType { get; set; } = AccountType.Current;
        public string? Branch { get; set; }
        public string? IFSCode { get; set; }
        public DateTime OpeningDate { get; set; } = DateTime.Now;
        public bool Active { get; set; } = true;
        public DateTime? ClosingDate { get; set; } = null;

        public decimal OpeningBalance { get; set; } = 0;
        public decimal ClosingBalance { get; set; } = 0;
        public Guid LedgerId { get; set; }
        public virtual Ledger? Ledger { get; set; }
        public Guid? VendorId { get; set; }
    }

    public class BankAccountList : CompanyBase
    {
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountHolderName { get; set; } = string.Empty;
        public string? BankName { get; set; }
        public string? Branch { get; set; }
        public string? IFSCode { get; set; }
        public AccountType AccountType { get; set; }
    }

    public enum TransactionType{
        Deposit,
        Withdraw
    }
    public enum TransactionMode{
        Cash,
        Cheque,
        NEFT,
        RTGS, UPI,NetBanking, IMPS, DD,
        ATM,Swipe,
        Other
    }

    public class BankTransaction : CompanyBase
    {
        public Guid BankAccountId { get; set; }
        public virtual BankAccount? BankAccount { get; set; }
        public DateTime OnDate { get; set; } = DateTime.Now;
        public TransactionType TransactionType { get; set; } = TransactionType.Deposit;
        public TransactionMode TransactionMode { get; set; }= TransactionMode.Cash;
        public string ? Narration { get; set; }= string.Empty;
        public string? Reference { get; set; }= string.Empty;
        public decimal Amount { get; set; }= decimal.Zero;
        public string? PersonName { get; set; }= string.Empty;

    }
    public class ChequeLog : CompanyBase
    {
        public Guid BankAccountId { get; set; }
        public virtual BankAccount? BankAccount { get; set; }
        public string ChequeNumber { get; set; } = string.Empty;
        public DateTime OnDate { get; set; } = DateTime.Now;
        public DateTime? ChequeDate { get; set; }
        public string ? Narration { get; set; }= string.Empty;
        public string? ChequeBank { get; set; }= string.Empty;
        public decimal Amount { get; set; }= decimal.Zero;
        public string? PersonName { get; set; }= string.Empty;
        public string CheequeNumber { get; set; }= string.Empty;
        public string? Status { get; set; } = string.Empty;
        public bool InHouse { get; set; }= false;
    }
}