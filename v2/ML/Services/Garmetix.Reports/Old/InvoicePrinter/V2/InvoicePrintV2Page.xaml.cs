// MainPage.xaml.cs (Code-behind)
using Garmetix.Reports.InvoicePrinter.V2.Models;
using Garmetix.Reports.InvoicePrinter.V2.Services;
using System.Collections.ObjectModel; // Required for ObservableCollection

namespace Garmetix.Reports.InvoicePrinter.V2;

public partial class InvoicePrintV2Page : ContentPage
{
    private readonly IPdfService _pdfService;
   // private readonly IPrintService _printService;

    // Use ObservableCollection for item list to enable easy UI binding and updates
    public ObservableCollection<InvoiceItem> InvoiceItems { get; set; }

    public InvoicePrintV2Page()
    {
        InitializeComponent();

        _pdfService = new PdfService();
       // _printService = new PrintService();

        // Initialize with some sample data for demonstration
        InvoiceItems = new ObservableCollection<InvoiceItem>();
        LoadSampleInvoiceData();

        BindingContext = this; // Set the BindingContext for data binding
    }

    private void LoadSampleInvoiceData()
    {
        // Sample Seller (Aadwika Sweets from your PDF)
        var seller = new Party
        {
            Name = "Aadwika Sweets",
            Address = "Your Company Address, City, State, Pin", // Placeholder
            ContactNo = "9334799099",
            Email = "amit.dumka@gmail.com",
            GSTIN = "20AJHPA7396P1ZV",
            State = "20-Jharkhand"
        };

        // Sample Buyer (AMit kumar from your PDF)
        var buyer = new Party
        {
            Name = "AMit kumar",
            Address = "NEAR TATA SHOWROOM BHAGALPUR ROAD DUMKA DUMKA",
            ContactNo = "9334799099",
            GSTIN = "20AJHPA7396P1ZV", // Assuming same GSTIN for simplicity, or modify if different
            State = "20-Jharkhand"
        };

        // Sample Invoice Items
        var item1 = new InvoiceItem
        {
            SNo = 1,
            ItemName = "Shirting",
            Barcode = "SHIRT12345",
            StyleCode = "M-XL-001",
            HSNCode = "11", // As per your PDF, but typically 4 or 6 digits
            Rate = 1999.00m,
            Quantity = 11,
            Discount = 0.00m, // No discount for now
            CGSTPercentage = 2.5m, // 5% total GST, so 2.5% CGST
            SGSTPercentage = 2.5m // 2.5% SGST
        };

        // Calculate LineTotal, TaxableAmount, CGSTAmt, SGSTAmt, TotalTax for item1
        item1.LineTotal = (item1.Rate * item1.Quantity) - item1.Discount;
        item1.TaxableAmount = item1.LineTotal; // Assuming no other charges for taxable amount calculation
        item1.CGSTAmt = item1.TaxableAmount * (item1.CGSTPercentage / 100);
        item1.SGSTAmt = item1.TaxableAmount * (item1.SGSTPercentage / 100);
        item1.TotalTax = item1.CGSTAmt + item1.SGSTAmt;

        InvoiceItems.Add(item1);

        // Create the Invoice object
        var sampleInvoice = new Invoice
        {
            InvoiceNo = "1",
            InvoiceDate = DateTime.Parse("2025-06-30"),
            Seller = seller,
            Buyer = buyer,
            PlaceOfSupply = "20-Jharkhand",
            Items = InvoiceItems.ToList() // Convert ObservableCollection to List for the Invoice model
        };

        // Calculate overall totals
        sampleInvoice.SubTotal = sampleInvoice.Items.Sum(item => item.LineTotal);
        sampleInvoice.TotalTaxAmount = sampleInvoice.Items.Sum(item => item.TotalTax);
        sampleInvoice.GrandTotal = sampleInvoice.SubTotal + sampleInvoice.TotalTaxAmount;

        // Round off logic (as per your PDF, -0.45 for 23088.45 to 23088.00)
        sampleInvoice.RoundOff = Math.Round(sampleInvoice.GrandTotal) - sampleInvoice.GrandTotal;
        sampleInvoice.GrandTotal = Math.Round(sampleInvoice.GrandTotal); // Final grand total after rounding

        sampleInvoice.AmountInWords = ConvertNumberToWords(sampleInvoice.GrandTotal);

        // Set the invoice data to a property accessible by the UI
        currentInvoice = sampleInvoice; // Store the invoice for printing
    }

    // A simple property to hold the current invoice for printing
    private Invoice currentInvoice;

    private async void OnPrintInvoiceClicked(object sender, EventArgs e)
    {
        if (currentInvoice == null)
        {
            await DisplayAlert("Error", "No invoice data to print. Please ensure data is loaded.", "OK");
            return;
        }

        try
        {
            // Generate PDF bytes
            var filePath = await _pdfService.GenerateInvoicePdf(currentInvoice);

            // Trigger printing
            //await _printService.PrintPdf(pdfBytes, $"Invoice_{currentInvoice.InvoiceNo}");
            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(filePath)
            });
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to generate or print invoice: {ex.Message}", "OK");
        }
    }

    // Helper function to convert number to words (Simplified for example)
    // For a robust solution, consider a dedicated NuGet package or more complete implementation
    private string ConvertNumberToWords(decimal number)
    {
        if (number == 0) return "Zero Rupees Only";

        long wholePart = (long)Math.Floor(number);
        long fractionalPart = (long)((number - wholePart) * 100);

        string words = NumToWords(wholePart) + " Rupees";
        if (fractionalPart > 0)
        {
            words += " and " + NumToWords(fractionalPart) + " Paisa";
        }
        return words + " Only";
    }

    private string NumToWords(long n)
    {
        if (n < 0) return "Minus " + NumToWords(-n);
        if (n == 0) return ""; // Handled by calling function for main zero

        string[] units = { "", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
        string[] tens = { "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

        if (n < 20) return units[n];
        if (n < 100) return tens[n / 10] + ((n % 10 != 0) ? " " + units[n % 10] : "");
        if (n < 1000) return units[n / 100] + " Hundred" + ((n % 100 != 0) ? " " + NumToWords(n % 100) : "");
        if (n < 100000) return NumToWords(n / 1000) + " Thousand" + ((n % 100000 != 0) ? " " + NumToWords(n % 1000) : "");
        if (n < 10000000) return NumToWords(n / 100000) + " Lakh" + ((n % 100000 != 0) ? " " + NumToWords(n % 100000) : "");
        return NumToWords(n / 10000000) + " Crore" + ((n % 10000000 != 0) ? " " + NumToWords(n % 10000000) : "");
    }
}
