

using Garmetix.Core.Models.Base;
using Garmetix.Models.DayOperations;

namespace Garmetix.CoreBase.DayOperations.Models
{

    public class DayBeginEntry:CEntity
    {
        public Guid Store{ get; set; }
        public DateTime OnDate { get; set; }
        public decimal OpeningBalance { get; set; }
        public CashDetailEntry CashDetail { get; set; }=new CashDetailEntry();
    }
    public class DayEndEntry:CEntity
    {
        public Guid Store { get; set; }
        public DateTime OnDate { get; set; }
        public decimal ClosingBalance { get; set; }
        public CashDetailEntry CashDetail { get; set; }=new CashDetailEntry();
        public PettyCashSheetEntry PettyCashSheet { get; set; }=new PettyCashSheetEntry();
    }

    public class PettyCashSheetEntry : CEntity
    {
        public Guid Store { get; set; }
        public DateTime OnDate { get; set; }
        public decimal OpeningBalance { get; set; } = 0;
        public decimal Sales { get; set; } = 0;
        public decimal Receipts { get; set; } = 0;
        public decimal DueReceipts { get; set; } = 0;
        public decimal BankWithdrawal { get; set; } = 0;

        public decimal Expenses { get; set; } = 0;
        public decimal Payments { get; set; } = 0;
        public decimal CustomerDue { get; set; } = 0;
        public decimal BankDeposit { get; set; } = 0;
        public decimal NonCashSale { get; set; } = 0;
        public decimal CashInHand { get; set; } = 0;

    }

     


    public class CashDetailEntry : CEntity
    {
        public Guid Store { get; set; }
        public DateTime OnDate { get; set; }
        public decimal Amount { get; set; }
        public int N2000 { get; set; } = 0;
        public int N500 { get; set; } = 0;
        public int N200 { get; set; } = 0;
        public int N100 { get; set; } = 0;
        public int N50 { get; set; } = 0;
        public int NC20 { get; set; } = 0;
        public int NC10 { get; set; } = 0;
        public int NC5 { get; set; } = 0;
        public int NC2 { get; set; } = 0;
        public int NC1 { get; set; } = 0;
    }
}
