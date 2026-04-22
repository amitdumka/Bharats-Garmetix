using SQLite;
using System;


//(Ensure you add await _database.CreateTableAsync<ChequeLog>(); and await _database.CreateTableAsync<BankTransaction>(); to your DatabaseHelper.cs)



namespace Garmetix.AI.Billing.Models
{

   public enum TransactionType { Deposit, Withdrawal }
    public enum TransactionMode { Cash, Cheque, OnlineTransfer, UPI, NEFT, RTGS, IMPS, Other , DD,}
    public class BankTransaction
    {
        [PrimaryKey]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Indexed]
        public Guid BankAccountId { get; set; }

        public DateTime OnDate { get; set; } = DateTime.Now;
        public TransactionType TransactionType { get; set; } = TransactionType.Deposit;
        public TransactionMode TransactionMode { get; set; } = TransactionMode.Cash;

        public string Narration { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty; // UTR, IMPS Ref, or Cheque No
        public decimal Amount { get; set; } = decimal.Zero;
        public string PersonName { get; set; } = string.Empty;

        // --- RECONCILIATION DATA ---
        public bool IsReconciled { get; set; } = false;
        public DateTime? ReconciledDate { get; set; }
    }

    public enum ChequeStatus { Pending, Cleared, Bounced, Cancelled }
    public enum ChequeType { Issued, Received }

    public class ChequeLog
    {
        [PrimaryKey]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Indexed]
        public Guid BankTransactionId { get; set; }

        public ChequeType Type { get; set; }
        public string ChequeNumber { get; set; }
        public string DraweeBankName { get; set; } // The bank printed on the cheque
        public DateTime ChequeDate { get; set; }
        public decimal Amount { get; set; }
        public string PartyName { get; set; }

        public ChequeStatus Status { get; set; } = ChequeStatus.Pending;
        public DateTime? ClearanceDate { get; set; }
    }
}