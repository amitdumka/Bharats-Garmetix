//using System;
//using System.Linq;
//using Garmetix.AI.Billing.Models;
//using QuestPDF.Fluent;
//using QuestPDF.Helpers;
//using QuestPDF.Infrastructure;
//using Colors = QuestPDF.Helpers.Colors;
//using IContainer = QuestPDF.Infrastructure.IContainer;

//namespace Garmetix.AI.Billing.PdfServices
//{
//    public class PurchaseInvoiceDocument : IDocument
//    {
//        private readonly PurchaseInvoice _invoice;
//        private readonly System.Collections.Generic.List<PurchaseItem> _items;

//        public PurchaseInvoiceDocument(PurchaseInvoice invoice, System.Collections.Generic.List<PurchaseItem> items)
//        {
//            _invoice = invoice;
//            _items = items;
//        }

//        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
//        public DocumentSettings GetSettings() => DocumentSettings.Default;

//        public void Compose(IDocumentContainer container)
//        {
//            container
//                .Page(page =>
//                {
//                    page.Size(PageSizes.A4);
//                    page.Margin(1, Unit.Centimetre);
//                    page.PageColor(Colors.White);
//                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Helvetica"));

//                    page.Header().Element(ComposeHeader);
//                    page.Content().Element(ComposeContent);
//                    page.Footer().Element(ComposeFooter);
//                });
//        }

//        private void ComposeHeader(IContainer container)
//        {
//            container.Row(row =>
//            {
//                row.RelativeItem().Column(column =>
//                {
//                    column.Item().Text("GARMETIX RETAIL").FontSize(24).SemiBold().FontColor(Colors.Blue.Darken2);
//                    column.Item().Text("Bhagalpur Road, Dumka, Jharkhand");
//                    column.Item().Text("GSTIN: 20XXXXX1234X1ZX");
//                });

//                row.ConstantItem(150).Column(column =>
//                {
//                    column.Item().Text("INWARD RECEIPT").FontSize(16).Bold().FontColor(Colors.Grey.Darken3);
//                    column.Item().Text($"Inward No: {_invoice.InwardNo}").SemiBold();
//                    column.Item().Text($"Date: {_invoice.InwardDate:dd-MMM-yyyy}");
//                });
//            });
//        }

//        private void ComposeContent(IContainer container)
//        {
//            container.PaddingVertical(1, Unit.Centimetre).Column(column =>
//            {
//                column.Spacing(15);

//                // Vendor Details
//                column.Item().Row(row =>
//                {
//                    row.RelativeItem().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(c =>
//                    {
//                        c.Item().Text("SUPPLIER / VENDOR DETAILS").Bold().FontColor(Colors.Grey.Darken2);
//                        c.Item().Text(_invoice.VendorName).FontSize(14).SemiBold();
//                        if (!string.IsNullOrEmpty(_invoice.VendorAddress)) c.Item().Text($"Address: {_invoice.VendorAddress}");
//                        if (!string.IsNullOrEmpty(_invoice.VendorGstin)) c.Item().Text($"GSTIN: {_invoice.VendorGstin}");
//                    });

//                    row.ConstantItem(20); // Spacer

//                    row.RelativeItem().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(c =>
//                    {
//                        c.Item().Text("VENDOR BILL DETAILS").Bold().FontColor(Colors.Grey.Darken2);
//                        c.Item().Text($"Bill No: {_invoice.VendorInvoiceNo}").SemiBold();
//                        c.Item().Text($"Bill Date: {_invoice.VendorInvoiceDate:dd-MMM-yyyy}");
//                    });
//                });

//                // Items Table
//                column.Item().Element(ComposeTable);

//                // Totals
//                column.Item().AlignRight().Element(ComposeTotals);
//            });
//        }

//        private void ComposeTable(IContainer container)
//        {
//            container.Table(table =>
//            {
//                // Define Columns
//                table.ColumnsDefinition(columns =>
//                {
//                    columns.ConstantColumn(30);  // #
//                    columns.RelativeColumn(4);   // Item Name
//                    columns.RelativeColumn();    // Qty
//                    columns.RelativeColumn();    // Rate
//                    columns.RelativeColumn();    // GST %
//                    columns.RelativeColumn();    // Total
//                });

//                // Header
//                table.Header(header =>
//                {
//                    header.Cell().Element(CellStyle).Text("#");
//                    header.Cell().Element(CellStyle).Text("Item Description");
//                    header.Cell().Element(CellStyle).AlignRight().Text("Qty");
//                    header.Cell().Element(CellStyle).AlignRight().Text("Buy Rate");
//                    header.Cell().Element(CellStyle).AlignRight().Text("GST %");
//                    header.Cell().Element(CellStyle).AlignRight().Text("Amount");

//                    static IContainer CellStyle(IContainer container)
//                    {
//                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
//                    }
//                });

//                // Rows
//                uint index = 1;
//                foreach (var item in _items)
//                {
//                    table.Cell().Element(CellStyle).Text(index++.ToString());
//                    table.Cell().Element(CellStyle).Text(item.ProductName);
//                    table.Cell().Element(CellStyle).AlignRight().Text(item.Quantity.ToString("0.##"));
//                    table.Cell().Element(CellStyle).AlignRight().Text($"₹ {item.Rate:N2}");
//                    table.Cell().Element(CellStyle).AlignRight().Text($"{item.TaxPercentage:0.##}%");
//                    table.Cell().Element(CellStyle).AlignRight().Text($"₹ {item.TotalAmount:N2}");

//                    static IContainer CellStyle(IContainer container)
//                    {
//                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
//                    }
//                }
//            });
//        }

//        private void ComposeTotals(IContainer container)
//        {
//            container.Width(250).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(column =>
//            {
//                column.Item().Row(row => { row.RelativeItem().Text("Sub Total:"); row.RelativeItem().AlignRight().Text($"₹ {_invoice.SubTotal:N2}"); });
//                column.Item().Row(row => { row.RelativeItem().Text("Total GST:"); row.RelativeItem().AlignRight().Text($"₹ {_invoice.TotalTax:N2}"); });
//                column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
//                column.Item().PaddingTop(5).Row(row =>
//                {
//                    row.RelativeItem().Text("GRAND TOTAL:").Bold().FontSize(12);
//                    row.RelativeItem().AlignRight().Text($"₹ {_invoice.GrandTotal:N2}").Bold().FontSize(12).FontColor(Colors.Blue.Darken2);
//                });
//            });
//        }

//        private void ComposeFooter(IContainer container)
//        {
//            container.AlignCenter().Text(x =>
//            {
//                x.Span("Page ");
//                x.CurrentPageNumber();
//                x.Span(" of ");
//                x.TotalPages();
//            });
//        }
//    }
//}