using Garmetix.AI.Billing.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Colors = QuestPDF.Helpers.Colors;
using IContainer = QuestPDF.Infrastructure.IContainer;

namespace Garmetix.Billing.AIBased.Helpers
{
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
                    page.DefaultTextStyle(x => x.FontSize(9).FontFamily(Fonts.Georgia).FontColor(BrandDark));

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
}