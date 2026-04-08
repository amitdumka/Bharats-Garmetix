// Services/PdfService.cs
using Garmetix.Reports.InvoicePrinter.V2.Models;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Barcode;
using Syncfusion.Pdf.Graphics;
using Color = Syncfusion.Drawing.Color;
using PointF = Syncfusion.Drawing.PointF;

namespace Garmetix.Reports.InvoicePrinter.V2.Services
{
    public interface IPdfService
    {
        Task<string> GenerateInvoicePdf(Invoice invoice);
    }

    public class PdfService : IPdfService
    {
        public async Task<string> GenerateInvoicePdf(Invoice invoice)
        {
            // Create a new PDF document
            using (PdfDocument document = new PdfDocument())
            {
                // Set page size to A5 Landscape
                document.PageSettings.Size = PdfPageSize.A5;
                document.PageSettings.Orientation = PdfPageOrientation.Landscape;
                document.PageSettings.Margins.All = 20; // Set margins

                // Add a page to the document
                PdfPage page = document.Pages.Add();
                PdfGraphics graphics = page.Graphics;

                // Load a font (Inter is not directly available, using a standard font or embedding custom)
                // For a "beautiful layout", consider embedding a custom font if needed.
                // Example for embedding a font:
                // byte[] fontData = GetFontData("GSTInvoiceApp.Resources.Fonts.OpenSans-Regular.ttf"); // Adjust path
                // PdfFont font = new PdfTrueTypeFont(fontData, 10);
                PdfFont headerFont = new PdfStandardFont(PdfFontFamily.Helvetica, 14, PdfFontStyle.Bold);
                PdfFont normalFont = new PdfStandardFont(PdfFontFamily.Helvetica, 8);
                PdfFont boldFont = new PdfStandardFont(PdfFontFamily.Helvetica, 8, PdfFontStyle.Bold);

                PdfBrush blackBrush = new PdfSolidBrush(Color.Black);
                PdfPen borderPen = new PdfPen(Color.Black, 0.5f);

                float currentY = 20; // Initial Y position for content

                // --- Invoice Header ---
                // Fix for CS1061: Replace the problematic line with an alternative approach to center the text without using MeasureString.
                graphics.DrawString(
                    "TAX INVOICE",
                    headerFont,
                    blackBrush,
                    new PointF((page.Graphics.ClientSize.Width - 100) / 2, currentY) // Adjusted to approximate centering
                );
               // graphics.DrawString("TAX INVOICE", headerFont, blackBrush, new PointF(page.Graphics.ClientSize.Width / 2 - graphics.MeasureString("TAX INVOICE", headerFont).Width / 2, currentY));
                currentY += 25;

                // Company Details (Seller)
                graphics.DrawString(invoice.Seller.Name, headerFont, blackBrush, new PointF(20, currentY));
                currentY += 15;
                graphics.DrawString($"Address: {invoice.Seller.Address}", normalFont, blackBrush, new PointF(20, currentY));
                currentY += 10;
                graphics.DrawString($"Phone: {invoice.Seller.ContactNo}", normalFont, blackBrush, new PointF(20, currentY));
                currentY += 10;
                graphics.DrawString($"Email: {invoice.Seller.Email}", normalFont, blackBrush, new PointF(20, currentY));
                currentY += 10;
                graphics.DrawString($"GSTIN: {invoice.Seller.GSTIN}", normalFont, blackBrush, new PointF(20, currentY));
                currentY += 10;
                graphics.DrawString($"State: {invoice.Seller.State}", normalFont, blackBrush, new PointF(20, currentY));
                currentY += 20;


                // Invoice Info (Right side)
                float invoiceInfoX = page.Graphics.ClientSize.Width - 150; // Adjust as needed
                graphics.DrawString($"Invoice No: {invoice.InvoiceNo}", normalFont, blackBrush, new PointF(invoiceInfoX, 65));
                graphics.DrawString($"Date: {invoice.InvoiceDate.ToString("dd/MM/yyyy")}", normalFont, blackBrush, new PointF(invoiceInfoX, 80));
                graphics.DrawString($"Place of Supply: {invoice.PlaceOfSupply}", normalFont, blackBrush, new PointF(invoiceInfoX, 95));


                // Billing Details (Buyer)
                graphics.DrawString("Bill To:", boldFont, blackBrush, new PointF(20, currentY));
                currentY += 15;
                graphics.DrawString(invoice.Buyer.Name, normalFont, blackBrush, new PointF(20, currentY));
                currentY += 10;
                graphics.DrawString(invoice.Buyer.Address, normalFont, blackBrush, new PointF(20, currentY));
                currentY += 10;
                graphics.DrawString($"Contact No: {invoice.Buyer.ContactNo}", normalFont, blackBrush, new PointF(20, currentY));
                currentY += 10;
                graphics.DrawString($"GSTIN: {invoice.Buyer.GSTIN}", normalFont, blackBrush, new PointF(20, currentY));
                currentY += 20;

                // --- Item Table Header ---
                PdfGraphicsState state = graphics.Save();
                graphics.SetTransparency(0.9f); // Slightly transparent background for header
                graphics.DrawRectangle(new PdfSolidBrush(new PdfColor(220, 220, 220)), new RectangleF(15, currentY, page.Graphics.ClientSize.Width - 30, 20));
                graphics.Restore(state);

                // Table headers
                float[] columnWidths = { 25, 80, 50, 40, 40, 40, 40, 45, 50, 50 }; // Adjust widths as needed
                string[] headers = { "S.No", "Item Name", "HSN", "Rate", "Qty", "Disc.", "Line Total", "Taxable Amt.", "CGST", "SGST" };
                float currentX = 20;

                for (int i = 0; i < headers.Length; i++)
                {
                    graphics.DrawString(headers[i], boldFont, blackBrush, currentX, currentY + 5);
                    currentX += columnWidths[i];
                }
                currentY += 25;

                // --- Item Table Rows ---
                foreach (var item in invoice.Items)
                {
                    currentX = 20;
                    graphics.DrawString(item.SNo.ToString(), normalFont, blackBrush, currentX, currentY);
                    currentX += columnWidths[0];
                    graphics.DrawString(item.ItemName, normalFont, blackBrush, currentX, currentY);
                    currentX += columnWidths[1];
                    graphics.DrawString(item.HSNCode, normalFont, blackBrush, currentX, currentY);
                    currentX += columnWidths[2];
                    graphics.DrawString(item.Rate.ToString("F2"), normalFont, blackBrush, currentX, currentY);
                    currentX += columnWidths[3];
                    graphics.DrawString(item.Quantity.ToString(), normalFont, blackBrush, currentX, currentY);
                    currentX += columnWidths[4];
                    graphics.DrawString(item.Discount.ToString("F2"), normalFont, blackBrush, currentX, currentY);
                    currentX += columnWidths[5];
                    graphics.DrawString(item.LineTotal.ToString("F2"), normalFont, blackBrush, currentX, currentY);
                    currentX += columnWidths[6];
                    graphics.DrawString(item.TaxableAmount.ToString("F2"), normalFont, blackBrush, currentX, currentY);
                    currentX += columnWidths[7];
                    graphics.DrawString($"{item.CGSTAmt.ToString("F2")} ({item.CGSTPercentage}%)", normalFont, blackBrush, currentX, currentY);
                    currentX += columnWidths[8];
                    graphics.DrawString($"{item.SGSTAmt.ToString("F2")} ({item.SGSTPercentage}%)", normalFont, blackBrush, currentX, currentY);
                    currentY += 15;

                    // Add barcode if available
                    if (!string.IsNullOrEmpty(item.Barcode))
                    {
                        PdfCode128Barcode barcode = new()
                        {
                            Text = item.Barcode,
                            BarHeight = 15,
                            TextDisplayLocation = TextLocation.None // Hide text below barcode
                        };
                        barcode.Draw(graphics, new PointF(currentX, currentY - 10)); // Adjust position
                    }
                     // Add style code if available (e.g., as a small note below item name)
                    if (!string.IsNullOrEmpty(item.StyleCode))
                    {
                        graphics.DrawString($"Style: {item.StyleCode}", new PdfStandardFont(PdfFontFamily.Helvetica, 7), blackBrush, 20 + columnWidths[0] + 5, currentY - 5);
                    }
                }

                currentY += 10;
                graphics.DrawLine(borderPen, 15, currentY, page.Graphics.ClientSize.Width - 15, currentY); // Line after items
                currentY += 5;

                // --- Totals Section ---
                float totalsX = page.Graphics.ClientSize.Width - 150; // Align totals to right

                graphics.DrawString("Sub Total:", boldFont, blackBrush, new PointF(totalsX - 60, currentY));
                graphics.DrawString(invoice.SubTotal.ToString("F2"), normalFont, blackBrush, new PointF(totalsX, currentY));
                currentY += 15;

                graphics.DrawString("Total Tax:", boldFont, blackBrush, new PointF(totalsX - 60, currentY));
                graphics.DrawString(invoice.TotalTaxAmount.ToString("F2"), normalFont, blackBrush, new PointF(totalsX, currentY));
                currentY += 15;

                graphics.DrawString("Round Off:", boldFont, blackBrush, new PointF(totalsX - 60, currentY));
                graphics.DrawString(invoice.RoundOff.ToString("F2"), normalFont, blackBrush, new PointF(totalsX, currentY));
                currentY += 15;

                graphics.DrawString("GRAND TOTAL:", headerFont, blackBrush, new PointF(totalsX - 100, currentY));
                graphics.DrawString(invoice.GrandTotal.ToString("F2"), headerFont, blackBrush, new PointF(totalsX, currentY));
                currentY += 25;

                // Amount in words
                graphics.DrawString($"Amount in Words: {invoice.AmountInWords}", normalFont, blackBrush, new PointF(20, currentY));
                currentY += 20;

                // Terms and Conditions
                graphics.DrawString("Terms & Conditions:", boldFont, blackBrush, new PointF(20, currentY));
                currentY += 15;
                graphics.DrawString("Thanks for doing business with us!", normalFont, blackBrush, new PointF(20, currentY));
                currentY += 30;

                // Authorized Signatory
                graphics.DrawString("For " + invoice.Seller.Name + ":", boldFont, blackBrush, new PointF(page.Graphics.ClientSize.Width - 150, currentY));
                currentY += 40;
                graphics.DrawLine(borderPen, page.Graphics.ClientSize.Width - 150, currentY, page.Graphics.ClientSize.Width - 50, currentY);
                graphics.DrawString("Authorized Signatory", normalFont, blackBrush, new PointF(page.Graphics.ClientSize.Width - 150, currentY + 5));

                // Save the document to a MemoryStream
                MemoryStream stream = new MemoryStream();
                document.Save(stream);
                stream.Position = 0;
                // Save the stream to a file and share it
                string filePath = Path.Combine(FileSystem.CacheDirectory, $"Invoice_{DateTime.Now.ToString("yyyyMMddHHmmss")}.pdf");
                File.WriteAllBytes(filePath, stream.ToArray());
                document.Close(true);
                // Open the PDF using default system viewer
                //await Launcher.OpenAsync(new OpenFileRequest
                //{
                //    File = new ReadOnlyFile(filePath)
                //});
                return filePath;
            }
        }

        // Helper to embed fonts - place your .ttf files in Resources/Fonts and set Build Action to EmbeddedResource
        // private byte[] GetFontData(string fontResourceName)
        // {
        //     Assembly assembly = typeof(PdfService).GetTypeInfo().Assembly;
        //     using (Stream stream = assembly.GetManifestResourceStream(fontResourceName))
        //     {
        //         if (stream == null)
        //         {
        //             throw new FileNotFoundException($"Font resource '{fontResourceName}' not found.");
        //         }
        //         byte[] fontData = new byte[stream.Length];
        //         stream.Read(fontData, 0, (int)stream.Length);
        //         return fontData;
        //     }
        // }
    }
}
