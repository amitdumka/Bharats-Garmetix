using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garmetix.CoreBase.Accounting.Models
{
    public class BankTransactionEntry : CEntity
    {

        [Display(Name = "Bank Account")]
        [Required(ErrorMessage = "Bank Account is required")]
        public Guid BankAccount { get; set; }
        [Display(Name = "Date"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Date is required")]
        public DateTime OnDate { get; set; } = DateTime.Now;
        [Display(Name = "Transaction Type(Deposit/Withdrawal)")]
        [Required(ErrorMessage = "Transaction Type is required")]
        public TransactionType TransactionType { get; set; } = TransactionType.Deposit;
        [Display(Name = "Transaction Mode(Cash/Online/Etc..)")]
        [Required(ErrorMessage = "Transaction Mode is required")]
        public TransactionMode TransactionMode { get; set; } = TransactionMode.Cash;
        [Display(Name = "Narration"), StringLength(100)]
        [RegularExpression("^[a-zA-Z0-9 ]+$", ErrorMessage = "Only Alphabets and Numbers allowed")]
        public string? Narration { get; set; } = string.Empty;
        [Display(Name = "Reference(UTR,Cheque Number)"), StringLength(100)]
        [RegularExpression("^[a-zA-Z0-9 ]+$", ErrorMessage = "Only Alphabets and Numbers allowed")]
        public string? Reference { get; set; } = string.Empty;
        [Display(Name = "Amount"),DataType(DataType.Currency)]
        [Required(ErrorMessage = "Amount is required")]
        //[Minimum(0.01, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; } = decimal.Zero;
        [Display(Name = "Person Name(Name of Depositor/Payee)"), StringLength(100)]
        public string? PersonName { get; set; } = string.Empty;

        public Guid Company { get; set; }

    }
    public class ChequeLogEntry : CEntity
    {
        [Display(Name = "Bank Account")]
        [Required(ErrorMessage = "Bank Account is required")]
        public Guid BankAccount{ get; set; }
        [Display(Name = "Cheque Number"), StringLength(100)]
        [RegularExpression("^[a-zA-Z0-9 ]+$", ErrorMessage = "Only Alphabets and Numbers allowed")]
        [Required(ErrorMessage = "Cheque Number is required")]
        public string ChequeNumber { get; set; } = string.Empty;
        [Display(Name = "Date"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Date is required")]
        public DateTime OnDate { get; set; } = DateTime.Now;
        [Display(Name = "Cheque/Bank Date"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Cheque/Bank Date is required")]
        public DateTime ChequeDate { get; set; }=DateTime.Now;
        [Display(Name = "Narration")]
        [RegularExpression("^[a-zA-Z0-9 ]+$", ErrorMessage = "Only Alphabets and Numbers allowed")]
        public string? Narration { get; set; } = string.Empty;
        [Display(Name = "Cheque Bank")]
        [RegularExpression("^[a-zA-Z0-9 ]+$", ErrorMessage = "Only Alphabets and Numbers allowed")]

        public string? ChequeBank { get; set; } = string.Empty;
        [Display(Name = "Amount"), DataType(DataType.Currency)]
        [Required(ErrorMessage = "Amount is required")]
        //[Minimum(0.01, ErrorMessage = "Amount must be greater than 0")
        public decimal Amount { get; set; } = decimal.Zero;
        [Display(Name = "Person Name(Name of Depositor/Payee)"), StringLength(100)]
        [RegularExpression("^[a-zA-Z0-9 ]+$", ErrorMessage = "Only Alphabets and Numbers allowed")]
        public string? PersonName { get; set; } = string.Empty;

        [Display(Name = "Cheeque Number"), StringLength(100)]
        [RegularExpression("^[a-zA-Z0-9 ]+$", ErrorMessage = "Only Alphabets and Numbers allowed")]
        [Required(ErrorMessage = "Cheeque Number is required")]
        public string CheequeNumber { get; set; } = string.Empty;
        [Display(Name = "Status")]
        [RegularExpression("^[a-zA-Z0-9 ]+$", ErrorMessage = "Only Alphabets and Numbers allowed")]
        public string? Status { get; set; } = string.Empty;
        [Display(Name = "In-House(Internal/External)")]
        public bool InHouse { get; set; } = false;

        public Guid Company { get; set; }
         
    }
}
