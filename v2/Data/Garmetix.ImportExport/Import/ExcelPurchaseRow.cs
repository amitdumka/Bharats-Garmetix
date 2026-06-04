using Garmetix.Core.Models.Inventory;
using System.Collections.Concurrent;

namespace Garmetix.ImportExport.Models
{
    public class ExcelPurchaseRow
    {
        public string SN { get; set; }  // Consider
        public string InwardDate { get; set; } //Consider
        public string InwardNumber { get; set; } //Consider and Generate Your Own Inward Number
        public string Brand { get; set; } // Consider
        public string Category { get; set; } // Consider
        public string Barcode { get; set; } // Consider and Update your Barcode in your Inventory
        public string ItemName { get; set; }  //Consider  and Update your Item Name in your Inventory
        public string Description { get; set; } // Consider
        public string StyleCode { get; set; } //Consdier
        public string ArtNo { get; set; } // Consider
        public string ShadeColor { get; set; } //Consider
        public string Sizes { get; set; } //Consider
        public decimal Rate { get; set; } // Consider
        public decimal Qty { get; set; } // Consider
        public decimal Discount { get; set; } // Consider
        public string Unit { get; set; } // Consider and Generate based on Item and Validate 
        public decimal MRP { get; set; } //Consider
        public decimal BasicRate { get; set; } // Consider
        public decimal BasicAmount { get; set; } // Consider and Calculate Basic Amount = Basic Rate * Qty
        public decimal InputTaxRate { get; set; } //Consider and Check the logic
        public decimal InputTaxAmount { get; set; }  // Consider and Calculate Input Tax Amount = Basic Amount * Input Tax Rate
        public decimal CostValue { get; set; } //Consider and Calculate Cost Value = Basic Amount + Input Tax Amount
        public decimal AdditionalCostRate { get; set; } // Consider and Calculate Additional Cost Rate based on your logic or input
        public decimal AdditionalCostAmount { get; set; } // Consider and Calculate Additional Cost Amount = Cost Value * Additional Cost Rate
        public decimal ActualCost { get; set; } // Consider and Calculate Actual Cost = Cost Value + Additional Cost Amount
        public decimal FreightCost { get; set; } // Consider and Calculate Freight Cost based on your logic or input
        public decimal AcquisitionCost { get; set; } // Consider and Calculate Acquisition Cost = Actual Cost + Freight Cost
        public decimal CostWithAllInclusive { get; set; }//
        public decimal ProjectedMRP { get; set; } //
        public decimal MRPOutputTaxRate { get; set; } //
        public decimal OutputTaxAmount { get; set; } //
        public decimal GSTPurchase { get; set; } //
        public string HSNCode { get; set; } //
        public string ItemCode { get; set; } // 
        public string BrandCode { get; set; }// 
        public string CategoryCode { get; set; }// 
        public decimal SoldPrice { get; set; }// 
        public decimal SoldQty { get; set; }// No need
        public string SoldDate { get; set; } // No need
        public decimal CurrentStock { get; set; } // No need
        public decimal CurrentCostValue { get; set; } // No need 
        public decimal ProfitLoss { get; set; } // No need 
        public decimal ProfitLossPercentage { get; set; }// No need
        public string Supplier { get; set; } //Consider
        public string InvoiceDate { get; set; } //Consider
        public string InvoiceNumber { get; set; } //Consider
        public string Status { get; set; }
    }
}


 

