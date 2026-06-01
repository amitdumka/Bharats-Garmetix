using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Android.Bluetooth;
using Garmetix.RemoteReceiver.Services;
using Java.Util;

namespace Garmetix.RemoteReceiver.Platforms.Android
{
    public class AndroidBluetoothPrinter : IPlatformPrinter
    {
        // Standard UUID for serial Bluetooth SPP profiles (Standard for thermal printers)
        private static readonly UUID SerialUuid = UUID.FromString("00001101-0000-1000-8000-00805f9b34fb");

        public async Task PrintRawPayloadAsync(byte[] data)
        {
            BluetoothAdapter bluetoothAdapter = BluetoothAdapter.DefaultAdapter;

            if (bluetoothAdapter == null || !bluetoothAdapter.IsEnabled)
                throw new Exception("Bluetooth adapter is turned off or unavailable on this device.");

            // 1. Look for paired devices. Often thermal printers contain "Printer", "POS", or "MTP" in their name
            BluetoothDevice printerDevice = bluetoothAdapter.BondedDevices
                .FirstOrDefault(d => d.Name.Contains("Printer", StringComparison.OrdinalIgnoreCase) ||
                                     d.Name.Contains("POS", StringComparison.OrdinalIgnoreCase));

            if (printerDevice == null)
                throw new Exception("No paired Bluetooth POS printer found. Please pair the printer in Android system settings first.");

            BluetoothSocket socket = null;

            await Task.Run(() =>
            {
                try
                {
                    // 2. Establish RFCOMM network stream channel
                    socket = printerDevice.CreateRfcommSocketToServiceRecord(SerialUuid);
                    socket.Connect();

                    // 3. Write raw print stream straight to hardware buffer
                    using (Stream outStream = socket.OutputStream)
                    {
                        outStream.Write(data, 0, data.Length);
                        outStream.Flush();
                    }
                }
                finally
                {
                    // 4. Safely release hardware locks
                    if (socket != null)
                    {
                        socket.Close();
                        socket.Dispose();
                    }
                }
            });
        }
    }
}