// Models.cs
using System;
using System.Collections.Generic;

namespace Garmetix.Reports.InvoicePrinter.V2.Models
{
    public class Party
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string ContactNo { get; set; }
        public string GSTIN { get; set; }
        public string State { get; set; }
        public string Email { get; set; }
    }

    public class InvoiceItem
    {
        public int SNo { get; set; }
        public string ItemName { get; set; }
        public string Barcode { get; set; }
        public string StyleCode { get; set; }
        public string HSNCode { get; set; }
        public decimal Rate { get; set; }
        public decimal Quantity { get; set; }
        public decimal Discount { get; set; } // Per unit discount
        public decimal LineTotal { get; set; } // Rate * Quantity - Discount
        public decimal TaxableAmount { get; set; }
        public decimal CGSTPercentage { get; set; }
        public decimal CGSTAmt { get; set; }
        public decimal SGSTPercentage { get; set; }
        public decimal SGSTAmt { get; set; }
        public decimal TotalTax { get; set; }
    }

    public class Invoice
    {
        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public Party Seller { get; set; }
        public Party Buyer { get; set; }
        public string PlaceOfSupply { get; set; }
        public List<InvoiceItem> Items { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TotalTaxAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal RoundOff { get; set; }
        public string AmountInWords { get; set; }
    }
}






// MainPage.xaml (XAML for UI)


 

 

 
