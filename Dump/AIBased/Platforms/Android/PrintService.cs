using System;
using System.Linq;
using System.Threading.Tasks;
using Android.Bluetooth;
using Java.Util;
using AadwikaBilling.Services;

namespace AadwikaBilling.Platforms.Android
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

                BluetoothDevice printer = adapter.BondedDevices.FirstOrDefault(d => 
                    d.BluetoothClass.MajorDeviceClass == BluetoothClassDeviceMajor.Imaging ||
                    d.Name.ToLower().Contains("printer") || 
                    d.Name.ToLower().Contains("pos"));

                if (printer == null) throw new Exception("No paired Bluetooth printer found.");

                BluetoothSocket socket = printer.CreateRfcommSocketToServiceRecord(RspSppUuid);
                
                await socket.ConnectAsync();
                
                if (socket.IsConnected)
                {
                    await socket.OutputStream.WriteAsync(receiptData, 0, receiptData.Length);
                    await socket.OutputStream.FlushAsync();
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