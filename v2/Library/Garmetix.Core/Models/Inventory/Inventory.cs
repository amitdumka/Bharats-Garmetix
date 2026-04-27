using Garmetix.Models.Bases;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace Garmetix.Models.Inventory
{
    public enum CARD
    {
        DebitCard,
        CreditCard,
        AmexCard,
        GiftCard,
        Other
    }

    public enum CARDType
    {
        Visa,
        MasterCard,
        Maestro,
        AmexCard,
        Dinners,
        Rupay,
        RupayCredit,
        Others,
    }
    public enum Unit
    {
        Meters,
        Nos,
        Pcs,
        Packets,
        Grams,
        Kgs,
        Liter,
        NoUnit,
        Than, Boxes
    }

    public enum TaxType
    {
        GST,
        SGST,
        CGST,
        IGST,
        VAT,
        CST,
    }

    public enum NotesType
    {
        DebitNote,
        CreditNote,
    }

    public enum InvoiceType
    {
        Sales,
        SalesReturn,
        ManualSale,
        ManualSaleReturn,
    }

    public enum PurchaseInvoiceType
    {
        Purchase,
        PurchaseReturn,
    }
    /// <summary>
    /// ProductType represents a type of a product
    /// </summary>
    public enum ProductType
    {
        Apparels,
        Clothing,
        Electronics,
        Fabric,
        Accessories,
        InnerWear,
        SuitCovers,
        FootWear,
        Readmade,
        Jewellery,
        Cosmetics,
        WinterWear,
        Others
    }


    //TODO: Convert to enum
    public class UOM : CompanyBase
    {
        public required string Name { get; set; }
        public bool Decimal { get; set; }
        public int DecimalPlace { get; set; }
    }

    public class Tax : BaseModel
    {
        public required string Name { get; set; }
        public decimal CompositeRate { get; set; }
        public TaxType TaxType { get; set; } = TaxType.GST;
        [JsonIgnore]
        public decimal IGST { get => TaxType == TaxType.IGST ? CompositeRate : 0; }
        [JsonIgnore]
        public decimal CGST { get => TaxType == TaxType.GST ? CompositeRate / 2m : TaxType == TaxType.CGST ? CompositeRate : 0; }
        [JsonIgnore]
        public decimal SGST { get => TaxType == TaxType.GST ? CompositeRate / 2m : TaxType == TaxType.SGST ? CompositeRate : 0; }

    }

    public class Stock : StoreBase
    {
        [ForeignKey("Product")]
        public Guid ProductId { get; set; }
        public required string Barcode { get; set; }
        public string? HSNCode { get; set; }
        public Unit Unit { get; set; }
        public decimal PurchaseQty { get; set; } = 0;
        public decimal CostPrice { get; set; } = 0;
        public decimal SoldQty { get; set; } = 0;
        public decimal MRP { get; set; } = 0;
        public decimal TaxRate { get; set; }
        public TaxType TaxType { get; set; }
        public Guid TaxId { get; set; }

        public bool BrandedProduct { get; set; }=true;

        [JsonIgnore]
        public virtual Tax? Tax { get; set; }
        [JsonIgnore]
        public virtual Product? Product { get; set; }

        [JsonIgnore]
        public decimal CurrentStock { get => PurchaseQty - SoldQty; }
        [JsonIgnore]
        public decimal BasicMRP { get => MRP / (1 + (TaxRate / 100)); }
        [JsonIgnore]
        public decimal UnitTax { get => MRP - BasicMRP; }
        [JsonIgnore]
        public decimal CostValue { get => CostPrice * CurrentStock; }
        [JsonIgnore]
        public decimal MRPValue { get => MRP * CurrentStock; }
    }

    public class Product : GroupBase
    {
        public required string Name { get; set; }
        public required string Barcode { get; set; }
        public string? Descriptions { get; set; }
        public decimal MRP { get; set; }
        public decimal TaxRate { get; set; }
        public Unit Unit { get; set; }
        public TaxType TaxType { get; set; }
        public ProductType ProductType { get; set; } = ProductType.Fabric;
        public Guid ProductCategoryId { get; set; }
        public Guid ProductSubCategoryId { get; set; }
        public virtual ProductCategory? ProductCategory { get; set; }
        public virtual ProductSubCategory? ProductSubCategory { get; set; }
        public virtual ICollection<Stock>? Stocks { get; set; } = null;

    }

    public class ProductCategory : CompanyBase
    {
        public required string Name { get; set; }
    }
    public class ProductSubCategory : CompanyBase
    {
        public required string Name { get; set; }
    }

    public class ProductDetail : CompanyBase
    {
        public Guid ProductId { get; set; }
        public required string Barcode { get; set; }
        public string? StyleCode { get; set; }
        public string? BaseColor { get; set; }
        public string? Brand { get; set; }
        public Guid? VendorId { get; set; }
        public virtual Vendor? Vendor { get; set; }
        public virtual Product? Product { get; set; }
    }

    public class Brand : BaseModel
    {
        public required string Name { get; set; }
        public required string BrandCode { get; set; }
        public Guid? SupplierId { get; set; } = null;
    }
}
