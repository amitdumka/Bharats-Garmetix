// Services/PdfPrintService.cs 
using Bharat.ToolKits.Helpers;
using Bharat.ToolKits.Notifications;
using Garmetix.Reports.InvoicePrinter.Models; 
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;

// PdfService.cs
using PointF = Syncfusion.Drawing.PointF;
// PdfService.cs
using RectangleF = Syncfusion.Drawing.RectangleF;
// Helpers/AmountInWordsHelper.cs


namespace Garmetix.Reports.InvoicePrinter.Services
{
    public class PdfPrintService : IPrintService
    {

        
        public async Task CreateSaleInvoiceAsync(InvoiceModel invoice, bool isA4Size=false, bool duplicate=false)
        {
            try
            {
                // Create a new PDF document.
                PdfDocument document = new PdfDocument();

                // Set page size to A5
                if (isA4Size)
                {
                    document.PageSettings.Size = PdfPageSize.A4;
                    document.PageSettings.Margins.All = 25;
                }
                else
                {
                    document.PageSettings.Size = PdfPageSize.A5;
                    document.PageSettings.Margins.All = 10;
                }
                
                // Add a page to the document.
                PdfPage page = document.Pages.Add();
                //Page Area
                float pageWidth = page.GetClientSize().Width;
                float pageHeight = page.GetClientSize().Height;
                var bounds = new RectangleF(0, 0, pageWidth, pageHeight);

                // Get the graphics object for the page.
                PdfGraphics graphics = page.Graphics;

                // Define fonts
                PdfFont titleFont = new PdfStandardFont(PdfFontFamily.Helvetica, 16, PdfFontStyle.Bold);
                PdfFont subTitleFont = new PdfStandardFont(PdfFontFamily.Helvetica, 12, PdfFontStyle.Bold);

                PdfFont headerFont = new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Bold);
                PdfFont normalFont = new PdfStandardFont(PdfFontFamily.Helvetica, 9);
                PdfFont boldFont = new PdfStandardFont(PdfFontFamily.Helvetica, 9 ,PdfFontStyle.Bold);
                PdfFont smallFont = new PdfStandardFont(PdfFontFamily.Helvetica, 8);

                // Define colors and pens
                PdfBrush blackBrush = PdfBrushes.Black;
                PdfPen linePen = new PdfPen(PdfBrushes.Gray, 0.5f);

                //PdfBrush blackBrush = new PdfSolidBrush(new PdfColor(0, 0, 0));
                PdfBrush grayBrush = new PdfSolidBrush(new PdfColor(100, 100, 100));
                PdfBrush tableHeaderBrush = new PdfSolidBrush(new PdfColor(230, 230, 230));
                PdfBrush tableEvenRowBrush = new PdfSolidBrush(new PdfColor(245, 245, 245));

                // -------- HEADER SECTION --------
                float yPos = 0;
                graphics.DrawRectangle(blackBrush, new RectangleF(0, yPos, page.GetClientSize().Width, 50));
                graphics.DrawString("TAX INVOICE", subTitleFont, blackBrush, new PointF(page.GetClientSize().Width / 2, yPos), new PdfStringFormat(PdfTextAlignment.Center));
                yPos += 30;

                PdfGrid companyGrid = new PdfGrid();
                PdfGrid customerGrid = new PdfGrid();
                PdfGrid itemGrid = new PdfGrid();
                PdfGrid taxGrid = new PdfGrid();

                // ------- COMPANY SECTION --------
                companyGrid.Style.CellPadding = new PdfPaddings(2, 2, 2, 2);
                companyGrid.Style.BorderOverlapStyle = PdfBorderOverlapStyle.Overlap;
                companyGrid.Style.Font = normalFont;

                companyGrid.Columns.Add(2);
                companyGrid.Columns[0].Width = bounds.Width * 0.60f; // Adjust column width relative to voucher bounds
                companyGrid.Columns[1].Width = bounds.Width * 0.40f;

                // Company Name row
                var row1 = companyGrid.Rows.Add();
                row1.Cells[0].Value =invoice.SellerInfo.Name;
                row1.Cells[0].Style.Font = titleFont; //Reduce font size if needed
                row1.Cells[0].Style.TextPen = new PdfPen(PdfBrushes.DarkSlateBlue, 0.5f);
                row1.Cells[0].RowSpan = 2; // Span two rows for better visual spacing
                row1.Cells[0].Style.StringFormat = new PdfStringFormat { LineAlignment = PdfVerticalAlignment.Middle };

                // Voucher Number row
                row1.Cells[1].Value = $"{invoice.InvoiceNumber}";
                row1.Cells[1].Style.Font = boldFont;
                row1.Cells[1].Style.TextBrush = PdfBrushes.DarkGreen;
                row1.Cells[1].Style.StringFormat = new PdfStringFormat { LineAlignment = PdfVerticalAlignment.Top, Alignment = PdfTextAlignment.Left };

                // Date row
                var row2 = companyGrid.Rows.Add();
                row2.Cells[1].Value = $"Date: {invoice.IssueDate:dd-MMM-yyyy}\nTime: {invoice.IssueDate:HH:mm:ss}";
                row2.Cells[1].Style.StringFormat = new PdfStringFormat { LineAlignment = PdfVerticalAlignment.Middle, Alignment = PdfTextAlignment.Center };

                row2.Cells[0].Value = $"{invoice.SellerInfo.AddressLine1} \n{invoice.SellerInfo.AddressLine2}\t State:{invoice.SellerInfo.PlaceOfSupply}\nGSTIN:{invoice.SellerInfo.Gstin}\t\t\t\t\t Phone:{invoice.SellerInfo.Phone}";
                row2.Cells[0].Style.Font = normalFont;
                row2.Cells[0].Style.StringFormat = new PdfStringFormat { LineAlignment = PdfVerticalAlignment.Top, Alignment = PdfTextAlignment.Right };

                // Remove all borders for this grid to give a cleaner look.
                companyGrid.Rows[0].Cells[0].Style.Borders.Bottom = new PdfPen(PdfColor.Empty);
                companyGrid.Rows[1].Cells[0].Style.Borders.Top = new PdfPen(PdfColor.Empty);

                // Draw info grid. Assuming it takes up a fixed height if layoutResult is not returned.
                companyGrid.Draw(graphics, new RectangleF(bounds.X, bounds.Y + yPos, bounds.Width, bounds.Height - yPos));
                yPos += 52; // Estimate height for 2 rows + padding
               
                // ------- Customer SECTION --------

                graphics.DrawRectangle(blackBrush, new RectangleF(0, yPos, page.GetClientSize().Width, 150));
                graphics.DrawString("Bill To", headerFont, blackBrush, new PointF(page.GetClientSize().Width / 2, yPos), new PdfStringFormat(PdfTextAlignment.Left));

                //TODO: Section for Shio To need to be enabled for B2B invoice
                //TODO: if B2B or bill to customer will gst number make it page A4, else page A5
                //TODO: graphics.DrawString("Ship To", headerFont, blackBrush, new PointF(page.GetClientSize().Width / 2, yPos + 20), new PdfStringFormat(PdfTextAlignment.Left));

                //Customer Name
                graphics.DrawString("Name: " + invoice.BuyerInfo.Name, normalFont, blackBrush, new PointF(10, yPos + 40), new PdfStringFormat(PdfTextAlignment.Left));
                graphics.DrawString("Address: " + invoice.BuyerInfo.AddressLine1+"\n"+invoice.BuyerInfo.AddressLine2, normalFont, blackBrush, new PointF(10, yPos + 60), new PdfStringFormat(PdfTextAlignment.Left));

                //Customer Details
                graphics.DrawString("Phone: " + invoice.BuyerInfo.Phone, normalFont, blackBrush, new PointF(10, yPos + 40), new PdfStringFormat(PdfTextAlignment.Left));
                graphics.DrawString("GSTIN: " + invoice.BuyerInfo.Gstin, normalFont, blackBrush, new PointF(10, yPos + 40), new PdfStringFormat(PdfTextAlignment.Center));
                graphics.DrawString("State: " + invoice.BuyerInfo.PlaceOfSupply, normalFont, blackBrush, new PointF(10, yPos + 40), new PdfStringFormat(PdfTextAlignment.Center));
                

                //// ------- Items SECTION --------
                itemGrid.Headers.Add(1);
                PdfGridRow header = itemGrid.Headers[0];
                // Set column headers
                header.Cells[0].Value = "# ";
                header.Cells[1].Value = "Item Name";
                header.Cells[2].Value = "HSN";
                header.Cells[3].Value = "Qty";
                header.Cells[4].Value = "Unit";
                header.Cells[5].Value = "Rate";
                header.Cells[6].Value = "Disc.";
                header.Cells[7].Value = "Amount";

                 // Apply styles to the grid
                PdfGridStyle gridStyle = new PdfGridStyle();
                gridStyle.Font = normalFont;
                gridStyle.CellPadding = new PdfPaddings(5, 5, 5, 5);

                PdfGridRowStyle headerStyle = new PdfGridRowStyle();
                headerStyle.Font = headerFont;
                //headerStyle.BackgroundBrush = grayBrush;//Light Gray
                headerStyle.TextBrush = PdfBrushes.Black;
                // headerStyle.StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                foreach (PdfGridCell cell in header.Cells)
                {
                    cell.Style.Font = headerFont;
                    cell.Style.BackgroundBrush = tableHeaderBrush;
                    cell.Style.TextBrush = blackBrush;
                    cell.Style.Borders.All = new PdfPen(blackBrush, 0.5f);
                    cell.StringFormat = new PdfStringFormat { Alignment = PdfTextAlignment.Center, LineAlignment = PdfVerticalAlignment.Middle };
                    cell.Style.CellPadding = new PdfPaddings(0, 0, 0, 5); ;
                }
                itemGrid.Style = gridStyle;
                itemGrid.Headers[0].Style = headerStyle;

                // Set column widths
                itemGrid.Columns[0].Width = 25; // S.No
                itemGrid.Columns[1].Width = 130; // Description
                itemGrid.Columns[2].Width = 50;  // HSN
                itemGrid.Columns[3].Width = 30;  // Qty
                itemGrid.Columns[4].Width = 40;  // Rate
                itemGrid.Columns[5].Width = 30;  // Discount
                itemGrid.Columns[6].Width = 30;  // Total
                itemGrid.Columns[6].Width = 60;  // Total

                // Align columns
                ((PdfGridColumn)itemGrid.Columns[3]).Format = new PdfStringFormat { Alignment = PdfTextAlignment.Center };
                ((PdfGridColumn)itemGrid.Columns[4]).Format = new PdfStringFormat { Alignment = PdfTextAlignment.Right };
                ((PdfGridColumn)itemGrid.Columns[5]).Format = new PdfStringFormat { Alignment = PdfTextAlignment.Center };
                ((PdfGridColumn)itemGrid.Columns[6]).Format = new PdfStringFormat { Alignment = PdfTextAlignment.Center };
                ((PdfGridColumn)itemGrid.Columns[6]).Format = new PdfStringFormat { Alignment = PdfTextAlignment.Right };
                // Draw the grid
                PdfGridLayoutResult gridLayoutResult = itemGrid.Draw(page, new PointF(0, yPos));
                yPos = gridLayoutResult.Bounds.Bottom + 20;


                //// ------- TAX SECTION --------
                //// ------- TOTAL SECTION --------
                //// ------- SIGNATURE SECTION --------
                //// ------- TERMS AND CONDITIONS SECTION --------
                graphics.DrawRectangle(blackBrush, new RectangleF(0, yPos, page.GetClientSize().Width, 100));
                string terms = "Terms and Conditions\n Thanks for doing bussiness with us. \nPlease visit again!";   
                graphics.DrawString(terms, smallFont, blackBrush, new PointF(page.GetClientSize().Width / 2, yPos), new PdfStringFormat(PdfTextAlignment.Center));
                //// ------- FOOTER SECTION --------

            }
            catch (Exception ex)
            {

                // Basic error handling
                Console.WriteLine($"Error generating PDF: {ex.Message}");
                await Notify.ShowError("Error", "Could not generate or print the invoice.");
            }

        }


        public async Task CreateAndPrintInvoiceAsync(InvoiceModel invoice)
        {
            try
            {
                // Create a new PDF document.
                PdfDocument document = new PdfDocument();

                // Set page size to A5
                document.PageSettings.Size = PdfPageSize.A5;
                document.PageSettings.Margins.All = 10;

                // Add a page to the document.
                PdfPage page = document.Pages.Add();

                // Get the graphics object for the page.
                PdfGraphics graphics = page.Graphics;

                // Define fonts
                PdfFont titleFont = new PdfStandardFont(PdfFontFamily.Helvetica, 16, PdfFontStyle.Bold);
                PdfFont headerFont = new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Bold);
                PdfFont normalFont = new PdfStandardFont(PdfFontFamily.Helvetica, 9);
                PdfFont smallFont = new PdfStandardFont(PdfFontFamily.Helvetica, 8);

                // Define colors and pens
                PdfBrush blackBrush = PdfBrushes.Black;
                PdfPen linePen = new PdfPen(PdfBrushes.Gray, 0.5f);

                //PdfBrush blackBrush = new PdfSolidBrush(new PdfColor(0, 0, 0));
                PdfBrush grayBrush = new PdfSolidBrush(new PdfColor(100, 100, 100));
                PdfBrush tableHeaderBrush = new PdfSolidBrush(new PdfColor(230, 230, 230));
                PdfBrush tableEvenRowBrush = new PdfSolidBrush(new PdfColor(245, 245, 245));

                // -------- HEADER SECTION --------
                float yPos = 0;
                graphics.DrawString("TAX INVOICE", titleFont, blackBrush, new PointF(page.GetClientSize().Width / 2, yPos), new PdfStringFormat(PdfTextAlignment.Center));
                yPos += 30;

                // -------- SELLER AND BUYER INFO --------
                float halfWidth = page.GetClientSize().Width / 2;

                // Seller Info (Billed From)
                graphics.DrawString("Billed From:", headerFont, blackBrush, new PointF(0, yPos));
                yPos += 15;
                graphics.DrawString(invoice.SellerInfo.Name, normalFont, blackBrush, new PointF(0, yPos));
                yPos += 12;
                graphics.DrawString(invoice.SellerInfo.AddressLine1, smallFont, blackBrush, new PointF(0, yPos));
                yPos += 12;
                graphics.DrawString(invoice.SellerInfo.AddressLine2, smallFont, blackBrush, new PointF(0, yPos));
                yPos += 12;
                graphics.DrawString($"GSTIN: {invoice.SellerInfo.Gstin}", smallFont, blackBrush, new PointF(0, yPos));
                yPos += 12;
                graphics.DrawString($"Phone: {invoice.SellerInfo.Phone}", smallFont, blackBrush, new PointF(0, yPos));

                // Buyer Info (Billed To) - Reset Y for the right column
                float rightColumnY = yPos - (12 * 4) - 15;
                graphics.DrawString("Billed To:", headerFont, blackBrush, new PointF(halfWidth, rightColumnY));
                rightColumnY += 15;
                graphics.DrawString(invoice.BuyerInfo.Name, normalFont, blackBrush, new PointF(halfWidth, rightColumnY));
                rightColumnY += 12;
                graphics.DrawString(invoice.BuyerInfo.AddressLine1, smallFont, blackBrush, new PointF(halfWidth, rightColumnY));
                rightColumnY += 12;
                graphics.DrawString(invoice.BuyerInfo.AddressLine2, smallFont, blackBrush, new PointF(halfWidth, rightColumnY));
                rightColumnY += 12;
                graphics.DrawString($"GSTIN: {invoice.BuyerInfo.Gstin}", smallFont, blackBrush, new PointF(halfWidth, rightColumnY));

                yPos += 20; // Move down past the address blocks

                // -------- INVOICE METADATA --------
                graphics.DrawLine(linePen, new PointF(0, yPos), new PointF(page.GetClientSize().Width, yPos));
                yPos += 10;

                graphics.DrawString($"Invoice #: {invoice.InvoiceNumber}", headerFont, blackBrush, new PointF(0, yPos));
                graphics.DrawString($"Date: {invoice.IssueDate:dd MMM yyyy}", headerFont, blackBrush, new PointF(halfWidth, yPos), new PdfStringFormat(PdfTextAlignment.Left));

                yPos += 20;
                graphics.DrawLine(linePen, new PointF(0, yPos), new PointF(page.GetClientSize().Width, yPos));
                yPos += 10;

                // -------- ITEMS GRID --------
                PdfGrid pdfGrid = new PdfGrid();
                pdfGrid.DataSource = invoice.Items.Select((item, index) => new
                {
                    SNo = index + 1,
                    ItemDetails = $"{item.ItemName}\nStyle: {item.StyleCode} | Barcode: {item.Barcode}",
                    item.HsnCode,
                    item.Quantity,
                    Rate = $"₹{item.Rate:N2}",
                    Discount = $"{item.DiscountPercentage}%",
                    Total = $"₹{item.Total:N2}"
                }).ToList();

                pdfGrid.Headers.Add(1);
                PdfGridRow header = pdfGrid.Headers[0];

                // Set column headers
                header.Cells[0].Value = "S.No";
                header.Cells[1].Value = "Item Description";
                header.Cells[2].Value = "HSN";
                header.Cells[3].Value = "Qty";
                header.Cells[4].Value = "Rate";
                header.Cells[5].Value = "Disc.";
                header.Cells[6].Value = "Amount";

                // Apply styles to the grid
                PdfGridStyle gridStyle = new PdfGridStyle();
                gridStyle.Font = normalFont;
                gridStyle.CellPadding = new PdfPaddings(5, 5, 5, 5);

                PdfGridRowStyle headerStyle = new PdfGridRowStyle();

                headerStyle.Font = headerFont;
                //headerStyle.BackgroundBrush = grayBrush;//Light Gray
                headerStyle.TextBrush = PdfBrushes.Black;
                // headerStyle.StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
                foreach (PdfGridCell cell in header.Cells)
                {
                    cell.Style.Font = headerFont;
                    cell.Style.BackgroundBrush = tableHeaderBrush;
                    cell.Style.TextBrush = blackBrush;
                    cell.Style.Borders.All = new PdfPen(blackBrush, 0.5f);
                    cell.StringFormat = new PdfStringFormat { Alignment = PdfTextAlignment.Center, LineAlignment = PdfVerticalAlignment.Middle };
                    cell.Style.CellPadding = new PdfPaddings(0, 0, 0, 5); ;
                }
                pdfGrid.Style = gridStyle;
                pdfGrid.Headers[0].Style = headerStyle;

                // Set column widths
                pdfGrid.Columns[0].Width = 25; // S.No
                pdfGrid.Columns[1].Width = 150; // Description
                pdfGrid.Columns[2].Width = 50;  // HSN
                pdfGrid.Columns[3].Width = 30;  // Qty
                pdfGrid.Columns[4].Width = 40;  // Rate
                pdfGrid.Columns[5].Width = 30;  // Discount
                pdfGrid.Columns[6].Width = 60;  // Total

                // Align columns
                ((PdfGridColumn)pdfGrid.Columns[3]).Format = new PdfStringFormat { Alignment = PdfTextAlignment.Center };
                ((PdfGridColumn)pdfGrid.Columns[4]).Format = new PdfStringFormat { Alignment = PdfTextAlignment.Right };
                ((PdfGridColumn)pdfGrid.Columns[5]).Format = new PdfStringFormat { Alignment = PdfTextAlignment.Center };
                ((PdfGridColumn)pdfGrid.Columns[6]).Format = new PdfStringFormat { Alignment = PdfTextAlignment.Right };


                // Draw the grid
                PdfGridLayoutResult gridLayoutResult = pdfGrid.Draw(page, new PointF(0, yPos));
                yPos = gridLayoutResult.Bounds.Bottom + 20;


                // -------- TOTALS SECTION --------
                float totalsX = halfWidth + 50;
                float valueX = page.GetClientSize().Width - 70;

                graphics.DrawString("Subtotal:", normalFont, blackBrush, new PointF(totalsX, yPos));
                graphics.DrawString($"₹{invoice.SubTotal:N2}", normalFont, blackBrush, new PointF(valueX, yPos), new PdfStringFormat(PdfTextAlignment.Right));
                yPos += 15;

                if (invoice.IgstAmount > 0)
                {
                    graphics.DrawString($"IGST ({invoice.IgstRate}%):", normalFont, blackBrush, new PointF(totalsX, yPos));
                    graphics.DrawString($"₹{invoice.IgstAmount:N2}", normalFont, blackBrush, new PointF(valueX, yPos), new PdfStringFormat(PdfTextAlignment.Right));
                    yPos += 15;
                }
                else
                {
                    graphics.DrawString($"CGST ({invoice.CgstRate}%):", normalFont, blackBrush, new PointF(totalsX, yPos));
                    graphics.DrawString($"₹{invoice.CgstAmount:N2}", normalFont, blackBrush, new PointF(valueX, yPos), new PdfStringFormat(PdfTextAlignment.Right));
                    yPos += 15;
                    graphics.DrawString($"SGST ({invoice.SgstRate}%):", normalFont, blackBrush, new PointF(totalsX, yPos));
                    graphics.DrawString($"₹{invoice.SgstAmount:N2}", normalFont, blackBrush, new PointF(valueX, yPos), new PdfStringFormat(PdfTextAlignment.Right));
                    yPos += 15;
                }

                graphics.DrawLine(linePen, new PointF(totalsX - 10, yPos), new PointF(page.GetClientSize().Width, yPos));
                yPos += 10;

                graphics.DrawString("Grand Total:", headerFont, blackBrush, new PointF(totalsX, yPos));
                graphics.DrawString($"₹{invoice.GrandTotal:N2}", headerFont, blackBrush, new PointF(valueX, yPos), new PdfStringFormat(PdfTextAlignment.Right));
                yPos += 25;

                // -------- AMOUNT IN WORDS --------
                string amountInWords = AmountInWordsHelper.ConvertToWords((long)invoice.GrandTotal);
                graphics.DrawString($"Amount in Words: {amountInWords} Only", smallFont, blackBrush, new RectangleF(0, yPos, page.GetClientSize().Width, 30));
                yPos += 30;

                // -------- FOOTER / TERMS --------
                graphics.DrawString("Terms & Conditions:\n1. Goods once sold will not be taken back.\n2. All disputes are subject to local jurisdiction.", smallFont, blackBrush, new PointF(0, yPos));

                graphics.DrawString("Authorized Signatory", normalFont, blackBrush, new PointF(page.GetClientSize().Width - 100, yPos + 60));


                // Save the document into a stream.
                using MemoryStream ms = new MemoryStream();
                document.Save(ms);
                ms.Position = 0;

                // Save the stream to a file and share it
                string filePath = Path.Combine(FileSystem.CacheDirectory, $"Invoice_{DateTime.Now.ToString("yyyyMMddHHmmss")}.pdf");
                File.WriteAllBytes(filePath, ms.ToArray());
                // Open the PDF using default system viewer
                await Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(filePath)
                });
                //TODO: Share the file
                //await Share.Default.RequestAsync(new ShareFileRequest
                //{
                //    Title = $"Invoice {invoice.InvoiceNumber}",
                //    File = new ShareFile(filePath, "application/pdf")
                //});

            }
            catch (Exception ex)
            {
                // Basic error handling
                Console.WriteLine($"Error generating PDF: {ex.Message}");
                await Notify.ShowError("Error", "Could not generate or print the invoice.");
            }
        }
    }
}
