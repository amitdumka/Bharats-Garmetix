//using CommunityToolkit.Mvvm.ComponentModel;
//using CommunityToolkit.Mvvm.Input;
//using Garmetix.Billing.Alternatives;
//using Garmetix.Core.Models.Accounting;
//using System.Collections.ObjectModel;

//namespace Garmetix.Billing.ViewModels;

//internal partial class InvoiceEntryViewModel : ObservableObject
//{
//    [ObservableProperty] private Party customer = new();
//    [ObservableProperty] private ObservableCollection<InvoiceItem> items = new();
//    [ObservableProperty] private InvoiceItem newItem = new();

//    [ObservableProperty] private string upiQrData = string.Empty;
//    [ObservableProperty] private string scannedCode = string.Empty;

//    // Invoice Details
//    [ObservableProperty] private string invoiceNumber = $"INV-{DateTime.Now:yyyyMMddHHmmss}";
//    [ObservableProperty] private DateTime invoiceDate = DateTime.Now;
//    [ObservableProperty] private decimal billDiscountAmount;
//    [ObservableProperty] private bool isSaleReturn = false;
//    [ObservableProperty] private string invoiceCode = string.Empty;
//    [ObservableProperty] private bool isB2B = false;

//    //private async Task<string> GenerateNextInvoiceNumberAsync()
//    //{
//    //    // 1. Get Store Code from MAUI Preferences (Defaults to "AFA" if not set yet)
//    //    string storeCode = Microsoft.Maui.Storage.Preferences.Default.Get("StoreCode", "AFA");

//    //    // 2. Get Current Year and Month (e.g., "202604")
//    //    string yearMonth = DateTime.Now.ToString("yyyyMM");

//    //    // 3. Define the prefix (e.g., "AFA-202604-IN-")
//    //    string prefix = $"{storeCode}-{yearMonth}-IN-";

//    //    try
//    //    {
//    //        // 4. Find the most recent invoice in the database that matches THIS month's prefix
//    //        var lastInvoice = await _database.Table<Invoice>()
//    //            .Where(i => i.InvoiceNo.StartsWith(prefix))
//    //            .OrderByDescending(i => i.InvoiceNo)
//    //            .FirstOrDefaultAsync();

//    //        int nextSequenceNumber = 1; // Default to 1 if it's the first bill of the month

//    //        if (lastInvoice != null && !string.IsNullOrEmpty(lastInvoice.InvoiceNo))
//    //        {
//    //            // Extract the last 4 characters (the numbers) from the previous invoice
//    //            string lastSequenceStr = lastInvoice.InvoiceNo.Substring(lastInvoice.InvoiceNo.Length - 4);

//    //            if (int.TryParse(lastSequenceStr, out int lastSequence))
//    //            {
//    //                nextSequenceNumber = lastSequence + 1;
//    //            }
//    //        }

//    //        // 5. Format the number with leading zeros so it is always 4 digits (e.g., "0001")
//    //        string sequenceString = nextSequenceNumber.ToString("D4");

//    //        // 6. Return the perfectly formatted string
//    //        return $"{prefix}{sequenceString}";
//    //    }
//    //    catch (Exception ex)
//    //    {
//    //        // Fallback in case of an unexpected database read error
//    //        System.Diagnostics.Debug.WriteLine($"Error generating invoice number: {ex.Message}");
//    //        string fallbackSequence = new Random().Next(1000, 9999).ToString();
//    //        return $"{prefix}{fallbackSequence}";
//    //    }
//    //}

//    public decimal SubTotal => Items.Sum(i => i.Amount);
//    public decimal TotalCgst => Items.Sum(i => i.Cgst);
//    public decimal TotalSgst => Items.Sum(i => i.Sgst);
//    public decimal GrandTotal => Items.Sum(i => i.Total);

//    public decimal TotalItemWiseDiscount => Items.Sum(i => i.Discount);

//    public decimal TotalDiscount => TotalItemWiseDiscount + BillDiscountAmount;
//    public decimal PayableAmount => GrandTotal - BillDiscountAmount;


//    public decimal TotalQuantity => Items.Sum(i => i.Quantity);
//    public decimal Count => Items.Count;


//    // Constructor for regular invoice entry
//    public InvoiceEntryViewModel()
//    {
//        NewItem = new InvoiceItem();
//        UpiQrData = string.Empty;
//        this.IsB2B = false;
//        this.IsSaleReturn = false;
//    }

//    // Constructor for sale return or B2C invoice entry
//    public InvoiceEntryViewModel(bool saleReturn = false, bool b2c = false)
//    {
//        // Call the default constructor to initialize properties
//        this.isSaleReturn = saleReturn;
//        this.isB2B = b2c;
//        NewItem = new InvoiceItem();
//        UpiQrData = string.Empty;
//    }

//    [RelayCommand]
//    void RemoveItem(InvoiceItem item)
//    {
//        if (Items.Contains(item))
//        {
//            Items.Remove(item);
//            OnPropertyChanged(nameof(SubTotal));
//            OnPropertyChanged(nameof(TotalCgst));
//            OnPropertyChanged(nameof(TotalSgst));
//            OnPropertyChanged(nameof(GrandTotal));
//            OnPropertyChanged(nameof(TotalItemWiseDiscount));
//            OnPropertyChanged(nameof(TotalDiscount));
//            OnPropertyChanged(nameof(TotalQuantity));
//        }
//    }

//    [RelayCommand]
//    void ClearItems()
//    {
//        Items.Clear();
//        OnPropertyChanged(nameof(SubTotal));
//        OnPropertyChanged(nameof(TotalCgst));
//        OnPropertyChanged(nameof(TotalSgst));
//        OnPropertyChanged(nameof(GrandTotal));
//        OnPropertyChanged(nameof(TotalItemWiseDiscount));
//        OnPropertyChanged(nameof(TotalDiscount));
//        OnPropertyChanged(nameof(TotalQuantity));
//    }

//    [RelayCommand]
//    void SaveCustomer()
//    {
//        //TODO: Implement logic to save customer details to database or service
//        // Check for Customer Name, GSTIN, and other required fields before saving 
//        // Check if the customer already exists and update details if necessary
//        // if not exists, create a new customer record
//        //Write the code to save the customer details to the database or service here
        
//    }

//    [RelayCommand]
//    void AddItem()
//    {
//        if (string.IsNullOrWhiteSpace(NewItem.Description) || NewItem.Quantity <= 0)
//            return;

//        Items.Add(NewItem);
//        NewItem = new InvoiceItem();
//        OnPropertyChanged(nameof(SubTotal));
//        OnPropertyChanged(nameof(TotalCgst));
//        OnPropertyChanged(nameof(TotalSgst));
//        OnPropertyChanged(nameof(GrandTotal));
//        OnPropertyChanged(nameof(TotalItemWiseDiscount));
//        OnPropertyChanged(nameof(TotalDiscount));
//        OnPropertyChanged(nameof(TotalQuantity));
//    }

//    [RelayCommand]
//    async Task ScanBarcodeAsync()
//    {
//        //TODO: Check and Enable
//        //var result = await BarcodeReader.Default.ScanAsync();
//        //if (!string.IsNullOrEmpty(result.Text))
//        //    ScannedCode = result.Text;
//    }

//    [RelayCommand]
//    void GenerateUpiQr()
//    {
//        // Example UPI URI: "upi://pay?pa=merchant@bank&pn=StoreName&am=1000"
//        var amount = PayableAmount.ToString("F2");
//        UpiQrData = $"upi://pay?pa=merchant@bank&pn=GarmentStore&am={amount}&cu=INR";


//    }
//}