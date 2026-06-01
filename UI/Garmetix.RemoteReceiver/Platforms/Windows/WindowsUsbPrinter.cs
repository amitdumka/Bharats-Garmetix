using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Garmetix.RemoteReceiver.Services;

namespace Garmetix.RemoteReceiver.Platforms.Windows
{
    public class WindowsUsbPrinter : IPlatformPrinter
    {
        // Change this string to match the exact name of your printer in the Windows Control Panel
        private const string TargetPrinterName = "POS-80";

        // Native Windows API wrappers to talk directly to the print spooler
        [DllImport("winspool.drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool StartDocPrinter(IntPtr hPrinter, int level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

        [DllImport("winspool.drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, int dwCount, out int dwWritten);

        [StructLayout(LayoutKind.Sequential)]
        private class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)] public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)] public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)] public string pDataType;
        }

        public async Task PrintRawPayloadAsync(byte[] data)
        {
            await Task.Run(() =>
            {
                IntPtr hPrinter = IntPtr.Zero;
                DOCINFOA di = new DOCINFOA { pDocName = "Aadwika Fashion Remote Receipt", pDataType = "RAW" };

                if (!OpenPrinter(TargetPrinterName, out hPrinter, IntPtr.Zero))
                    throw new Exception($"Cannot open Windows printer queue for: {TargetPrinterName}. Ensure it is connected and turned on.");

                try
                {
                    if (StartDocPrinter(hPrinter, 1, di))
                    {
                        if (StartPagePrinter(hPrinter))
                        {
                            // Allocate unmanaged memory memory blocks for interop handling
                            IntPtr pBytes = Marshal.AllocHGlobal(data.Length);
                            Marshal.Copy(data, 0, pBytes, data.Length);

                            bool success = WritePrinter(hPrinter, pBytes, data.Length, out int bytesWritten);
                            Marshal.FreeHGlobal(pBytes);

                            EndPagePrinter(hPrinter);
                        }
                        EndDocPrinter(hPrinter);
                    }
                }
                finally
                {
                    ClosePrinter(hPrinter);
                }
            });
        }
    }
}