using Bharat.Base;
using PointOfSale.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bharat.Base
{
    public class BaseModel
    {
        public Guid Id { get; set; }= Guid.NewGuid();
       
    }
}

namespace Bharat.Inventory
{

    public enum ProductCategory { Fabric, Readymade, InnerWear, WinterWear, EthinicWear, Accessiory, Bag, GiftItem, MiscItem, Others}
    public enum ProductSubCategory {Others, Shriting, Suiting, SuitLength, Suits, Coat, Blazers, Sherwani, Jhodpuri, Kurta, Pajama, KurtaPajama, JuttiNagra, Pagdi, Duppata, PagdiDuppta, Broch, Tie, PocketSquare, Thali, Mala, Sword, Bundi, KotiSet, Trousers, Shirt, TShit, Jeans, CottonPant, Short, Brief, Vest, Jackets  }

    public class Product : BaseModel
    {
        public string Name { get; set; }= string.Empty;
        public string Description { get; set; }=string.Empty;
        public string HSNCode { get; set; } = string.Empty;
        public string Barcode { get; set; } = string.Empty;
        public decimal MRP { get; set; }=decimal.Zero;
        public Unit Unit { get; set; } = Unit.Pcs;
        public decimal UnitPrice { get; set; }= decimal.Zero;
        public ProductCategory ProductCategory { get; set; } = ProductCategory.Readymade;
        public ProductSubCategory ProductSubCategory { get; set; } = ProductSubCategory.Others;

    }

    public class Stock : BaseModel
    {
        public Guid ProductId {  get; set; }
        public string Barcode { get; set;  } = string.Empty;
        public decimal PurchaseQty { get;set;  }= decimal.Zero;
        public decimal CostPrice { get; set;  }= decimal.Zero;
        public decimal SoldQty { get; set; } = decimal.Zero;
        public decimal UnitMRP { get; set; } = decimal.Zero;
        public Unit Unit { get;set; } = Unit.Pcs;
    }
}

namespace Bharat.Invoicing {

    public class Customer : BaseModel { 
    
        public string Name { get; set; }
        public string MobileNumber { get; set; }
        public string GSTIN { get; set; }
        public string Address { get; set; }
        public string PinCode { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime AniverysaryDate {  get; set; }
    
    }


    public class Vendor : BaseModel
    {
        public string Name { get; set; }
        public string MobileNumber { get; set; }
        public string GSTIN { get; set; }
        public string Address { get; set; }
        public string PinCode { get; set; }
        public string ContactName { get; set; }

    }

}


namespace PointOfSale.Models
{
    public enum InvoiceType { Sale = 0 , SaleReturn = 1,Purchase=2 ,PurchaseReturn = 3, Service=4 }
    public enum PaymentType { Cash, Card, UPI, Cheques, IMPS, NEFT, RTGS,Others}
    public enum Unit { Pcs, Nos, Mtrs, Other }
    public enum CardType { Debit, Credit, LoyalityCard, GiftCard, AmexCard, GiftVoucher }
    public class PaymentDetail : BaseModel
    {
        public Guid InvoiceId { get; set; }= new Guid();
        public DateTime OnDate {  get; set; }= DateTime.Now;
        public decimal Amount { get; set; }= decimal.Zero;
        public PaymentType PaymentType { get; set; }
        public string Naration { get; set; }= string.Empty;
        public CardType CardType { get; set; }= CardType.Debit;
        public string CardDetails { get; set; } = string.Empty;

    }

    public class Salesman : BaseModel
    {
        public string Name { get; set; }=string.Empty;
        public Guid? EmployeeId {  get; set; }= Guid.Empty;
        public bool Active { get; set; } = true;

    }

    public class InvoiceBase:BaseModel
    {
        public DateTime OnDate {  get; set; }= DateTime.Now;
        public string InvoiceNumber { get; set; }= string.Empty;
        public InvoiceType InvoiceType { get; set; }= InvoiceType.Sale;
        public string PartyName { get; set; }= string.Empty;
        public Guid PartyId {  get; set; }= Guid.Empty;
        public string? GSTIN { get; set; }= string.Empty;
        public int Count { get; set; }= 0;
        public decimal Quntity {  get; set; }= decimal.Zero;
        public decimal BillAmount { get; set; } = decimal.Zero;
        public decimal BasicAmount {  get; set; } = decimal.Zero;
        public decimal TaxAmount {  get; set; } = decimal.Zero;
        public decimal MRPAmount { get; set; } = decimal.Zero;
        public decimal DiscountAmount { get; set; } = decimal.Zero;
        public bool Paid { get; set; } = false;
    }


    public class SaleInvoice : InvoiceBase
    {
        public bool B2BInvoices { get; set; }= false;
        public Guid SalesmenId {  get; set; }= Guid.Empty;
        public ICollection<PaymentDetail> PaymentsDetails { get; set; }= new List<PaymentDetail>();
    }

    public class PurchaseInvoice : InvoiceBase { 
        public string InwardNumber { get; set; }= string.Empty;
        public DateTime InwardDate {  get; set; }= DateTime.Now;
        public decimal FrieghtCharges { get; set;  }= decimal.Zero;
        
    }
}
