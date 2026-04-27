using Garmetix.Core.Models.Base;
using Garmetix.Models.Accounting;
using Garmetix.Models.Enums;
using System.Text.Json.Serialization;

namespace Garmetix.Models.Inventory
{
    public class Salesman : StoreBase
    {
        public string Name { get; set; } = "Manager";
        public Guid? EmployeeId { get; set; }
        public bool Active { get; set; } = true;
    }

    public abstract class BaseInvoice : CompanyBase
    {
        public required string InvoiceNumber { get; set; }
        public DateTime OnDate { get; set; }
        public bool ReturnInvoice { get; set; } = false;
        public decimal MRP { get; set; }

        public decimal BasePrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal RoundOff { get; set; } = 0;
        public decimal BillAmount { get; set; }

        public decimal Quantity { get; set; }
        public decimal ActualQuantity { get; set; } = 0;
        public decimal BilledQuantity { get; set; } = 0;
        public int ItemCount { get; set; }
        public PaymentMode? PaymentMode { get; set; }

        //Handling GST System and Vat System as well

        public decimal? CGSTAmount { get; set; }
        public decimal? SGSTAmount { get; set; }
        public decimal? IGSTAmount { get; set; }
        public bool InterState { get; set; }=false;
        
    }

    public class Customer : CompanyBase
    {
        public required string Name { get; set; }
        public string Address { get; set; } = "Dumka";
        public string City { get; set; } = "Dumka";
        public string ZipCode { get; set; } = "814101";
        public string? State { get; set; }="Jharkhand"; 
        public string Country { get; set; }="India";

        public required string MobileNumber { get; set; }
        public string? Email { get; set; }

        public DateTime? BirthDate { get; set; }
        public DateTime? Aniversary { get; set; }
        public Guid? PartyId { get; set; }
        public bool Registred { get; set; } = false;
        public string? GSTIN { get; set; }
        public decimal Amount { get; set; } = 0;
        public int BillCount { get; set; } = 0;
    }

    public class Vendor : CompanyBase
    {
        public required string Name { get; set; }
        public required string Address { get; set; }
        public required string City { get; set; }
        public string? ZipCode { get; set; }
        public required string MobileNumber { get; set; }
        public string? Email { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime? EndDate { get; set; } = null;

        public string? GSTIN { get; set; }
        public string? Pan { get; set; }
        public string? Tan { get; set; }
        public bool Active { get; set; }
        public Guid? PartyId { get; set; }

        public virtual Party? Party { get; set; }
        public int BillCount { get; set; } = 0;
        public decimal BillAmount { get; set; } = 0;
        public decimal Paid { get; set; } = 0;
        public decimal Balance
        { get { return Math.Round(BillAmount - Paid, 0); } }
    }

    public class PurchaseInvoice : BaseInvoice
    {
        public Guid VendorId { get; set; }
        public string? VendorName { get; set; }
        public string? VendorGSTIN { get; set; }
        public required string InwardNumber { get; set; }
        public DateTime InwardDate { get; set; } = DateTime.Now.AddDays(-1);
        public decimal FrightAmount { get; set; } = 0;
        public virtual Vendor? Vendor { get; set; }
        public DateTime DueDate { get; set; } = DateTime.Today.AddDays(45);
    }

    public class Invoice : BaseInvoice
    {
        public Guid CustomerId { get; set; }
        public Guid SalemanId { get; set; }

        public string? CustomerName { get; set; }
        public required string CustomerMobileNumber { get; set; }
        public string? CustomerGSTIN { get; set; }

        public bool CreditSale { get; set; }
        public bool B2BSale { get; set; } = false;

        public virtual Salesman? Saleman { get; set; }
        public virtual Customer? Customer { get; set; }
        public  virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();

    }

    public class InvoicePayment : CompanyBase
    {
        public Guid InvoiceId { get; set; }
        public DateTime OnDate { get; set; }
        public decimal Amount { get; set; }
        public string? ReferenceNumber { get; set; }

        public PaymentMode PaymentMode { get; set; }
    }

    public class CardPayment : CompanyBase
    {
        public Guid InvoiceId { get; set; }
        public decimal Amount { get; set; }
        public DateTime OnDate { get; set; }
        public int AuthCode { get; set; }
        public int CardNumber { get; set; }
        public CARDType CardType { get; set; }
        public CARD Card { get; set; }
        public string? BankName { get; set; }
    }

    public class VendorPayment : CompanyBase
    {
        public Guid VendorId { get; set; }
        public decimal Amount { get; set; }
        public string? UTRNumber { get; set; }
        public string? ChequeNumber { get; set; }
        public DateTime OnDate { get; set; }

        public Guid InvoiceId { get; set; }
        public virtual Invoice? Invoice { get; set; }
        public virtual Vendor? Vendor { get; set; }
    }

    //TODO: need to make it robust so no need to store in db which can be calculated
    //TODO: add JsonIgnore attribute for those properties
    public class InvoiceItem : CompanyBase
    {
        public Guid InvoiceId { get; set; }
        public Guid ProductId { get; set; }
        public required string Barcode { get; set; }

        public decimal MRP { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal BasePrice { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal TaxAmount { get; set; }

        public decimal Amount { get; set; }

        public TaxType TaxType { get; set; }
        public Guid TaxId { get; set; }
        public virtual Tax? Tax { get; set; }
        public decimal BilledQuantity { get; set; }
        public decimal ActualQuantity { get; set; }

        [JsonIgnore]
        public virtual Product? Product { get; set; }
        [JsonIgnore]
        public virtual Invoice? Invoice { get; set; }
        
        [JsonIgnore]
        public decimal TaxableAmount { get { return Math.Round((MRP-DiscountAmount) / (1 + (TaxPercentage / 100)), 2); } }
        [JsonIgnore]
        public decimal TotalTaxAmount { get { return Math.Round(TaxableAmount * (TaxPercentage / 100), 2); } }

        [JsonIgnore]
        public decimal LineTotal { get { return Math.Round(TaxableAmount + TotalTaxAmount, 2); } }

    }

    public class PurchaseInvoiceItem : InvoiceItem
    {
        public virtual new PurchaseInvoice? Invoice { get; set; }
    }
}