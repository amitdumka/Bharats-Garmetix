using Bharat.Base;
using PointOfSale.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MaxLengthAttribute = System.ComponentModel.DataAnnotations.MaxLengthAttribute;

namespace Bharat.Base
{
    // Base model with SQLite primary key
    public class BaseModel
    {
        [PrimaryKey]
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}

namespace Bharat.Inventory
{
    public enum ProductCategory { Fabric, Readymade, InnerWear, WinterWear, EthinicWear, Accessiory, Bag, GiftItem, MiscItem, Others }
    public enum ProductSubCategory { Others, Shirting, Suiting, SuitLength, Suits, Coat, Blazers, Sherwani, Jhodpuri, Kurta, Pajama, KurtaPajama, JuttiNagra, Pagdi, Duppata, PagdiDuppta, Broch, Tie, PocketSquare, Thali, Mala, Sword, Bundi, KotiSet, Trousers, Shirt, TShit, Jeans, CottonPant, Short, Brief, Vest, Jackets }

    [Table("Products")]
    public class Product : BaseModel
    {
        [MaxLength(200)]
        [Required]
        [FormField("Product Name", order: 1, controlType: "Entry", IsRequired = true)]
        public string Name { get; set; } = string.Empty;

        [FormField("Description", order: 2, controlType: "Editor")]
        public string Description { get; set; } = string.Empty;

        [MaxLength(50)]
        [FormField("HSN Code", order: 3)]
        public string HSNCode { get; set; } = string.Empty;

        [MaxLength(100)]
        [Indexed]
        [FormField("Barcode", order: 4)]
        public string Barcode { get; set; } = string.Empty;

        [DataType(DataType.Currency)]
        [FormField("MRP", order: 5, controlType: "Numeric")]
        public decimal MRP { get; set; } = decimal.Zero;

        [FormField("Unit", order: 6, controlType: "Picker")]
        public PointOfSale.Models.Unit Unit { get; set; } = PointOfSale.Models.Unit.Pcs;

        [DataType(DataType.Currency)]
        [FormField("Unit Price", order: 7, controlType: "Numeric")]
        public decimal UnitPrice { get; set; } = decimal.Zero;

        [FormField("Category", order: 8, controlType: "Picker")]
        public ProductCategory ProductCategory { get; set; } = ProductCategory.Readymade;

        [FormField("Sub Category", order: 9, controlType: "Picker")]
        public ProductSubCategory ProductSubCategory { get; set; } = ProductSubCategory.Others;
    }

    [Table("Stocks")]
    public class Stock : BaseModel
    {
        [Indexed]
        [FormField("Product Id", order: 1, controlType: "Picker")]
        public Guid ProductId { get; set; }

        [FormField("Barcode", order: 2)]
        public string Barcode { get; set; } = string.Empty;

        [FormField("Purchase Qty", order: 3, controlType: "Numeric")]
        public decimal PurchaseQty { get; set; } = decimal.Zero;

        [FormField("Cost Price", order: 4, controlType: "Numeric")]
        public decimal CostPrice { get; set; } = decimal.Zero;

        [FormField("Sold Qty", order: 5, controlType: "Numeric")]
        public decimal SoldQty { get; set; } = decimal.Zero;

        [FormField("Unit MRP", order: 6, controlType: "Numeric")]
        public decimal UnitMRP { get; set; } = decimal.Zero;

        [FormField("Unit", order: 7, controlType: "Picker")]
        public PointOfSale.Models.Unit Unit { get; set; } = PointOfSale.Models.Unit.Pcs;
    }
}

namespace Bharat.Invoicing
{
    [Table("Customers")]
    public class Customer : BaseModel
    {
        [Required]
        [FormField("Name", order: 1, IsRequired = true)]
        public string Name { get; set; } = string.Empty;

        [FormField("Mobile Number", order: 2, controlType: "Phone")]
        public string MobileNumber { get; set; } = string.Empty;

        [FormField("GSTIN", order: 3)]
        public string GSTIN { get; set; } = string.Empty;

        [FormField("Address", order: 4, controlType: "Editor")]
        public string Address { get; set; } = string.Empty;

        [FormField("Pin Code", order: 5)]
        public string PinCode { get; set; } = string.Empty;

        [FormField("Date of Birth", order: 6, controlType: "DatePicker")]
        public DateTime DateOfBirth { get; set; }

        [FormField("Anniversary", order: 7, controlType: "DatePicker")]
        public DateTime AniverysaryDate { get; set; }
    }

    [Table("Vendors")]
    public class Vendor : BaseModel
    {
        [FormField("Name", order: 1)]
        public string Name { get; set; } = string.Empty;

        [FormField("Mobile Number", order: 2)]
        public string MobileNumber { get; set; } = string.Empty;

        [FormField("GSTIN", order: 3)]
        public string GSTIN { get; set; } = string.Empty;

        [FormField("Address", order: 4, controlType: "Editor")]
        public string Address { get; set; } = string.Empty;

        [FormField("Pin Code", order: 5)]
        public string PinCode { get; set; } = string.Empty;

        [FormField("Contact Name", order: 6)]
        public string ContactName { get; set; } = string.Empty;
    }
}

namespace PointOfSale.Models
{
    public enum InvoiceType { Sale = 0, SaleReturn = 1, Purchase = 2, PurchaseReturn = 3, Service = 4 }
    public enum PaymentType { Cash, Card, UPI, Cheques, IMPS, NEFT, RTGS, Others }
    public enum Unit { Pcs, Nos, Mtrs, Other }
    public enum CardType { Debit, Credit, LoyalityCard, GiftCard, AmexCard, GiftVoucher }

    [Table("PaymentDetails")]
    public class PaymentDetail : Bharat.Base.BaseModel
    {
        [Indexed]
        [FormField("Invoice Id", order: 1, controlType: "Picker")]
        public Guid InvoiceId { get; set; } = Guid.Empty;

        [FormField("On Date", order: 2, controlType: "DatePicker")]
        public DateTime OnDate { get; set; } = DateTime.Now;

        [DataType(DataType.Currency)]
        [FormField("Amount", order: 3, controlType: "Numeric")]
        public decimal Amount { get; set; } = decimal.Zero;

        [FormField("Payment Type", order: 4, controlType: "Picker")]
        public PaymentType PaymentType { get; set; }

        [FormField("Narration", order: 5)]
        public string Naration { get; set; } = string.Empty;

        [FormField("Card Type", order: 6, controlType: "Picker")]
        public CardType CardType { get; set; } = CardType.Debit;

        [FormField("Card Details", order: 7)]
        public string CardDetails { get; set; } = string.Empty;
    }

    [Table("Salesmen")]
    public class Salesman : Bharat.Base.BaseModel
    {
        [FormField("Name", order: 1)]
        public string Name { get; set; } = string.Empty;

        [FormField("Employee Id", order: 2)]
        public Guid? EmployeeId { get; set; } = Guid.Empty;

        [FormField("Active", order: 3, controlType: "Switch")]
        public bool Active { get; set; } = true;
    }

    [Table("Invoices")]
    public class InvoiceBase : Bharat.Base.BaseModel
    {
        [FormField("On Date", order: 1, controlType: "DatePicker")]
        public DateTime OnDate { get; set; } = DateTime.Now;

        [FormField("Invoice Number", order: 2)]
        public string InvoiceNumber { get; set; } = string.Empty;

        [FormField("Invoice Type", order: 3, controlType: "Picker")]
        public InvoiceType InvoiceType { get; set; } = InvoiceType.Sale;

        [FormField("Party Name", order: 4)]
        public string PartyName { get; set; } = string.Empty;

        [FormField("Party Id", order: 5, controlType: "Picker")]
        public Guid PartyId { get; set; } = Guid.Empty;

        [FormField("GSTIN", order: 6)]
        public string? GSTIN { get; set; } = string.Empty;

        [FormField("Count", order: 7, controlType: "Numeric")]
        public int Count { get; set; } = 0;

        [FormField("Quantity", order: 8, controlType: "Numeric")]
        public decimal Quntity { get; set; } = decimal.Zero;

        [DataType(DataType.Currency)]
        [FormField("Bill Amount", order: 9, controlType: "Numeric")]
        public decimal BillAmount { get; set; } = decimal.Zero;

        [FormField("Basic Amount", order: 10, controlType: "Numeric")]
        public decimal BasicAmount { get; set; } = decimal.Zero;

        [FormField("Tax Amount", order: 11, controlType: "Numeric")]
        public decimal TaxAmount { get; set; } = decimal.Zero;

        [FormField("MRP Amount", order: 12, controlType: "Numeric")]
        public decimal MRPAmount { get; set; } = decimal.Zero;

        [FormField("Discount Amount", order: 13, controlType: "Numeric")]
        public decimal DiscountAmount { get; set; } = decimal.Zero;

        [FormField("Paid", order: 14, controlType: "Switch")]
        public bool Paid { get; set; } = false;
    }

    [Table("SaleInvoices")]
    public class SaleInvoice : InvoiceBase
    {
        [FormField("B2B Invoice", order: 15, controlType: "Switch")]
        public bool B2BInvoices { get; set; } = false;

        [FormField("Salesman Id", order: 16, controlType: "Picker")]
        public Guid SalesmenId { get; set; } = Guid.Empty;

        // Collections are not automatically persisted by sqlite-net; store Payments in a separate table
        [Ignore]
        public ICollection<PaymentDetail> PaymentsDetails { get; set; } = new List<PaymentDetail>();
    }

    [Table("PurchaseInvoices")]
    public class PurchaseInvoice : InvoiceBase
    {
        [FormField("Inward Number", order: 15)]
        public string InwardNumber { get; set; } = string.Empty;

        [FormField("Inward Date", order: 16, controlType: "DatePicker")]
        public DateTime InwardDate { get; set; } = DateTime.Now;

        [FormField("Freight Charges", order: 17, controlType: "Numeric")]
        public decimal FrieghtCharges { get; set; } = decimal.Zero;
    }

    // Attribute used to annotate form generation metadata
    [AttributeUsage(AttributeTargets.Property)]
    public class FormFieldAttribute : Attribute
    {
        public string Label { get; }
        public string ControlType { get; set; } = "Entry";
        public int Order { get; set; } = 0;
        public string Placeholder { get; set; } = string.Empty;
        public bool IsRequired { get; set; } = false;

        public FormFieldAttribute(string label, string controlType = "Entry", int order = 0)
        {
            Label = label;
            ControlType = controlType;
            Order = order;
        }
    }
}
