using Microsoft.Maui.Controls;
using System.IO;
using System.Threading.Tasks;

namespace Garmetix.AI.Billing.Views
{
    public partial class InvoicePreviewPage : ContentPage
    {
        private readonly string _htmlContent;
        private readonly string _invoiceNo;

        public InvoicePreviewPage(string htmlContent, string invoiceNo)
        {
            InitializeComponent();
            _htmlContent = htmlContent;
            _invoiceNo = invoiceNo;

            // Load the HTML into the WebView
            InvoiceWebView.Source = new HtmlWebViewSource { Html = _htmlContent };
        }

        private async void Close_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void Print_Clicked(object sender, EventArgs e)
        {
            // Note: Native printing of a WebView requires platform-specific code in MAUI.
            // For now, we alert the user, or you can route this to your custom IPrintService.
            await DisplayAlert("Print", "Connect to an A5 printer to print this document.", "OK");
        }

        private async void Share_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Save the HTML temporarily to share it as a file
                string fileName = $"{_invoiceNo}.html";
                string filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
                File.WriteAllText(filePath, _htmlContent);

                await Share.Default.RequestAsync(new ShareFileRequest
                {
                    Title = $"Invoice {_invoiceNo}",
                    File = new ShareFile(filePath)
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Could not share invoice: {ex.Message}", "OK");
            }
        }
    }
}