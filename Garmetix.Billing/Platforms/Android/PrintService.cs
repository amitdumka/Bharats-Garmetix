using Android.Bluetooth;
using Android.Content;
using Android.Print;
using Garmetix.Billing.AIBased.Services;
using Java.Util;
using System.Threading.Tasks;
using Android.Content;
using Android.Print;
using Application = Android.App.Application;
// 1. ADD THESE TWO ALIASES USING "global::"
using NativeWebView = global::Android.Webkit.WebView;
using NativeWebViewClient = global::Android.Webkit.WebViewClient;

namespace Garmetix.AI.Billing.Platforms.Android
{
    public class PrintService : IPrintService
    {
        private static readonly UUID RspSppUuid = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");
        public Task PrintHtmlAsync(string htmlContent, string documentName = "Invoice")
        {
            var context = Application.Context;

            // 2. Use the alias to create the WebView
            var webView = new NativeWebView(context);

            webView.SetWebViewClient(new PrintWebViewClient(documentName));
            webView.LoadDataWithBaseURL(null, htmlContent, "text/HTML", "UTF-8", null);

            return Task.CompletedTask;
        }

        // 3. Inherit from the alias
        private class PrintWebViewClient : NativeWebViewClient
        {
            private readonly string _documentName;

            public PrintWebViewClient(string documentName)
            {
                _documentName = documentName;
            }

            // 4. Use the alias in the method signature
            public override void OnPageFinished(NativeWebView view, string url)
            {
                base.OnPageFinished(view, url);

                var printManager = (PrintManager)Application.Context.GetSystemService(Context.PrintService);
                var printAdapter = view.CreatePrintDocumentAdapter(_documentName);

                var printAttributes = new PrintAttributes.Builder()
                    .SetMediaSize(PrintAttributes.MediaSize.IsoA5)
                    .Build();

                printManager.Print(_documentName, printAdapter, printAttributes);
            }
        }
        public async Task PrintReceiptAsync(byte[] receiptData)
        {
            try
            {
                // 1. CRITICAL: Check and Request Bluetooth Permissions first
                var permissionStatus = await CheckBluetoothPermissionsAsync();
                if (permissionStatus != PermissionStatus.Granted)
                {
                    System.Diagnostics.Debug.WriteLine("Bluetooth permission denied by user.");
                    return;
                }

                // 2. Modern way to get the Bluetooth Adapter (DefaultAdapter is deprecated)
                var bluetoothManager = (BluetoothManager)global::Android.App.Application.Context.GetSystemService(Context.BluetoothService);
                BluetoothAdapter? adapter = bluetoothManager?.Adapter;

                if (adapter == null || !adapter.IsEnabled)
                {
                    System.Diagnostics.Debug.WriteLine("Bluetooth is turned off or not supported.");
                    return;
                }

                var bonded = adapter.BondedDevices;
                if (bonded == null || bonded.Count == 0)
                    throw new Exception("No paired Bluetooth printer found.");

                BluetoothDevice? printer = bonded.FirstOrDefault(d =>
                    (d.BluetoothClass?.MajorDeviceClass == MajorDeviceClass.Imaging) ||
                    (d.Name != null && (
                        d.Name.IndexOf("printer", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        d.Name.IndexOf("pos", StringComparison.OrdinalIgnoreCase) >= 0
                    ))
                ) ?? throw new Exception("No paired Bluetooth printer found.");
                BluetoothSocket? socket = printer.CreateRfcommSocketToServiceRecord(RspSppUuid) ?? throw new Exception("Failed to create Bluetooth socket for printer.");
                await socket.ConnectAsync();

                if (socket.IsConnected)
                {
                    var output = socket.OutputStream;
                    if (output != null)
                    {
                        await output.WriteAsync(receiptData, 0, receiptData.Length);
                        await output.FlushAsync();
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Android Print Error: socket.OutputStream was null.");
                    }

                    socket.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Android Print Error: {ex.Message}");
            }
        }

        // Helper method to handle MAUI Runtime Permissions
        private async Task<PermissionStatus> CheckBluetoothPermissionsAsync()
        {
            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.Bluetooth>();

            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Bluetooth>();
            }

            return status;
        }
    }
}