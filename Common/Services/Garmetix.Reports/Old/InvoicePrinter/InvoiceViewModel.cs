// ViewModels/InvoicePageModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Garmetix.Reports.InvoicePrinter.Models;
using Garmetix.Reports.InvoicePrinter.Services;

namespace Garmetix.Reports.InvoicePrinter.ViewModels
{
    public partial class InvoicePageModel : ObservableObject
    {
        private readonly IPrintService _printService;

        public InvoicePageModel(IPrintService printService)
        {
            _printService = printService;
        }

        [RelayCommand]
        private async Task GenerateInvoice()
        {
            // Create sample invoice data
            var invoiceData = new InvoiceModel
            {
                InvoiceNumber = "INV-2024-00123",
                IssueDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(15),
                SellerInfo = new CompanyInfo
                {
                    Name = "Aadwika Fashions.",
                    AddressLine1 = "Bhagalpur Road, Dumka",
                    AddressLine2 = "Dumka, Jharkhand 814101",
                    Gstin = "20CLEPK0467L1Z8",
                    Phone = "+91-6434224461"
                },
                BuyerInfo = new CompanyInfo
                {
                    Name = "Retail King Inc.",
                    AddressLine1 = "456 Commerce Street",
                    AddressLine2 = "Mumbai, Maharashtra 400001",
                    Gstin = "27FGHIJ5678K1Z4",
                },
                Items = new List<InvoiceItem>
                {
                    new InvoiceItem { ItemName = "Wireless Mouse", Barcode = "890123456789", StyleCode = "WM-BLK-01", HsnCode = "847160", Quantity = 2, Rate = 1200, DiscountPercentage = 5 },
                    new InvoiceItem { ItemName = "Mechanical Keyboard", Barcode = "890123456790", StyleCode = "MK-RGB-01", HsnCode = "847160", Quantity = 1, Rate = 4500, DiscountPercentage = 0 },
                    new InvoiceItem { ItemName = "4K Webcam", Barcode = "890123456791", StyleCode = "WC-4K-01", HsnCode = "852580", Quantity = 1, Rate = 8000, DiscountPercentage = 10 },
                    new InvoiceItem { ItemName = "USB-C Hub", Barcode = "890123456792", StyleCode = "UCH-8P-01", HsnCode = "850440", Quantity = 3, Rate = 2500, DiscountPercentage = 0 },
                },
                // Set to 18 if buyer is in another state, 0 otherwise
                IgstRate = 0 // Since seller is in Karnataka and buyer in Maharashtra, this should ideally be 18
            };
            
            // A simple logic check for IGST
            if (invoiceData.SellerInfo.AddressLine2.Contains("Karnataka") && invoiceData.BuyerInfo.AddressLine2.Contains("Maharashtra"))
            {
                invoiceData.IgstRate = 18;
                invoiceData.CgstRate = 0;
                invoiceData.SgstRate = 0;
            }

            // Call the service
            await _printService.CreateAndPrintInvoiceAsync(invoiceData);
        }
    }
}
