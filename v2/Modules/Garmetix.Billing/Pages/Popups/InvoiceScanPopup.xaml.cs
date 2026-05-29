using CommunityToolkit.Maui.Views;
namespace Garmetix.Billing.Pages.Popups;


public class ScanPopupResult
{
    public string ActionRequested { get; set; } // "Edit", "View", or "Return"
    public string ScannedCode { get; set; }     // The Guid or InvoiceNo
}

public partial class InvoiceScanPopup : Popup<ScanPopupResult>
{
    private string _scannedCode = string.Empty;

    public InvoiceScanPopup()
    {
        InitializeComponent();

        // Auto-focus the entry box for USB Barcode Scanners on Desktop
        if (DeviceInfo.Idiom == DeviceIdiom.Desktop)
        {
            UsbScannerEntry.Focus();
        }
    }
     

     
    // 1. Triggered when Mobile Camera sees a QR/Barcode
    private void CameraScanner_BarcodesDetected(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs e)
    {
        var result = e.Results.FirstOrDefault();
        if (result != null)
        {
            // Turn off camera to save battery
            CameraScanner.IsDetecting = false;
            ProcessScan(result.Value);
        }
    }

    // 2. Triggered when USB Scanner finishes typing and hits "Enter"
    private void UsbScannerEntry_Completed(object sender, EventArgs e)
    {
        ProcessScan(UsbScannerEntry.Text);
    }

    // 3. Switch the UI from Camera to Buttons
    private void ProcessScan(string code)
    {
        if (string.IsNullOrWhiteSpace(code)) return;

        // ZXing events run on a background thread. We MUST use MainThread to update UI.
        MainThread.BeginInvokeOnMainThread(() =>
        {
            _scannedCode = code;
            ScannedCodeLabel.Text = $"Code: {code}";

            // Hide Scanner, Show Buttons
            ScanLayout.IsVisible = false;
            ActionLayout.IsVisible = true;
        });
    }

    // 4. Button Click Handlers - Close popup and return the decision
//    private void Edit_Clicked(object sender, EventArgs e) => Close(new ScanPopupResult { ActionRequested = "Edit", ScannedCode = _scannedCode });

     
    private async void Edit_Clicked(object sender, EventArgs e)
    {
        await CloseAsync(new ScanPopupResult { ActionRequested = "Edit", ScannedCode = _scannedCode });
    }

    private async void View_Clicked(object sender, EventArgs e)
    {
        await CloseAsync(new ScanPopupResult { ActionRequested = "View", ScannedCode = _scannedCode });
    }

    private async void Return_Clicked(object sender, EventArgs e)
    {
        await CloseAsync(new ScanPopupResult { ActionRequested = "Return", ScannedCode = _scannedCode });
    }
    //private void View_Clicked(object sender, EventArgs e) => Close(new ScanPopupResult { ActionRequested = "View", ScannedCode = _scannedCode });
    //private void Return_Clicked(object sender, EventArgs e) => Close(new ScanPopupResult { ActionRequested = "Return", ScannedCode = _scannedCode });

    private void Cancel_Clicked(object sender, EventArgs e)
    { 
        CameraScanner.IsDetecting = false; // Ensure camera turns off
        CloseAsync(); // Return null if cancelled
    }
}
