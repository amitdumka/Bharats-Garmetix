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
        public static void Initialize()
        {
            // QuestPDF requires this licensing declaration for the free Community edition
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public static string GenerateA5Pdf(Invoice invoice, IEnumerable<InvoiceItem> items, IEnumerable<PaymentDetail> payments)
        {
            Initialize();

            // Setup file path in the device's cache folder
            string fileName = $"{invoice.InvoiceNo}.pdf";
            string filePath = Path.Combine(Microsoft.Maui.Storage.FileSystem.CacheDirectory, fileName);

            // Replace these with your actual links!
            string instagramUrl = "https://instagram.com/aadwika_fashion";
            string googleMapsUrl = "https://g.page/r/your-google-map-link/review";

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A5);
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(9).FontFamily(Fonts.SegoeUI));

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
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text(StoreInfo.StoreName).FontSize(20).SemiBold().FontColor(Colors.Blue.Darken2);
                    column.Item().Text(StoreInfo.StoreAddress.Replace("\n", ", ")).FontSize(9);
                    column.Item().Text($"{StoreInfo.ContactInfo} | GSTIN: {StoreInfo.GSTIN}");

                    column.Item().PaddingTop(10).Text("TAX INVOICE").FontSize(14).SemiBold();
                });

                row.ConstantItem(120).Column(column =>
                {
                    column.Item().Text("Bill To:").SemiBold();
                    column.Item().Text(invoice.CustomerName);
                    column.Item().Text($"Mob: {invoice.MobileNo}");
                    if (!string.IsNullOrEmpty(invoice.Gstin))
                        column.Item().Text($"GSTIN: {invoice.Gstin}");

                    column.Item().PaddingTop(5).Text($"Inv No: {invoice.InvoiceNo}");
                    column.Item().Text($"Date: {invoice.Date:dd-MMM-yyyy}");
                });
            });
        }

        private static void ComposeContent(IContainer container, Invoice invoice, IEnumerable<InvoiceItem> items, IEnumerable<PaymentDetail> payments)
        {
            container.PaddingVertical(10).Column(column =>
            {
                // 1. Items Table
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(20); // #
                        columns.RelativeColumn();   // Item
                        columns.ConstantColumn(30); // Qty
                        columns.ConstantColumn(50); // Rate
                        columns.ConstantColumn(30); // GST
                        columns.ConstantColumn(60); // Total
                    });

                    table.Header(header =>
                    {
                        header.Cell().Text("#").SemiBold();
                        header.Cell().Text("Description").SemiBold();
                        header.Cell().AlignRight().Text("Qty").SemiBold();
                        header.Cell().AlignRight().Text("Rate").SemiBold();
                        header.Cell().AlignRight().Text("GST%").SemiBold();
                        header.Cell().AlignRight().Text("Amount").SemiBold();

                        header.Cell().ColumnSpan(6).PaddingTop(2).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                    });

                    int sNo = 1;
                    foreach (var item in items)
                    {
                        table.Cell().Text(sNo.ToString());
                        table.Cell().Text(item.ProductName);
                        table.Cell().AlignRight().Text(item.Quantity.ToString());
                        table.Cell().AlignRight().Text($"{item.Rate:F2}");
                        table.Cell().AlignRight().Text($"{item.GstPercentage}%");
                        table.Cell().AlignRight().Text($"{item.TotalAmount:F2}");
                        sNo++;
                    }
                });

                // 2. Totals Area
                column.Item().PaddingTop(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        columns.ConstantColumn(100);
                        columns.ConstantColumn(60);
                    });

                    // Payment Details Box (Left Side)
                    table.Cell().RowSpan(5).PaddingRight(20).Column(pCol =>
                    {
                        pCol.Item().Text("Payment Details:").SemiBold().Underline();
                        if (payments != null && payments.Any())
                        {
                            foreach (var p in payments)
                            {
                                pCol.Item().Text($"{p.Mode}: Rs. {p.Amount:F2}");
                            }
                            if (invoice.BalanceAmount > 0)
                            {
                                pCol.Item().PaddingTop(2).Text($"Balance Due: Rs. {invoice.BalanceAmount:F2}").FontColor(Colors.Red.Medium).SemiBold();
                            }
                        }
                        else
                        {
                            pCol.Item().Text("No payments recorded.");
                        }
                    });

                    // Totals (Right Side)
                    table.Cell().AlignRight().Text("Sub Total:").SemiBold();
                    table.Cell().AlignRight().Text($"Rs. {invoice.SubTotal:F2}");

                    decimal totalDiscount = invoice.TotalDiscount + invoice.GlobalDiscountAmount;
                    if (totalDiscount > 0)
                    {
                        table.Cell().AlignRight().Text("Discount:").SemiBold();
                        table.Cell().AlignRight().Text($"-Rs. {totalDiscount:F2}").FontColor(Colors.Orange.Medium);
                    }

                    if (invoice.IsInterStateSale)
                    {
                        table.Cell().AlignRight().Text("IGST:").SemiBold();
                        table.Cell().AlignRight().Text($"Rs. {invoice.TotalTax:F2}");
                    }
                    else
                    {
                        decimal halfTax = invoice.TotalTax / 2m;
                        table.Cell().AlignRight().Text("CGST + SGST:").SemiBold();
                        table.Cell().AlignRight().Text($"Rs. {invoice.TotalTax:F2}");
                    }

                    if (invoice.RoundOffAmount != 0)
                    {
                        table.Cell().AlignRight().Text("Round Off:").SemiBold();
                        table.Cell().AlignRight().Text($"Rs. {invoice.RoundOffAmount:F2}");
                    }

                    table.Cell().ColumnSpan(2).PaddingTop(2).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);

                    table.Cell().AlignRight().Text("GRAND TOTAL:").FontSize(11).SemiBold();
                    table.Cell().AlignRight().Text($"Rs. {invoice.GrandTotal:F2}").FontSize(11).SemiBold().FontColor(Colors.Blue.Darken2);
                });
            });
        }

        private static void ComposeFooter(IContainer container, string instaUrl, string googleUrl)
        {
            container.Column(column =>
            {
                column.Item().AlignCenter().Text("Thank you for your business!").SemiBold();

                // Clickable Hyperlinks Area
                column.Item().PaddingTop(5).Background(Colors.Grey.Lighten4).Padding(5).AlignCenter().Text(text =>
                {
                    text.Span("📸 Love your purchase? Follow us on ").FontSize(10);
                    text.Hyperlink("Instagram", instaUrl).FontColor(Colors.Blue.Medium).Underline().FontSize(10).SemiBold();
                    text.Span(" | ⭐ Rate us on ").FontSize(10);
                    text.Hyperlink("Google Maps", googleUrl).FontColor(Colors.Blue.Medium).Underline().FontSize(10).SemiBold();
                });
            });
        }
    }
}