using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Garmetix_Billing.ViewModels;

// Customer details
public class Customer
{
    public string Name { get; set; }
    public string Mobile { get; set; }
    public string Gstin { get; set; } // Optional
}

// Single invoice line item
public class InvoiceItem
{
    public string ItemCode { get; set; }
    public string Description { get; set; }
    public int Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal Amount => Quantity * Rate;
    public decimal GstPercent { get; set; } = 12m; // default 12%
    public decimal Cgst => Amount * GstPercent / 200m;
    public decimal Sgst => Amount * GstPercent / 200m;
    public decimal Total => Amount + Cgst + Sgst;
}

//internal class InvoiceEntryViewModel
//{
//    public InvoiceEntryViewModel() { }
//}
internal partial class InvoiceEntryViewModel : ObservableObject
{
    [ObservableProperty] private Customer customer = new();
    [ObservableProperty] private ObservableCollection<InvoiceItem> items = new();
    [ObservableProperty] private InvoiceItem newItem = new();
    [ObservableProperty] private string upiQrData;
    [ObservableProperty] private string scannedCode;

    public decimal SubTotal => Items.Sum(i => i.Amount);
    public decimal TotalCgst => Items.Sum(i => i.Cgst);
    public decimal TotalSgst => Items.Sum(i => i.Sgst);
    public decimal GrandTotal => Items.Sum(i => i.Total);

    public InvoiceEntryViewModel()
    {
        NewItem = new InvoiceItem();
    }

    [RelayCommand]
    void AddItem()
    {
        if (string.IsNullOrWhiteSpace(NewItem.Description) || NewItem.Quantity <= 0)
            return;

        Items.Add(NewItem);
        NewItem = new InvoiceItem();
        OnPropertyChanged(nameof(SubTotal));
        OnPropertyChanged(nameof(TotalCgst));
        OnPropertyChanged(nameof(TotalSgst));
        OnPropertyChanged(nameof(GrandTotal));
    }

    [RelayCommand]
    async Task ScanBarcodeAsync()
    {
        //TODO: Check and Enable
        //var result = await BarcodeReader.Default.ScanAsync();
        //if (!string.IsNullOrEmpty(result.Text))
        //    ScannedCode = result.Text;
    }

    [RelayCommand]
    void GenerateUpiQr()
    {
        // Example UPI URI: "upi://pay?pa=merchant@bank&pn=StoreName&am=1000"
        var amount = GrandTotal.ToString("F2");
        upiQrData = $"upi://pay?pa=merchant@bank&pn=GarmentStore&am={amount}&cu=INR";


    }
}