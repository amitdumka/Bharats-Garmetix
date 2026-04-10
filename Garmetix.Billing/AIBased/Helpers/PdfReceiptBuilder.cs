using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        static readonly string AccentColor = "#0284C7";  // Sky 600 (For Links)

        public static void Initialize()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public static string GenerateA5Pdf(Invoice invoice, IEnumerable<InvoiceItem> items, IEnumerable<PaymentDetail> payments)
        {
            Initialize();

            string fileName = $"{invoice.InvoiceNo}.pdf";
            string filePath = Path.Combine(Microsoft.Maui.Storage.FileSystem.CacheDirectory, fileName);

            // Update these URLs to your actual pages
            string instagramUrl = "https://instagram.com/aadwika_fashion";
            string googleMapsUrl = "https://g.page/r/your-google-map-link/review";

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A5);
                    page.Margin(15, Unit.Millimetre);
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
            container.PaddingBottom(15).Row(row =>
            {
                // Left Side: Brand Identity
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text(StoreInfo.StoreName)
                        .FontSize(22)
                        .Black()
                        .LetterSpacing(0.05f); // Adds premium spacing to brand name

                    column.Item().PaddingTop(2).Text(StoreInfo.StoreAddress.Replace("\n", ", "))
                        .FontSize(8)
                        .FontColor(BrandGray);

                    column.Item().Text($"{StoreInfo.ContactInfo}  •  GSTIN: {StoreInfo.GSTIN}")
                        .FontSize(8)
                        .FontColor(BrandGray);
                });

                // Right Side: Minimalist Invoice Badge
                row.ConstantItem(120).AlignRight().Column(column =>
                {
                    column.Item().Background(BrandDark).PaddingVertical(4).PaddingHorizontal(10).AlignCenter()
                        .Text("TAX INVOICE").FontSize(10).Bold().FontColor(Colors.White).LetterSpacing(0.05f);

                    column.Item().PaddingTop(5).Text($"# {invoice.InvoiceNo}").FontSize(9).SemiBold().AlignRight();
                    column.Item().Text($"{invoice.Date:dd MMM yyyy, hh:mm tt}").FontSize(8).FontColor(BrandGray).AlignRight();
                });
            });
        }

        private static void ComposeContent(IContainer container, Invoice invoice, IEnumerable<InvoiceItem> items, IEnumerable<PaymentDetail> payments)
        {
            container.Column(column =>
            {
                // 1. Clean Customer Info Block
                column.Item().PaddingBottom(15).Background(LightBg).Padding(10).Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("BILLED TO").FontSize(7).Bold().FontColor(BrandGray).LetterSpacing(0.05f);
                        col.Item().Text(invoice.CustomerName).FontSize(11).SemiBold();
                        col.Item().Text($"+91 {invoice.MobileNo}").FontSize(9);
                        if (!string.IsNullOrEmpty(invoice.Gstin))
                            col.Item().Text($"GSTIN: {invoice.Gstin}").FontSize(9);
                    });
                });

                // 2. Minimalist Items Table
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(20); // #
                        columns.RelativeColumn();   // Item
                        columns.ConstantColumn(30); // Qty
                        columns.ConstantColumn(45); // Rate
                        columns.ConstantColumn(40); // GST
                        columns.ConstantColumn(55); // Total
                    });

                    // Modern Header: Border bottom only, subtle text
                    table.Header(header =>
                    {
                        header.Cell().PaddingBottom(5).BorderBottom(1).BorderColor(BorderColor).Text("#").FontSize(8).Bold().FontColor(BrandGray);
                        header.Cell().PaddingBottom(5).BorderBottom(1).BorderColor(BorderColor).Text("ITEM").FontSize(8).Bold().FontColor(BrandGray);
                        header.Cell().PaddingBottom(5).BorderBottom(1).BorderColor(BorderColor).AlignRight().Text("QTY").FontSize(8).Bold().FontColor(BrandGray);
                        header.Cell().PaddingBottom(5).BorderBottom(1).BorderColor(BorderColor).AlignRight().Text("RATE").FontSize(8).Bold().FontColor(BrandGray);
                        header.Cell().PaddingBottom(5).BorderBottom(1).BorderColor(BorderColor).AlignRight().Text("TAX").FontSize(8).Bold().FontColor(BrandGray);
                        header.Cell().PaddingBottom(5).BorderBottom(1).BorderColor(BorderColor).AlignRight().Text("AMOUNT").FontSize(8).Bold().FontColor(BrandGray);
                    });

                    // Item Rows: Spaced out, no vertical lines
                    int sNo = 1;
                    foreach (var item in items)
                    {
                        table.Cell().PaddingVertical(6).BorderBottom(1).BorderColor(BorderColor).Text(sNo.ToString()).FontSize(9);
                        table.Cell().PaddingVertical(6).BorderBottom(1).BorderColor(BorderColor).Text(item.ProductName).FontSize(9).SemiBold();
                        table.Cell().PaddingVertical(6).BorderBottom(1).BorderColor(BorderColor).AlignRight().Text(item.Quantity.ToString()).FontSize(9);
                        table.Cell().PaddingVertical(6).BorderBottom(1).BorderColor(BorderColor).AlignRight().Text($"{item.Rate:F2}").FontSize(9);
                        table.Cell().PaddingVertical(6).BorderBottom(1).BorderColor(BorderColor).AlignRight().Text($"{item.GstPercentage}%").FontSize(9).FontColor(BrandGray);
                        table.Cell().PaddingVertical(6).BorderBottom(1).BorderColor(BorderColor).AlignRight().Text($"{item.TotalAmount:F2}").FontSize(9).SemiBold();
                        sNo++;
                    }
                });

                // 3. Structured Footer & Totals
                column.Item().PaddingTop(15).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        columns.ConstantColumn(15); // Spacer
                        columns.ConstantColumn(130); // Totals Column
                    });

                    // Left Side: Payment Breakdown
                    table.Cell().Column(pCol =>
                    {
                        pCol.Item().Text("PAYMENT METHOD").FontSize(7).Bold().FontColor(BrandGray).LetterSpacing(0.05f);
                        if (payments != null && payments.Any())
                        {
                            foreach (var p in payments)
                            {
                                pCol.Item().PaddingTop(2).Text($"{p.Mode.ToUpper()}  •  ₹ {p.Amount:F2}").FontSize(9).SemiBold();
                            }
                        }

                        if (invoice.BalanceAmount > 0)
                        {
                            pCol.Item().PaddingTop(5).Text($"DUE: ₹ {invoice.BalanceAmount:F2}").FontSize(9).Bold().FontColor(Colors.Red.Medium);
                        }
                    });

                    // Spacer
                    table.Cell().Text("");

                    // Right Side: Financial Totals
                    table.Cell().Column(tCol =>
                    {
                        tCol.Item().Row(r => { r.RelativeItem().Text("Subtotal").FontColor(BrandGray); r.ConstantItem(60).AlignRight().Text($"₹ {invoice.SubTotal:F2}"); });

                        decimal totalDiscount = invoice.TotalDiscount + invoice.GlobalDiscountAmount;
                        if (totalDiscount > 0)
                        {
                            tCol.Item().PaddingTop(2).Row(r => { r.RelativeItem().Text("Discount").FontColor(BrandGray); r.ConstantItem(60).AlignRight().Text($"- ₹ {totalDiscount:F2}"); });
                        }

                        if (invoice.IsInterStateSale)
                        {
                            tCol.Item().PaddingTop(2).Row(r => { r.RelativeItem().Text("IGST").FontColor(BrandGray); r.ConstantItem(60).AlignRight().Text($"₹ {invoice.TotalTax:F2}"); });
                        }
                        else
                        {
                            decimal halfTax = invoice.TotalTax / 2m;
                            tCol.Item().PaddingTop(2).Row(r => { r.RelativeItem().Text("CGST").FontColor(BrandGray); r.ConstantItem(60).AlignRight().Text($"₹ {halfTax:F2}"); });
                            tCol.Item().PaddingTop(2).Row(r => { r.RelativeItem().Text("SGST").FontColor(BrandGray); r.ConstantItem(60).AlignRight().Text($"₹ {halfTax:F2}"); });
                        }

                        if (invoice.RoundOffAmount != 0)
                        {
                            tCol.Item().PaddingTop(2).Row(r => { r.RelativeItem().Text("Round Off").FontColor(BrandGray); r.ConstantItem(60).AlignRight().Text($"₹ {invoice.RoundOffAmount:F2}"); });
                        }

                        // Big Grand Total Line
                        tCol.Item().PaddingTop(8).BorderTop(1).BorderColor(BrandDark).PaddingTop(5).Row(r =>
                        {
                            r.RelativeItem().Text("TOTAL").Bold().FontSize(11);
                            r.ConstantItem(70).AlignRight().Text($"₹ {invoice.GrandTotal:F2}").Bold().FontSize(12);
                        });
                    });
                });
            });
        }

        private static void ComposeFooter(IContainer container, string instaUrl, string googleUrl)
        {
            container.Column(column =>
            {
                // Stylish Thank You message
                column.Item().AlignCenter().Text("Thank you for shopping with us.")
                    .FontSize(11).Italic().FontColor(BrandGray);

                // Embedded Social Bar
                column.Item().PaddingTop(8).Background(LightBg).PaddingVertical(8).PaddingHorizontal(10).Row(row =>
                {
                    row.RelativeItem().AlignCenter().Text(text =>
                    {
                        text.Span("Follow us on ").FontSize(8).FontColor(BrandGray);
                        text.Hyperlink("Instagram", instaUrl).FontSize(8).Bold().FontColor(AccentColor);

                        text.Span("   |   ").FontSize(8).FontColor(BrandGray);

                        text.Span("Rate your experience on ").FontSize(8).FontColor(BrandGray);
                        text.Hyperlink("Google", googleUrl).FontSize(8).Bold().FontColor(AccentColor);
                    });
                });

                // Micro Terms & Conditions
                column.Item().PaddingTop(5).AlignCenter().Text("Goods once sold will not be taken back. Subject to local jurisdiction.")
                    .FontSize(6).FontColor(Colors.Grey.Lighten1);
            });
        }
    }
}