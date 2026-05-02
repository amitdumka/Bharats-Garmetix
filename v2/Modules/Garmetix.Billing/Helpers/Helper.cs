using Garmetix.Core.Models.Inventory;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Colors = QuestPDF.Helpers.Colors;
using IContainer = QuestPDF.Infrastructure.IContainer;
namespace Garmetix.Billing.Helpers
{
    public interface IPrintService
    {
        // Your existing thermal print method
        Task PrintReceiptAsync(byte[] receiptBytes);

        // NEW: Direct HTML Print method
        Task PrintHtmlAsync(string htmlContent, string documentName = "Invoice");
    }
    public static class PdfReceiptBuilder
    {
        // Premium Brand Colors
        static readonly string BrandDark = "#0F172A";    // Slate 900
        static readonly string BrandGray = "#64748B";    // Slate 500
        static readonly string LightBg = "#F8FAFC";      // Slate 50
        static readonly string BorderColor = "#E2E8F0";  // Slate 200
        static readonly string AccentColor = "#0284C7";  // Sky 600

        public static void Initialize()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public static string GenerateA5Pdf(Invoice invoice, IEnumerable<InvoiceItem> items, IEnumerable<PaymentDetail> payments)
        {
            Initialize();

            string fileName = $"{invoice.InvoiceNo}.pdf";
            string filePath = Path.Combine(Microsoft.Maui.Storage.FileSystem.CacheDirectory, fileName);

            string instagramUrl = "https://instagram.com/aadwikafashion";
            string googleMapsUrl = "https://maps.app.goo.gl/hwyprtc1DKmdPV757/review"; //"https://g.page/r/your-google-map-link/review";

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    // CHANGE: Set size to A5 Landscape
                    page.Size(PageSizes.A5.Landscape());

                    page.Margin(12, Unit.Millimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(9).FontFamily(QuestPDF.Helpers.Fonts.Georgia).FontColor(BrandDark));
                    //page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Georgia").FontColor(BrandDark));
                    page.Header().Element(header => ComposeHeader(header, invoice));
                    page.Content().Element(content => ComposeContent(content, invoice, items, payments));
                    page.Footer().Element(footer => ComposeFooter(footer, instagramUrl, googleMapsUrl));
                });
            })
            .GeneratePdf(filePath);

            return filePath;
        }

        private static void ComposeHeader(IContainer container, Invoice invoice)
        {
            container.PaddingBottom(10).Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text(StoreInfo.StoreName).FontSize(24).Black().LetterSpacing(0.05f);
                    column.Item().Text(StoreInfo.StoreAddress.Replace("\n", ", ")).FontSize(8).FontColor(BrandGray);
                    column.Item().Text($"{StoreInfo.ContactInfo}  •  GSTIN: {StoreInfo.GSTIN}").FontSize(8).FontColor(BrandGray);
                });

                row.ConstantItem(150).AlignRight().Column(column =>
                {
                    column.Item().Background(BrandDark).PaddingVertical(4).PaddingHorizontal(10).AlignCenter()
                        .Text("TAX INVOICE").FontSize(10).Bold().FontColor(Colors.White).LetterSpacing(0.05f);

                    column.Item().PaddingTop(5).Row(r => {
                        r.RelativeItem().Text("Invoice:").FontSize(8).FontColor(BrandGray);
                        r.AutoItem().Text(invoice.InvoiceNo).FontSize(8).SemiBold();
                    });
                    column.Item().Row(r => {
                        r.RelativeItem().Text("Date:").FontSize(8).FontColor(BrandGray);
                        r.AutoItem().Text($"{invoice.Date:dd MMM yyyy}").FontSize(8);
                    });
                });
            });
        }

        private static void ComposeContent(IContainer container, Invoice invoice, IEnumerable<InvoiceItem> items, IEnumerable<PaymentDetail> payments)
        {
            container.Column(column =>
            {
                // Customer Info Row
                column.Item().PaddingBottom(10).Background(LightBg).Padding(8).Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("BILLED TO").FontSize(7).Bold().FontColor(BrandGray);
                        col.Item().Text(invoice.CustomerName).FontSize(11).SemiBold();
                        col.Item().Text($"+91 {invoice.MobileNo}").FontSize(9);
                    });

                    if (!string.IsNullOrEmpty(invoice.Gstin))
                    {
                        row.RelativeItem().AlignRight().Column(col => {
                            col.Item().Text("CUSTOMER GSTIN").FontSize(7).Bold().FontColor(BrandGray);
                            col.Item().Text(invoice.Gstin).FontSize(9);
                        });
                    }
                });

                // Items Table - Wider columns for Landscape
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(25);  // #
                        columns.RelativeColumn(4);   // Item Description
                        columns.RelativeColumn(1);   // Qty
                        columns.RelativeColumn(1.5f); // Rate
                        columns.RelativeColumn(1.2f); // GST
                        columns.RelativeColumn(1.8f); // Total
                    });

                    table.Header(header =>
                    {
                        var headerStyle = TextStyle.Default.FontSize(8).Bold().FontColor(BrandGray);
                        header.Cell().BorderBottom(1).BorderColor(BorderColor).PaddingBottom(5).Text("#").Style(headerStyle);
                        header.Cell().BorderBottom(1).BorderColor(BorderColor).PaddingBottom(5).Text("ITEM DESCRIPTION").Style(headerStyle);
                        header.Cell().BorderBottom(1).BorderColor(BorderColor).PaddingBottom(5).AlignRight().Text("QTY").Style(headerStyle);
                        header.Cell().BorderBottom(1).BorderColor(BorderColor).PaddingBottom(5).AlignRight().Text("RATE").Style(headerStyle);
                        header.Cell().BorderBottom(1).BorderColor(BorderColor).PaddingBottom(5).AlignRight().Text("TAX").Style(headerStyle);
                        header.Cell().BorderBottom(1).BorderColor(BorderColor).PaddingBottom(5).AlignRight().Text("AMOUNT").Style(headerStyle);
                    });

                    int sNo = 1;
                    foreach (var item in items)
                    {
                        table.Cell().BorderBottom(1).BorderColor(BorderColor).PaddingVertical(5).Text(sNo++.ToString());
                        table.Cell().BorderBottom(1).BorderColor(BorderColor).PaddingVertical(5).Text(item.ProductName).SemiBold();
                        table.Cell().BorderBottom(1).BorderColor(BorderColor).PaddingVertical(5).AlignRight().Text(item.Quantity.ToString("0.##"));
                        //table.Cell().BorderBottom(1).BorderColor(BorderColor).PaddingVertical(5).AlignRight().Text(item.Quantity.ToString());
                        table.Cell().BorderBottom(1).BorderColor(BorderColor).PaddingVertical(5).AlignRight().Text($"₹ {item.Rate:F2}");
                        table.Cell().BorderBottom(1).BorderColor(BorderColor).PaddingVertical(5).AlignRight().Text($"{item.GstPercentage}%");
                        table.Cell().BorderBottom(1).BorderColor(BorderColor).PaddingVertical(5).AlignRight().Text($"₹ {item.TotalAmount:F2}").SemiBold();
                    }
                });

                // Bottom Section: Payments and Summary side-by-side
                column.Item().PaddingTop(10).Row(row =>
                {
                    // Left: Payments
                    row.RelativeItem().Column(pCol =>
                    {
                        pCol.Item().Text("PAYMENT SUMMARY").FontSize(7).Bold().FontColor(BrandGray);
                        if (payments != null && payments.Any())
                        {
                            foreach (var p in payments)
                            {
                                pCol.Item().PaddingTop(2).Text($"{p.Mode.ToUpper()} : ₹ {p.Amount:F2}").FontSize(8).SemiBold();
                            }
                        }
                        if (invoice.BalanceAmount > 0)
                        {
                            pCol.Item().PaddingTop(4).Text($"DUE AMOUNT: ₹ {invoice.BalanceAmount:F2}").FontSize(9).Bold().FontColor(Colors.Red.Medium);
                        }
                    });

                    row.ConstantItem(40); // Spacer

                    // Right: Calculation Summary
                    row.ConstantItem(160).Column(tCol =>
                    {
                        void AddSummaryRow(string label, string value, bool isBold = false)
                        {
                            tCol.Item().PaddingVertical(1).Row(r => {
                                r.RelativeItem().Text(label).FontSize(8).FontColor(isBold ? BrandDark : BrandGray).Style(isBold ? TextStyle.Default.Bold() : TextStyle.Default);
                                r.ConstantItem(70).AlignRight().Text(value).FontSize(isBold ? 10 : 8).Style(isBold ? TextStyle.Default.Bold() : TextStyle.Default);
                            });
                        }

                        AddSummaryRow("Subtotal", $"₹ {invoice.SubTotal:F2}");

                        decimal totalDiscount = invoice.TotalDiscount + invoice.GlobalDiscountAmount;
                        if (totalDiscount > 0) AddSummaryRow("Total Discount", $"- ₹ {totalDiscount:F2}");

                        if (invoice.IsInterStateSale)
                            AddSummaryRow("IGST", $"₹ {invoice.TotalTax:F2}");
                        else
                        {
                            decimal halfTax = invoice.TotalTax / 2m;
                            AddSummaryRow("CGST (2.5%)", $"₹ {halfTax:F2}");
                            AddSummaryRow("SGST (2.5%)", $"₹ {halfTax:F2}");
                        }

                        if (invoice.RoundOffAmount != 0) AddSummaryRow("Round Off", $"₹ {invoice.RoundOffAmount:F2}");

                        tCol.Item().PaddingVertical(4).LineHorizontal(1).LineColor(BrandDark);
                        AddSummaryRow("NET AMOUNT", $"₹ {invoice.GrandTotal:F2}", true);
                    });
                });
            });
        }

        private static void ComposeFooter(IContainer container, string instaUrl, string googleUrl)
        {
            container.Column(column =>
            {
                column.Item().AlignCenter().Text("Thank you for your visit to Aadwika Fashion!")
                    .FontSize(10).Italic().FontColor(BrandGray);

                column.Item().PaddingTop(5).Background(LightBg).PaddingVertical(6).Row(row =>
                {
                    row.RelativeItem().AlignCenter().Text(text =>
                    {
                        text.Span("Connect with us: ").FontSize(8).FontColor(BrandGray);
                        text.Hyperlink("Instagram @aadwika_fashion", instaUrl).FontSize(8).Bold().FontColor(AccentColor);
                        text.Span("  |  ").FontSize(8).FontColor(BrandGray);
                        text.Hyperlink("Rate us on Google", googleUrl).FontSize(8).Bold().FontColor(AccentColor);
                    });
                });

                column.Item().PaddingTop(3).AlignCenter().Text("This is a computer-generated invoice. No signature required.")
                    .FontSize(6).FontColor(Colors.Grey.Lighten2);
            });
        }
    }

    public static class StoreInfo
    {
        public static string StoreName { get; set; } = "AADWIKA FASHION";
        public static string StoreAddress { get; set; } = "Bhagalpur Road, Near TATA Showroom,\nDumka, Jharkhand";
        public static string ContactInfo { get; set; } = "Contact: +91 9334799099";
        public static string GSTIN { get; set; } = "20AJHPA7396P1ZV"; // Starts with 20 (Jharkhand State Code)
        public static string StateCode { get; set; } = "20";
    }
    public static class ReceiptBuilder
    {
        // ====================================================================================
        // 1. THERMAL PRINTER FORMAT (ESC/POS RAW BYTES)
        // ====================================================================================
        public static byte[] GenerateThermalReceiptBytes(Invoice invoice, IEnumerable<InvoiceItem> items)
        {
            List<byte> bytes = new List<byte>();

            // ESC/POS Commands
            byte[] initPrinter = new byte[] { 27, 64 };
            byte[] alignCenter = new byte[] { 27, 97, 1 };
            byte[] alignLeft = new byte[] { 27, 97, 0 };
            byte[] alignRight = new byte[] { 27, 97, 2 };
            byte[] boldOn = new byte[] { 27, 69, 1 };
            byte[] boldOff = new byte[] { 27, 69, 0 };
            byte[] cutPaper = new byte[] { 29, 86, 66, 0 };

            bytes.AddRange(initPrinter);

            // --- HEADER ---
            bytes.AddRange(alignCenter);
            bytes.AddRange(boldOn);
            bytes.AddRange(Encoding.ASCII.GetBytes(StoreInfo.StoreName + "\n"));
            bytes.AddRange(boldOff);
            bytes.AddRange(Encoding.ASCII.GetBytes(StoreInfo.StoreAddress.Replace("\n", " ") + "\n"));
            bytes.AddRange(Encoding.ASCII.GetBytes(StoreInfo.ContactInfo + "\n"));
            bytes.AddRange(Encoding.ASCII.GetBytes("GSTIN: " + StoreInfo.GSTIN + "\n"));
            bytes.AddRange(Encoding.ASCII.GetBytes("--------------------------------\n")); // 32 Chars wide (58mm standard)
            bytes.AddRange(boldOn);
            bytes.AddRange(Encoding.ASCII.GetBytes("TAX INVOICE\n"));
            bytes.AddRange(boldOff);
            bytes.AddRange(Encoding.ASCII.GetBytes("--------------------------------\n"));

            // --- CUSTOMER DETAILS ---
            bytes.AddRange(alignLeft);
            bytes.AddRange(Encoding.ASCII.GetBytes($"Inv No: {invoice.InvoiceNo}\n"));
            bytes.AddRange(Encoding.ASCII.GetBytes($"Date  : {invoice.Date:dd-MMM-yyyy HH:mm}\n"));
            bytes.AddRange(Encoding.ASCII.GetBytes($"Name  : {invoice.CustomerName}\n"));
            bytes.AddRange(Encoding.ASCII.GetBytes($"Mobile: {invoice.MobileNo}\n"));
            if (!string.IsNullOrEmpty(invoice.Gstin))
                bytes.AddRange(Encoding.ASCII.GetBytes($"GSTIN : {invoice.Gstin}\n"));
            bytes.AddRange(Encoding.ASCII.GetBytes("--------------------------------\n"));

            // --- ITEMS TABLE ---
            bytes.AddRange(boldOn);
            bytes.AddRange(Encoding.ASCII.GetBytes("Item          Qty  Rate   Total \n"));
            bytes.AddRange(boldOff);
            bytes.AddRange(Encoding.ASCII.GetBytes("--------------------------------\n"));

            foreach (var item in items)
            {
                string name = item.ProductName.Length > 12 ? item.ProductName.Substring(0, 12) : item.ProductName.PadRight(12);
                //string qty = item.Quantity.ToString().PadLeft(3);
                // Expanded PadLeft to 4 to accommodate the decimal point
                string qty = item.Quantity.ToString("0.##").PadLeft(4);
                string rate = item.Rate.ToString("0").PadLeft(6);
                string total = item.TotalAmount.ToString("0").PadLeft(7);
                bytes.AddRange(Encoding.ASCII.GetBytes($"{name} {qty} {rate} {total}\n"));
            }
            bytes.AddRange(Encoding.ASCII.GetBytes("--------------------------------\n"));

            // --- TOTALS & GST LOGIC ---
            bytes.AddRange(alignLeft);
            bytes.AddRange(Encoding.ASCII.GetBytes($"Sub Total        : Rs. {invoice.SubTotal,8:F2}\n"));

            decimal totalDiscount = invoice.TotalDiscount + invoice.GlobalDiscountAmount;
            if (totalDiscount > 0)
                bytes.AddRange(Encoding.ASCII.GetBytes($"Discount         :-Rs. {totalDiscount,8:F2}\n"));

            // Indian GST Split Logic
            if (invoice.IsInterStateSale)
            {
                bytes.AddRange(Encoding.ASCII.GetBytes($"IGST             : Rs. {invoice.TotalTax,8:F2}\n"));
            }
            else
            {
                decimal halfTax = invoice.TotalTax / 2m;
                bytes.AddRange(Encoding.ASCII.GetBytes($"CGST             : Rs. {halfTax,8:F2}\n"));
                bytes.AddRange(Encoding.ASCII.GetBytes($"SGST             : Rs. {halfTax,8:F2}\n"));
            }

            if (invoice.RoundOffAmount != 0)
                bytes.AddRange(Encoding.ASCII.GetBytes($"Round Off        : Rs. {invoice.RoundOffAmount,8:F2}\n"));

            bytes.AddRange(Encoding.ASCII.GetBytes("--------------------------------\n"));
            bytes.AddRange(boldOn);
            bytes.AddRange(Encoding.ASCII.GetBytes($"GRAND TOTAL      : Rs. {invoice.GrandTotal,8:F2}\n"));
            bytes.AddRange(boldOff);
            bytes.AddRange(Encoding.ASCII.GetBytes("--------------------------------\n"));

            // --- FOOTER ---
            bytes.AddRange(alignCenter);
            bytes.AddRange(Encoding.ASCII.GetBytes("Thank you for shopping with us!\n"));
            bytes.AddRange(Encoding.ASCII.GetBytes("Visit Again\n\n\n\n"));
            bytes.AddRange(cutPaper);

            return bytes.ToArray();
        }

        // ====================================================================================
        // 2. A5 / WHATSAPP FORMAT (BEAUTIFUL HTML GENERATION)
        // ====================================================================================
        public static string GenerateA5HtmlInvoice(Invoice invoice, IEnumerable<InvoiceItem> items)
        {
            decimal totalDiscount = invoice.TotalDiscount + invoice.GlobalDiscountAmount;

            string gstRows = "";
            if (invoice.IsInterStateSale)
            {
                gstRows = $@"
                    <tr><td colspan='5' class='text-right'><b>IGST</b></td><td class='text-right'>₹ {invoice.TotalTax:F2}</td></tr>";
            }
            else
            {
                decimal halfTax = invoice.TotalTax / 2m;
                gstRows = $@"
                    <tr><td colspan='5' class='text-right'><b>CGST</b></td><td class='text-right'>₹ {halfTax:F2}</td></tr>
                    <tr><td colspan='5' class='text-right'><b>SGST</b></td><td class='text-right'>₹ {halfTax:F2}</td></tr>";
            }

            string discountRow = totalDiscount > 0 ? $"<tr><td colspan='5' class='text-right'><b>Total Discount</b></td><td class='text-right text-orange'>-₹ {totalDiscount:F2}</td></tr>" : "";
            string roundOffRow = invoice.RoundOffAmount != 0 ? $"<tr><td colspan='5' class='text-right'><b>Round Off</b></td><td class='text-right'>₹ {invoice.RoundOffAmount:F2}</td></tr>" : "";
            string customerGstinRow = !string.IsNullOrEmpty(invoice.Gstin) ? $"<br><b>GSTIN:</b> {invoice.Gstin}" : "";

            StringBuilder itemRows = new StringBuilder();
            int sNo = 1;
            foreach (var item in items)
            {
                itemRows.Append($@"
                    <tr>
                        <td class='text-center'>{sNo++}</td>
                        <td>{item.ProductName}</td>
                        <td class='text-center'>{item.Quantity}</td>
                        <td class='text-right'>{item.Rate:F2}</td>
                        <td class='text-center'>{item.GstPercentage}%</td>
                        <td class='text-right'>{item.TotalAmount:F2}</td>
                    </tr>");
            }

            string html = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='utf-8'>
                <style>
                    body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; color: #333; margin: 0; padding: 20px; font-size: 14px; }}
                    .invoice-box {{ max-width: 148mm; margin: auto; padding: 20px; border: 1px solid #ddd; box-shadow: 0 0 10px rgba(0, 0, 0, 0.15); background: #fff; }}
                    .header {{ text-align: center; border-bottom: 2px solid #005A9C; padding-bottom: 10px; margin-bottom: 20px; }}
                    .header h1 {{ margin: 0; color: #005A9C; font-size: 24px; text-transform: uppercase; letter-spacing: 2px; }}
                    .header p {{ margin: 3px 0; font-size: 12px; color: #555; }}
                    .title {{ text-align: center; font-weight: bold; font-size: 16px; margin-bottom: 15px; letter-spacing: 1px; }}
                    .details-table {{ width: 100%; margin-bottom: 20px; font-size: 13px; }}
                    .details-table td {{ vertical-align: top; }}
                    .items-table {{ width: 100%; border-collapse: collapse; margin-bottom: 20px; font-size: 13px; }}
                    .items-table th, .items-table td {{ padding: 8px; border: 1px solid #ddd; }}
                    .items-table th {{ background: #005A9C; color: white; text-align: left; font-weight: bold; }}
                    .text-center {{ text-align: center !important; }}
                    .text-right {{ text-align: right !important; }}
                    .text-orange {{ color: #E67E22; }}
                    .totals-section td {{ padding: 6px 8px; border-bottom: 1px solid #eee; }}
                    .grand-total {{ font-size: 18px; font-weight: bold; color: #005A9C; background-color: #F4F6F9; }}
                    .footer {{ text-align: center; margin-top: 30px; font-size: 12px; color: #777; border-top: 1px solid #ddd; padding-top: 10px; }}
                </style>
            </head>
            <body>
                <div class='invoice-box'>
                    <div class='header'>
                        <h1>{StoreInfo.StoreName}</h1>
                        <p>{StoreInfo.StoreAddress.Replace("\n", ", ")}</p>
                        <p>{StoreInfo.ContactInfo} | GSTIN: {StoreInfo.GSTIN}</p>
                    </div>

                    <div class='title'>TAX INVOICE</div>

                    <table class='details-table'>
                        <tr>
                            <td style='width: 50%;'>
                                <b>Bill To:</b><br>
                                {invoice.CustomerName}<br>
                                Mobile: {invoice.MobileNo}
                                {customerGstinRow}
                            </td>
                            <td style='width: 50%; text-align: right;'>
                                <b>Invoice No:</b> {invoice.InvoiceNo}<br>
                                <b>Date:</b> {invoice.Date:dd-MMM-yyyy hh:mm tt}<br>
                                <b>State Code:</b> {StoreInfo.StateCode}
                            </td>
                        </tr>
                    </table>

                    <table class='items-table'>
                        <thead>
                            <tr>
                                <th class='text-center'>#</th>
                                <th>Item Description</th>
                                <th class='text-center'>Qty</th>
                                <th class='text-right'>Rate (₹)</th>
                                <th class='text-center'>GST %</th>
                                <th class='text-right'>Amount (₹)</th>
                            </tr>
                        </thead>
                        <tbody class='totals-section'>
                            {itemRows}
                            <tr>
                                <td colspan='5' class='text-right' style='border-top: 2px solid #005A9C;'><b>Sub Total</b></td>
                                <td class='text-right' style='border-top: 2px solid #005A9C;'>₹ {invoice.SubTotal:F2}</td>
                            </tr>
                            {discountRow}
                            {gstRows}
                            {roundOffRow}
                            <tr class='grand-total'>
                                <td colspan='5' class='text-right'>GRAND TOTAL</td>
                                <td class='text-right'>₹ {invoice.GrandTotal:F2}</td>
                            </tr>
                        </tbody>
                    </table>

                    <div class='footer'>
                        <b>Thank you for your business!</b><br>
                        Computer Generated Invoice. No Signature Required.
                    </div>
                </div>
            </body>
            </html>";

            return html;
        }
    }

}
