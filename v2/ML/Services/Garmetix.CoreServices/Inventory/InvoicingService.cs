using Garmetix.CoreServices;
using Garmetix.Databases;
using Garmetix.Models.Inventory;
using Garmetix.Models.ViewModels; 
using Microsoft.EntityFrameworkCore;

//TODO: check and solve
using CustomerInfo = Garmetix.Models.Info.CustomerInfo;
using Invoice = Garmetix.Models.Inventory.Invoice;
using SaleInvoice = Garmetix.Models.Info.SaleInvoice;
using StockInfo = Garmetix.Models.Info.StockInfo;
using Vendor = Garmetix.Models.Inventory.Vendor;
using VendorInfo = Garmetix.Models.Info.VendorInfo;

namespace Garmetix.ModuleService
{
    /// <summary>
    /// Invoicing Service
    /// It helps to generate invoice
    /// </summary>
    public class InvoicingService: BaseServices
    {

        public List<StockInfo> StockInfos = [];
        public List<Stock> Stocks;
        //protected static DatabaseContext Db => DatabaseService.Instance.LocalDB;
        /// <summary>
        /// Main Entry Point
        /// </summary>
        public InvoicingService()
        {

            Stocks = [];
        }
        public static async Task<Product> FetchProduct(Guid id) => await Db?.Products?.Where(c => !c.Deleted && c.Id == id)!.FirstOrDefaultAsync()!;

        /// <summary>
        /// Fetch Salesmen
        /// </summary>
        /// <param name="storeid"></param>
        /// <returns></returns>

        public async Task SalesmenListAsync(Guid storeid) => await Db.Salesmen.Where(c => !c.Deleted && c.StoreId == storeid)
                .Select(c => new ComboBoxItemVM { Id = c.Id, Name = c.Name })
                .ToListAsync();

        /// <summary>
        /// Fetch Customer by Mobile
        /// </summary>
        /// <param name="mobile">Custoemr Mobile Number</param>
        /// <returns>It returns CustomerInfo</returns>
        public async Task<CustomerInfo?> FetchCustomer(string mobile)
        {
            return await Db.Customers.Where(c => !c.Deleted && c.MobileNumber == mobile)
                 .Select(c => new CustomerInfo { Count = c.BillCount, Id = c.Id, Name = c.Name, Mobile = c.MobileNumber, PurchaseValue = c.Amount })
                 .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Fetch All Customers
        /// </summary>
        /// <returns>It return list of Customers</returns>
        public async Task FetchCustomers() => await Db.Customers.Where(c => !c.Deleted).OrderBy(c => c.Name).ToListAsync();

        public void FetchInvoice()
        {
            throw new NotImplementedException();
        }

        public void FetchPurchaseInvoice()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Fetch Stock Item by Barcode
        /// </summary>
        /// <param name="barcode">Barcode of Product</param>
        /// <param name="storeId">StoreId of Store</param>
        /// <returns>it return StockInfo object</returns>
        public async Task<StockInfo?> FetchStockInfoAsync(string barcode, Guid storeId)
        {
            if (StockInfos.Any(s => s.Barcode == barcode))
            {
                return StockInfos.FirstOrDefault(s => s.Barcode == barcode);
            }

            var stock = await Db.Stocks.Include(s => s.Product).Where(s => s.Barcode == barcode && s.StoreId == storeId)
                .Select(c => new StockInfo
                {
                    Quantity = c.CurrentStock,
                    TaxRate = c.TaxRate,
                    Barcode = c.Barcode,
                    HSNCode = c.HSNCode,
                    UnitPrice = c.MRP,
                    ProductId = c.ProductId,
                    ProductName = c.Product!.Name
                })
                .FirstOrDefaultAsync();

            if (stock != null)
            {
                StockInfos.Add(stock);
            }

            return stock;
        }

        /// <summary>
        /// Fetch Stock Item based on Product Id
        /// </summary>
        /// <param name="productId">Product Id of Product</param>
        /// <returns>it returns Stock Info</returns>
        public async Task<StockInfo?> FetchStockInfoAsync(Guid productId)
        {

            if (StockInfos.Any(s => s.ProductId == productId))
            {
                return StockInfos.FirstOrDefault(s => s.ProductId == productId);
            }

            var stock = await Db.Stocks.Include(s => s.Product).Where(s => s.ProductId == productId)
                .Select(c => new StockInfo
                {
                    Quantity = c.CurrentStock,
                    TaxRate = c.TaxRate,
                    Barcode = c.Barcode,
                    HSNCode = c.HSNCode,
                    UnitPrice = c.MRP,
                    ProductId = c.ProductId,
                    ProductName = c.Product!.Name
                })
                .FirstOrDefaultAsync();

            if (stock != null)
            {
                StockInfos.Add(stock);
            }

            return stock;

        }

        public async Task<Vendor?> FetchVendor(string gstin)
        {
            return await Db.Vendors.Where(c => c.GSTIN == gstin).FirstOrDefaultAsync();
        }

        public async Task<Vendor?> FetchVendor(Guid vendorId)
        {
            return await Db.Vendors.FindAsync(vendorId);
            //.Where(c => c.Id == vendorId).FirstOrDefaultAsync();
        }

        public async Task FetchVendors() => await Db.Vendors.Where(c => !c.Deleted && c.Active).Select(c => new VendorInfo { Id = c.Id, GSTIN = c.GSTIN!, VendorName = c.Name }).ToListAsync();

        public string GenerateSaleInvoiceNumber() => Guid.NewGuid().ToString();
        public    bool SaveInvoice(SaleInvoice invoice)
        {

            ////TODO: Return make response and error message
            //_db.Invoices.Add(invoice.Invoice);
            //var result = await _db.SaveChangesAsync();
            //if (result > 0)
            //{
            //    result = 0;
            //    _db.InvoiceItems.AddRange(invoice.InvoiceItems);
            //    result = await _db.SaveChangesAsync();
            //    if (result == invoice.InvoiceItems.Count)
            //    {
            //        result = 0;
            //        _db.InvoicePayments.AddRange(invoice.Payments);
            //        if (invoice.CardPayments.Count > 0)
            //            _db.CardPayments.AddRange(invoice.CardPayments);

            //        result = await _db.SaveChangesAsync();
            //        if (result == invoice.CardPayments.Count + invoice.Payments.Count)
            //        {
            //            return true;
            //        }
            //    }
            //}
            Task.Delay(1000).Wait();
            return false;

        }
        public void SaveInvoicePrint(SaleInvoice invoice)
        {
            throw new NotImplementedException();
        }
        public void SaveInvoice(Invoice invoice)
        {
            throw new NotImplementedException();
        }
        public void SavePrint() { throw new NotImplementedException(); }

        public void SavePrint(Invoice invoice) { throw new NotImplementedException(); }
        public void Print(SaleInvoice invoice) { throw new NotImplementedException(); }
        public void SavePurchaseInvouce(Invoice invoice) { throw new NotImplementedException(); }




    }
}


//AI Generated
//< ContentPage xmlns = "http://schemas.microsoft.com/dotnet/2021/maui"
//             xmlns: x = "http://schemas.microsoft.com/winfx/2009/xaml"
//             xmlns: local = "clr-namespace:YourAppNamespace"
//             x: Class = "YourAppNamespace.MainPage" >

//    < ContentPage.BindingContext >
//        < local:InvoiceViewModel />
//    </ ContentPage.BindingContext >

//    < StackLayout >
//        < Label Text = "Invoice Billing System" FontSize = "24" HorizontalOptions = "Center" />


//        < ListView ItemsSource = "{Binding InvoiceItems}" >
//            < ListView.ItemTemplate >
//                < DataTemplate >
//                    < ViewCell >
//                        < StackLayout Orientation = "Horizontal" >
//                            < Label Text = "{Binding Product.ProductName}" />
//                            < Label Text = "{Binding Quantity}" />
//                            < Label Text = "{Binding UnitPrice}" />
//                            < Label Text = "{Binding TotalAmount}" />
//                            < Button Text = "Remove" Command = "{Binding Path=BindingContext.RemoveProductCommand, Source={x:Reference Name=This}}" CommandParameter = "{Binding .}" />
//                        </ StackLayout >
//                    </ ViewCell >
//                </ DataTemplate >
//            </ ListView.ItemTemplate >
//        </ ListView >

//        < Picker Title = "Select Product" ItemsSource = "{Binding Products}" ItemDisplayBinding = "{Binding ProductName}" />
//        < Button Text = "Add Product" Command = "{Binding AddProductCommand}" />

//        < Label Text = "{Binding CurrentInvoice.TotalAmount, StringFormat='Total Amount: {0:C}'}" />
//        < Label Text = "{Binding CurrentInvoice.TotalTax, StringFormat='Total Tax: {0:C}'}" />
//        < Label Text = "{Binding CurrentInvoice.GrandTotal, StringFormat='Grand Total: {0:C}'}" />

//        < Button Text = "Save Invoice" Command = "{Binding SaveInvoiceCommand}" />
//    </ StackLayout >
//</ ContentPage >
