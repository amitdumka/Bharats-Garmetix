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
using Garmetix.Models.Accounting;
using Garmetix.Models.Enums;

namespace Garmetix.Core.DTO.Accounting
{
    public class DueRecoveryDTO : CEntity
    {
        public required string InvoiceNumber { get; set; }
        public DateTime OnDate { get; set; }
        public decimal Amount { get; set; }
        public bool Paid { get; set; } = false;
        public DateTime? ClearingDate { get; set; }
        public Guid? StoreId { get; set; }
        public Guid? StoreGroupId { get; set; }
        public Guid? CompanyId { get; set; }
        public string? StoreName { get; set; }
    }
    public class CustomerDueDto : CEntity
    {
        public required string InvoiceNumber { get; set; }
        public DateTime OnDate { get; set; }
        public decimal Amount { get; set; }
        public bool Paid { get; set; } = false;
        public DateTime? ClearingDate { get; set; }
        public Guid? StoreId { get; set; }
        public Guid? StoreGroupId { get; set; }
        public Guid? CompanyId { get; set; }
        public string? StoreName { get; set; }
    }

    public class PartyDto : CEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? EmailId { get; set; }
        public string? Phone { get; set; }
        public string? GSTIN { get; set; }
        public string? PAN { get; set; }
        public PartyType Category { get; set; }
        public Guid LedgerId { get; set; } = Guid.Empty;
        public string? Ledger { get; set; } = string.Empty;
        public Guid? CompanyId { get; set; }
        public string? CompanyName { get; set; }
    }

    public class LedgerGroupDto : CEntity
    {
        public required string Name { get; set; }
        public LedgerCategory Category { get; set; }
        public string? Remarks { get; set; } = string.Empty;
        public Guid? CompanyId { get; set; }
        public string? CompanyName { get; set; }
    }

    public class LedgerDto : CEntity
    {
        public required string Name { get; set; } = string.Empty;

        public Guid LedgerGroupId { get; set; }
        public string? LedgerGroup { get; set; }
        public LedgerType LedgerType { get; set; }
        public DateTime OpenningDate { get; set; }
        public decimal OpenningBalance { get; set; }
        public bool IsParty { get; set; } = false;
        public Guid? CompanyId { get; set; }
        public string? CompanyName { get; set; }
    }

    public class VoucherBaseDto : CEntity
    {
        public required string VoucherNumber { get; set; }
        public DateTime OnDate { get; set; }

        public VoucherType VoucherType { get; set; } = VoucherType.Payment;

        public required string PartyName { get; set; }
        public required string Particulars { get; set; }

        public decimal Amount { get; set; }
        public string? Remarks { get; set; } = string.Empty;

        public string? SlipNumber { get; set; }

        public bool IsParty { get; set; } = false;

        public Guid? PartyId { get; set; } = Guid.Empty;
        public string? Party { get; set; }
        public Guid? LedgerId { get; set; } = Guid.Empty;
        public string? Ledger { get; set; } = string.Empty;
        public Guid? CompanyId { get; set; }
        public string? CompanyName { get; set; }
    }

    public class TranscationDto : CEntity
    {
        public required string Name { get; set; }
        public Guid? CompanyId { get; set; }
        public string? CompanyName { get; set; }
    }

    public class VoucherDTo : VoucherBaseDto
    {
        public PaymentMode PaymentMode { get; set; } = PaymentMode.Cash;
        public string? PaymentDetails { get; set; }

        public Guid? AccountNumber { get; set; }
        public string? AccountHolderName { get; set; }
    }

    public class CashVoucherDto : VoucherBaseDto
    {
        public Guid TranscationId { get; set; }
        public string? Transcation { get; set; }
    }
}