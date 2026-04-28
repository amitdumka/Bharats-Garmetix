using CommunityToolkit.Mvvm.ComponentModel;
using Garmetix.Core.Enums;

namespace Garmetix.Models.Reports
{

    public class AttendanceItem
    {
        public DateTime Date { get; set; }
        public AttendanceStatus Status { get; set; }
        public string EntryTimne { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
    }


    public class BackupFile
    {
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
    }
     
    public enum PrintType
    {
        Invoice, PaymentVoucher,
        ReceiptVocuher, CashPaymentVoucher,
        CashReceiptVocucher, Contra, JV, DebitNote, CreditNote,
        Expenses, Note, Payslip, Ledger, PartyLedger, SalaryPaymentVoucher,
        SalaryReceiptVoucher, AttendanceLedger, MonthlyAttendanceLedger,
    }
    public class PDfFileDetails
    {
        public string FileName { get; set; } = string.Empty;
        public string ModelName { get; set; } = string.Empty;
        public string Naration { get; set; } = string.Empty;
    }

    //Voucher and Receipt Printing
    public partial class VoucherDetails : BaseDetail
    {

        [ObservableProperty]
        private VoucherType _voucher;

        [ObservableProperty]
        private bool _isCashVoucher = false;

        [ObservableProperty]
        private string _transactionType = String.Empty;

        [ObservableProperty]
        private string _voucherType="Payment"; // "Payment" or "Receipt or Expenses"

        [ObservableProperty]
        private string _voucherNumber = string.Empty;

        [ObservableProperty]
        private DateTime _date = DateTime.Today;

        [ObservableProperty]
        private string _payeeOrPayerName=string.Empty;

        [ObservableProperty]
        private string _partyDetails=string.Empty;

        [ObservableProperty]
        private decimal _amount=0.0m;

        [ObservableProperty]
        private string _amountInWords = string.Empty;

        [ObservableProperty]
        private string _narration = string.Empty;

        [ObservableProperty]
        private string _authorizedSignatory = "Manager";

        [ObservableProperty]
        private string _paymentMethod = "Cash";

        [ObservableProperty]
        private string _paymentDetails = "Paid in Cash";
    }
    
    public partial class LedgerInfo
    {
        public DateTime Date { get; set; }
        public string Particulars { get; set; } = string.Empty;
        public string PaymentMode { get; set; } = string.Empty;
        public decimal In { get; set; }
        public decimal Out { get; set; }
        public decimal Balance { get; set; }
    }
    public partial class GeneralLedger
    {
        public string LedgerName { get; set; } = string.Empty;
        public string LegerType { get; set; } = string.Empty; // Renamed to LedgerType for consistency
        public List<LedgerInfo> Ledgers { get; set; } = [];
    }
    //public partial class LedgerInfo
    //{
        //    public DateTime Date { get; set; }
        //    public string Particulars { get; set; }
        //    public string PaymentMode { get; set; }
        //public decimal Debit { get; set; }
       // public decimal Credit { get; set; }
        //    public decimal Balance { get; set; }
    //}
    public partial class PartyLedger // Simplified for this example, you can keep ObservableObject
    {
        public string PartyName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Gstin { get; set; } = string.Empty;
        public string LegerType { get; set; } = string.Empty; // Renamed to LedgerType for consistency
        public List<LedgerInfo> Ledgers { get; set; } = [];
    }
}
