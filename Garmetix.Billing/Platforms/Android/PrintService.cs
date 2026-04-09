using System;
using System.Linq;
using System.Threading.Tasks;
using Android.Bluetooth;
using Java.Util;
using Garmetix.AI.Billing.Services;

namespace  Garmetix.Billing.Platforms.Android
{
    public class PrintService : IPrintService
    {
        private static readonly UUID RspSppUuid = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");

        public async Task PrintReceiptAsync(byte[] receiptData)
        {
            try
            {
                BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
                if (adapter == null || !adapter.IsEnabled) return;

                var bonded = adapter.BondedDevices;
                if (bonded == null || bonded.Count == 0)
                    throw new Exception("No paired Bluetooth printer found.");

                BluetoothDevice? printer = bonded.FirstOrDefault(d =>
                    (d.BluetoothClass?.MajorDeviceClass == MajorDeviceClass.Imaging) ||
                    (d.Name != null && (
                        d.Name.IndexOf("printer", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        d.Name.IndexOf("pos", StringComparison.OrdinalIgnoreCase) >= 0
                    ))
                );

                if (printer == null) throw new Exception("No paired Bluetooth printer found.");

                BluetoothSocket? socket = printer.CreateRfcommSocketToServiceRecord(RspSppUuid);
                if (socket == null) throw new Exception("Failed to create Bluetooth socket for printer.");

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
    }
}