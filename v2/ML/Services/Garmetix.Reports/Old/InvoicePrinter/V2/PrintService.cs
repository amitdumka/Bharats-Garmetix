//// Services/PrintService.cs
//using Android.Content;
//using Android.OS;
//using Android.Print;
//using Java.IO;
//using Microsoft.Maui.ApplicationModel; // Common for all platforms for FileSystem.CacheDirectory and Platform.CurrentActivity
//using System.IO;
//using System.Linq; // For LINQ operations like FirstOrDefault

//namespace Garmetix.Reports.InvoicePrinter.V2.Services
//{
//    public interface IPrintService
//    {
//        Task PrintPdf(byte[] pdfBytes, string documentName);
//    }

//    public class PrintService : IPrintService
//    {
//        public async Task PrintPdf(byte[] pdfBytes, string documentName)
//        {
//#if WINDOWS
//            using Windows.Storage; // For StorageFile
//            using Windows.System; // For Launcher

//            // On Windows, we'll save to a temp file and use Launcher to open it for printing
//            var tempFilePath = Path.Combine(FileSystem.CacheDirectory, $"{documentName}.pdf");
//            File.WriteAllBytes(tempFilePath, pdfBytes);

//            var file = await StorageFile.GetFileFromPathAsync(tempFilePath);
//            if (file != null)
//            {
//                await Launcher.LaunchFileAsync(file, new LauncherOptions { DisplayApplicationPicker = true });
//            }
//            else
//            {
//                // Handle error: Could not get file from path
//                Console.WriteLine("Error: Could not get file from path for printing on Windows.");
//                await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Error", "Failed to prepare PDF for printing.", "OK");
//            }

//#elif ANDROID
//            using Android.Graphics.Pdf;
//            using Android.OS;
//            using Android.Print;
//            using Android.Content;
//            using Android.Runtime;
//            using Java.IO;
//            using Java.Nio;

//            var printManager = Platform.CurrentActivity.GetSystemService(Context.PrintService).JavaCast<PrintManager>();
//            var jobName = $"{documentName} Document";

//            printManager.Print(jobName, new PdfPrintDocumentAdapter(pdfBytes, documentName), null);

//#elif IOS || MACCATALYST
//            using UIKit;
//            using Foundation;
//            using CoreGraphics; // For CGRect
//            using System.Drawing; // For RectangleF (if used, though CoreGraphics.CGRect is preferred)

//            var printInfo = UIPrintInfo.PrintInfo;
//            printInfo.OutputType = UIPrintInfoOutputType.General;
//            printInfo.JobName = documentName;

//            var printController = UIPrintInteractionController.SharedPrintController;
//            printController.PrintInfo = printInfo;
//            printController.PrintingItem = NSData.FromArray(pdfBytes);

//            // Present the print controller
//            if (UIDevice.Current.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
//            {
//                // On iPad, you need to present from a source view or rect
//                var window = UIApplication.SharedApplication.Windows.FirstOrDefault(w => w.IsKeyWindow);
//                var view = window?.RootViewController?.View;
//                if (view != null)
//                {
//                    printController.PresentFromRectInView(new CoreGraphics.CGRect(0,0,1,1), view, true, (controller, completed, error) => {
//                        if (error != null)
//                        {
//                            Console.WriteLine($"Print Error: {error.LocalizedDescription}");
//                        }
//                    });
//                } else {
//                    Console.WriteLine("Error: Could not find a view to present print controller on iPad.");
//                    await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Error", "Failed to present print dialog.", "OK");
//                }
//            }
//            else
//            {
//                // On iPhone, present modally
//                printController.Present(true, (controller, completed, error) => {
//                    if (error != null)
//                    {
//                        Console.WriteLine($"Print Error: {error.LocalizedDescription}");
//                    }
//                });
//            }
//#else
//            await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Not Supported", "Printing is not supported on this platform.", "OK");
//#endif
//        }
//    }

//#if ANDROID
//    // Custom PrintDocumentAdapter for Android
//    public class PdfPrintDocumentAdapter : PrintDocumentAdapter
//    {
//        private byte[] _pdfBytes;
//        private string _documentName;

//        public PdfPrintDocumentAdapter(byte[] pdfBytes, string documentName)
//        {
//            _pdfBytes = pdfBytes;
//            _documentName = documentName;
//        }

//        public override void OnLayout(PrintAttributes oldAttributes, PrintAttributes newAttributes, CancellationSignal cancellationSignal, LayoutResultCallback callback, Bundle extras)
//        {
//            if (cancellationSignal.IsCancellationRequested)
//            {
//                callback.OnLayoutCancelled();
//                return;
//            }

//            // Create a PrintDocumentInfo with the total page count
//            var info = new PrintDocumentInfo.Builder(_documentName)
//                .SetContentType(PrintContentType.Document)
//                .SetPageCount(1) // Assuming one page for simplicity, you might need to parse PDF for actual page count
//                .Build();

//            callback.OnLayoutFinished(info, oldAttributes != newAttributes);
//        }

//        public override void OnWrite(PageRange[] pages, ParcelFileDescriptor destination, CancellationSignal cancellationSignal, WriteResultCallback callback)
//        {
//            if (cancellationSignal.IsCancellationRequested)
//            {
//                callback.OnWriteCancelled();
//                return;
//            }

//            using (var output = new FileOutputStream(destination.FileDescriptor))
//            {
//                try
//                {
//                    output.Write(_pdfBytes);
//                    callback.OnWriteFinished(pages);
//                }
//                catch (Java.IO.IOException e)
//                {
//                    callback.OnWriteFailed(e.ToString());
//                }
//            }
//        }
//    }
//#endif
//}