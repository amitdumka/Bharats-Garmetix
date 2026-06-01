using Garmetix.Billing.Models;
using Garmetix.Core.Models.Inventory;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Colors = QuestPDF.Helpers.Colors;
using IContainer = QuestPDF.Infrastructure.IContainer;

namespace Garmetix.Billing.Helpers
{
    public class CreditNotePdfBuilder
    {
        public static void Initialize()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }
        public static string GenerateCreditNote(CreditNoteDto data, string companyName = "Aadwika Fashion")
        {
            Initialize();
            //TODO: Check for null  and handle it
            //string fileName = $"{data?.ReturnInvoiceNo}.pdf";
            string fileName = $"CreditNote_{data?.ReturnInvoiceNo}.pdf";
            string filePath = Path.Combine(Microsoft.Maui.Storage.FileSystem.CacheDirectory, fileName);
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    // Set to Standard A4 Paper
                    page.Size(PageSizes.A4);
                    page.Margin(30, Unit.Point);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Helvetica"));

                    // The Content: Two Vouchers split by a dashed line
                    page.Content().Column(column =>
                    {
                        // 1. Top Half: Customer Copy
                        column.Item().Element(c => ComposeVoucher(c, data, companyName, "CUSTOMER COPY"));

                        //TODO: 2. The "Cut Here" dashed line in the middle of the A4 page
                        column.Item().PaddingVertical(25).Row(row =>
                        {
                            row.AutoItem().PaddingRight(10).Text("✂").FontSize(14).FontColor(Colors.Grey.Medium);
                            row.RelativeItem().PaddingTop(8).LineHorizontal(1).LineColor(Colors.Grey.Medium).LineDashPattern(new float[] { 5, 5 });//.Dashed();
                        });

                        // 3. Bottom Half: Store Copy
                        column.Item().Element(c => ComposeVoucher(c, data, companyName, "STORE COPY"));
                    });
                });
            });

            //return document.GeneratePdf();
            document.GeneratePdf(filePath);
            return filePath;
        }

        private static void ComposeVoucher(IContainer container, CreditNoteDto data, string companyName, string copyType)
        {
            // Wrap the voucher in a nice border
            container.Border(1).BorderColor(Colors.Grey.Lighten1).Padding(20).Column(column =>
            {
                // --- VOUCHER HEADER ---
                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(header =>
                    {
                        header.Item().Text(companyName).FontSize(24).SemiBold().FontColor("#D4AF37"); // Gold
                        header.Item().Text("STORE CREDIT NOTE").FontSize(16).FontColor("#0F172A").SemiBold(); // Dark Navy
                        header.Item().Text(copyType).FontSize(10).FontColor(Colors.Grey.Medium).Italic();
                    });

                    row.AutoItem().AlignRight().Column(valBlock =>
                    {
                        valBlock.Item().Text("CREDIT VALUE").FontSize(10).FontColor(Colors.Grey.Medium);
                        valBlock.Item().Text($"₹ {data.TotalAmount:N2}").FontSize(22).SemiBold().FontColor("#0F172A");
                    });
                });

                column.Item().PaddingVertical(15).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                // --- VOUCHER DETAILS & QR CODE ---
                column.Item().Row(row =>
                {
                    // Left Side: Text Details
                    row.RelativeItem().Column(details =>
                    {
                        details.Item().Text(text =>
                        {
                            text.Span("Issued To: ").SemiBold();
                            text.Span(data.CustomerName);
                        });
                        details.Item().Text(text =>
                        {
                            text.Span("Mobile No: ").SemiBold();
                            text.Span(data.MobileNo);
                        });

                        details.Item().PaddingTop(10).Text(text =>
                        {
                            text.Span("Return Invoice Ref: ").SemiBold();
                            text.Span(data.ReturnInvoiceNo);
                        });
                        details.Item().Text(text =>
                        {
                            text.Span("Original Purchase Date: ").SemiBold();
                            text.Span(data.ReturnInvoiceDate.ToString("dd-MMM-yyyy"));
                        });
                        details.Item().Text(text =>
                        {
                            text.Span("Date of Issue: ").SemiBold();
                            text.Span(data.NoteDate.ToString("dd-MMM-yyyy hh:mm tt"));
                        });
                    });

                    // Right Side: The QR Code
                    if (data.QrCodeImage != null)
                    {
                        row.ConstantItem(120).AlignRight().Column(qrCol =>
                        {
                            qrCol.Item().Width(100).Height(100).Image(data.QrCodeImage);
                            qrCol.Item().AlignCenter().Text("Scan to Redeem").FontSize(8).FontColor(Colors.Grey.Medium);
                        });
                    }
                });

                // --- FOOTER & TERMS ---
                column.Item().PaddingTop(20).Background("#0F172A").Padding(10).Column(footer =>
                {
                    footer.Item().Text("Terms & Conditions:").FontSize(9).SemiBold().FontColor("#D4AF37");
                    footer.Item().Text("1. This credit note is valid for future purchases at Aadwika Fashion.").FontSize(8).FontColor(Colors.White);
                    footer.Item().Text("2. Cannot be exchanged for cash. Must be presented at the time of checkout.").FontSize(8).FontColor(Colors.White);
                });
            });
        }
    }
}