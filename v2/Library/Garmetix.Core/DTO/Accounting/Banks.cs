/*
 * Garmetix
 * Author: Amit Kumar
 * https://garmetix.com/
 * Copyright (c) 2025. All rights reserved.
 * Version: 5.0.0
 * License: https://garmetix.com/license
 * Website: https://garmetix.com/
*/

using Garmetix.Core.Enums;
using Garmetix.Core.Models.Base;

namespace Garmetix.Core.DTO.Accounting
{
    public class BankAccountDto : CEntity
    {
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountHolderName { get; set; } = string.Empty;
        public Guid BankId { get; set; }
        public string Bank { get; set; }
        public AccountType AccountType { get; set; } = AccountType.Current;
        public string? Branch { get; set; }
        public string? IFSCode { get; set; }
        public DateTime OpeningDate { get; set; } = DateTime.Now;
        public bool Active { get; set; } = true;
        public DateTime? ClosingDate { get; set; } = null;

        public decimal OpeningBalance { get; set; } = 0;
        public decimal ClosingBalance { get; set; } = 0;
        public Guid LedgerId { get; set; }
        public string? LedgerName { get; set; }
        public Guid? CompanyId { get; set; }
        public string? CompanyName { get; set; }


    }

    public class BankAccountDetailDto : CEntity
    {
        public Guid BankAccountId { get; set; }
        public string? AccountNumber { get; set; }

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
        public Guid? CompanyId { get; set; }
        public string? CompanyName { get; set; }
    }

    public class VendorBankAccountDto : CEntity
    {
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountHolderName { get; set; } = string.Empty;
        public Guid BankId { get; set; }
        public string? Bank { get; set; }
        public AccountType AccountType { get; set; } = AccountType.Current;
        public string? Branch { get; set; }
        public string? IFSCode { get; set; }
        public DateTime OpeningDate { get; set; } = DateTime.Now;
        public bool Active { get; set; } = true;
        public DateTime? ClosingDate { get; set; } = null;

        public decimal OpeningBalance { get; set; } = 0;
        public decimal ClosingBalance { get; set; } = 0;
        public Guid LedgerId { get; set; }
        public string? Ledger { get; set; }
        public Guid? VendorId { get; set; }
        public string? VendorName { get; set; }
        public Guid? CompanyId { get; set; }
        public string? CompanyName { get; set; }
    }

    public class BankAccountListDto : CEntity
    {
        public string AccountNumber { get; set; }
        public string AccountHolderName { get; set; }
        public string? BankName { get; set; }
        public string? Branch { get; set; }
        public string? IFSCode { get; set; }
        public AccountType AccountType { get; set; }
        public Guid? CompanyId { get; set; }
        public string? CompanyName { get; set; }
    }
}