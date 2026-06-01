using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm.Input;
using Garmetix.Billing.Helpers;
using Garmetix.Billing.Pages.Popups;
using Garmetix.Billing.Services;
using Garmetix.Core.Models.Inventory;
using QuestPDF.Fluent;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using Colors = QuestPDF.Helpers.Colors;
using IContainer = QuestPDF.Infrastructure.IContainer;
namespace Garmetix.Billing.Helpers
{
    public class RedemptionDto
    {
        public string CustomerName { get; set; }
        public string CreditNoteNo { get; set; }
        public string NewSaleInvoiceNo { get; set; }
        public decimal RedeemedAmount { get; set; }
        public decimal RemainingBalance { get; set; }
        public DateTime RedemptionDate { get; set; }
    }

    public class RedemptionReceiptPdfBuilder
    {
        public static byte[] GenerateReceipt(RedemptionDto data, string companyName = "Aadwika Fashion")
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30, Unit.Point);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Helvetica"));

                    page.Content().Column(column =>
                    {
                        // Top Half: Customer Copy (No signature required)
                        column.Item().Element(c => ComposeVoucher(c, data, companyName, "CUSTOMER COPY", false));

                        // Dashed Cut Line
                        column.Item().PaddingVertical(25).Row(row =>
                        {
                            row.AutoItem().PaddingRight(10).Text("✂").FontSize(14).FontColor(Colors.Grey.Medium);
                            row.RelativeItem().PaddingTop(8).LineHorizontal(1).LineColor(Colors.Grey.Medium).LineDashPattern(new float[] { 5, 5 });
                        });

                        // Bottom Half: Store Copy (REQUIRES SIGNATURE)(
                        column.Item().Element(c => ComposeVoucher(c, data, companyName, "STORE COPY", true));
                    });
                });
            });

            return document.GeneratePdf();
        }

        private static void ComposeVoucher(IContainer container, RedemptionDto data, string companyName, string copyType, bool requiresSignature)
        {
            container.Border(1).BorderColor(Colors.Grey.Lighten1).Padding(20).Column(column =>
            {
                // Header
                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(header =>
                    {
                        header.Item().Text(companyName).FontSize(24).SemiBold().FontColor("#D4AF37");
                        header.Item().Text("CREDIT REDEMPTION RECEIPT").FontSize(14).FontColor("#0F172A").SemiBold();
                        header.Item().Text(copyType).FontSize(10).FontColor(Colors.Grey.Medium).Italic();
                    });

                    row.AutoItem().AlignRight().Column(valBlock =>
                    {
                        valBlock.Item().Text("AMOUNT REDEEMED").FontSize(10).FontColor(Colors.Grey.Medium);
                        valBlock.Item().Text($"₹ {data.RedeemedAmount:N2}").FontSize(20).SemiBold().FontColor("#0F172A");
                    });
                });

                column.Item().PaddingVertical(15).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                // Details
                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(details =>
                    {
                        details.Item().Text($"Customer: {data.CustomerName}").SemiBold();
                        details.Item().PaddingTop(5).Text($"Original Credit Note Ref: {data.CreditNoteNo}");
                        details.Item().Text($"Applied to Sale Invoice: {data.NewSaleInvoiceNo}");
                        details.Item().Text($"Date of Redemption: {data.RedemptionDate:dd-MMM-yyyy hh:mm tt}");
                        details.Item().PaddingTop(5).Text($"Remaining Note Balance: ₹ {data.RemainingBalance:N2}").FontColor(Colors.Green.Darken2);
                    });
                });

                // Signature Line (Only on Store Copy)
                if (requiresSignature)
                {
                    column.Item().PaddingTop(40).Row(row =>
                    {
                        row.RelativeItem(); // Spacer
                        row.ConstantItem(200).Column(sig =>
                        {
                            sig.Item().LineHorizontal(1).LineColor(Colors.Black);
                            sig.Item().AlignCenter().PaddingTop(5).Text("Customer Signature").FontSize(9);
                        });
                    });
                }
            });
        }
    }
}


// Example usage:

//using CommunityToolkit.Maui.Views;
//using Garmetix.UI.Popups;
//using Garmetix.UI.Services.Printing;

//// ... inside PosViewModel ...

//[ObservableProperty]
//private Invoice _activeCreditNote;

//[ObservableProperty]
//private decimal _appliedCreditAmount;

//[RelayCommand]
//public async Task ScanCreditNoteAsync()
//{
//    // 1. Open the Camera/USB Scanner Popup
//    var scanPopup = new InvoiceScanPopup();
//    var scanResult = await Shell.Current.CurrentPage.ShowPopupAsync(scanPopup) as ScanPopupResult;

//    if (scanResult == null || string.IsNullOrWhiteSpace(scanResult.ScannedCode)) return;

//    // 2. Look up the Credit Note by the scanned Guid
//    if (Guid.TryParse(scanResult.ScannedCode, out Guid scannedId))
//    {
//        var context = _invoiceService.GetContext();
//        var creditNote = await context.Invoices.FirstOrDefaultAsync(i => i.Id == scannedId && i.IsSaleReturn == true);

//        if (creditNote == null)
//        {
//            await Application.Current.MainPage.DisplayAlert("Error", "Invalid or unrecognized Credit Note.", "OK");
//            return;
//        }

//        if (creditNote.RemainingCredit <= 0)
//        {
//            await Application.Current.MainPage.DisplayAlert("Depleted", "This Credit Note has a zero balance and cannot be used.", "OK");
//            return;
//        }

//        // 3. Open Redemption Amount Popup
//        var redeemPopup = new RedeemCreditPopup(creditNote, CurrentBill.GrandTotal);
//        var amountToRedeem = await Shell.Current.CurrentPage.ShowPopupAsync(redeemPopup) as decimal?;

//        // 4. Apply to memory (Database save happens on final checkout)
//        if (amountToRedeem.HasValue && amountToRedeem.Value > 0)
//        {
//            ActiveCreditNote = creditNote;
//            AppliedCreditAmount = amountToRedeem.Value;

//            // Adjust current bill display total
//            CurrentBill.GrandTotal -= AppliedCreditAmount;

//            await Application.Current.MainPage.DisplayAlert("Applied", $"₹ {AppliedCreditAmount} applied to bill.", "OK");
//        }
//    }
//}

//[RelayCommand]
//public async Task CheckoutAsync()
//{
//    // ... setup db transaction ...

//    try
//    {
//        // 1. Save standard invoice
//        CurrentBill.AppliedCreditNoteId = ActiveCreditNote?.Id;
//        CurrentBill.AppliedCreditAmount = AppliedCreditAmount;
//        await _context.Invoices.AddAsync(CurrentBill);

//        // 2. Deduct from the original Credit Note
//        if (ActiveCreditNote != null && AppliedCreditAmount > 0)
//        {
//            var noteToUpdate = await _context.Invoices.FindAsync(ActiveCreditNote.Id);
//            noteToUpdate.RemainingCredit -= AppliedCreditAmount;
//            _context.Invoices.Update(noteToUpdate);

//            // Optional: Write to StoreCreditLedger here for auditing
//        }

//        // ... save items & commit transaction ...
//        await _context.SaveChangesAsync();
//        await transaction.CommitAsync();

//        // 3. GENERATE THE SIGNATURE RECEIPT IF CREDIT WAS USED
//        if (ActiveCreditNote != null)
//        {
//            var printData = new RedemptionDto
//            {
//                CustomerName = CurrentBill.CustomerName ?? "Walk-in Customer",
//                CreditNoteNo = ActiveCreditNote.InvoiceNo,
//                NewSaleInvoiceNo = CurrentBill.InvoiceNo,
//                RedeemedAmount = AppliedCreditAmount,
//                RemainingBalance = ActiveCreditNote.RemainingCredit - AppliedCreditAmount,
//                RedemptionDate = DateTime.Now
//            };

//            byte[] pdfBytes = RedemptionReceiptPdfBuilder.GenerateReceipt(printData);
//            await PdfPrintService.SaveAndSharePdfAsync(pdfBytes, $"Redemption_{CurrentBill.InvoiceNo}.pdf");
//        }

//        // Print standard sale invoice next...
//        // Reset POS...
//    }
//    catch (Exception ex)
//    {
//        await transaction.RollbackAsync();
//        // Handle error...
//    }
//}


