using System;
using Garmetix.AI.Billing.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Garmetix.AI.Billing.PdfServices
{
    public class VoucherDocument : IDocument
    {
        private readonly Voucher _voucher;
        private readonly bool _isDuplicate;

        public VoucherDocument(Voucher voucher, bool isDuplicate = false)
        {
            _voucher = voucher;
            _isDuplicate = isDuplicate;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public DocumentSettings GetSettings() => DocumentSettings.Default;

        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.Size(PageSizes.A5.Landscape());
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Helvetica"));

                    // Watermark for Duplicates
                    if (_isDuplicate)
                    {
                        page.Background().AlignCenter().AlignMiddle()
                            .Text("DUPLICATE").FontSize(60).FontColor(Colors.Grey.Lighten3).Bold();
                    }

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    page.Footer().Element(ComposeFooter);
                });
        }

        private void ComposeHeader(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().Row(row =>
                {
                    row.RelativeItem().Text("GARMETIX RETAIL").FontSize(20).Bold().FontColor(Colors.Blue.Darken2);
                    
                    string title = _voucher.VoucherType == VoucherType.Receipt ? "RECEIPT VOUCHER" : 
                                   _voucher.VoucherType == VoucherType.Payment ? "PAYMENT VOUCHER" : "EXPENSE VOUCHER";
                    
                    row.ConstantItem(200).AlignRight().Text(title).FontSize(16).Bold().FontColor(Colors.Grey.Darken3);
                });
                
                column.Item().Row(row =>
                {
                    row.RelativeItem().Text("Bhagalpur Road, Dumka, Jharkhand");
                    row.ConstantItem(200).AlignRight().Text(_isDuplicate ? "(DUPLICATE COPY)" : "(ORIGINAL COPY)").FontSize(9).Italic();
                });
                
                column.Item().PaddingTop(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.PaddingVertical(15).Column(column =>
            {
                // Voucher Meta Data
                column.Item().Row(row =>
                {
                    row.RelativeItem().Text($"Voucher No: {_voucher.VoucherNumber}").SemiBold();
                    row.RelativeItem().AlignRight().Text($"Date: {_voucher.OnDate:dd-MMM-yyyy}").SemiBold();
                });

                column.Item().PaddingVertical(15).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(inner =>
                {
                    // Paid To / Received From
                    string partyPrefix = _voucher.VoucherType == VoucherType.Receipt ? "Received From:" : "Paid To:";
                    inner.Item().PaddingBottom(10).Row(row =>
                    {
                        row.ConstantItem(100).Text(partyPrefix).SemiBold();
                        row.RelativeItem().Text(_voucher.PartyName).Bold().FontSize(13);
                    });

                    // Amount
                    inner.Item().PaddingBottom(10).Row(row =>
                    {
                        row.ConstantItem(100).Text("Amount (₹):").SemiBold();
                        row.RelativeItem().Text($"Rs. {_voucher.Amount:N2}").Bold().FontSize(13);
                    });

                    // Payment Mode & Bank
                    inner.Item().PaddingBottom(10).Row(row =>
                    {
                        row.ConstantItem(100).Text("Payment Mode:").SemiBold();
                        string modeText = _voucher.PaymentMode.ToString();
                        if (_voucher.PaymentMode != PaymentMode.Cash && !string.IsNullOrEmpty(_voucher.PaymentDetails))
                        {
                            modeText += $" (Ref: {_voucher.PaymentDetails})";
                        }
                        row.RelativeItem().Text(modeText);
                    });

                    // Particulars
                    inner.Item().Row(row =>
                    {
                        row.ConstantItem(100).Text("Towards:").SemiBold();
                        row.RelativeItem().Text(_voucher.Particulars);
                    });
                });
            });
        }

        private void ComposeFooter(IContainer container)
        {
            container.PaddingTop(20).Row(row =>
            {
                row.RelativeItem().AlignCenter().Text("___________________________\nReceiver's Signature");
                row.RelativeItem().AlignCenter().Text("___________________________\nAuthorised Signatory");
            });
        }
    }
}