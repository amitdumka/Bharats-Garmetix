
using Bharat.ToolKits.Helpers;
using Garmetix.Models.Reports;
using Garmetix.PdfServices.Base;
using Garmetix.PdfServices.Interfaces;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using System.Diagnostics;

namespace Garmetix.PdfServices.Services.Accounting;

internal class PdfVoucherService : PdfBaseService, IPdfVoucherService
{
    public PdfVoucherService()
    {
        //TODO: Make the PDF Service Singleton and make dispose of pdfstynling for fonts, 
        //TODO: remove the neccessity of reinitializing the service and use of fontstreamer
        _ = Init();
    }

    

    /// <summary>
    /// Creates a cash voucher PDF document
    /// </summary>
    /// <param name="details"></param>
    /// <param name="duplicate"></param>
    /// <returns></returns>
    public async Task<string> CreatePdfCashVoucherAsync(VoucherDetails details, bool duplicate = false)
    {
        try
        {
            await Init();
            using PdfDocument document = pdfService.SetupDocumentSettings(duplicate);

            // Add a page to the document.
            PdfPage page = document.Pages.Add();
            PdfGraphics graphics = page.Graphics;
            float pageWidth = page.GetClientSize().Width;
            float pageHeight = page.GetClientSize().Height;

            // Calculate height for each voucher area, considering space for a separator.
            float voucherAreaHeight = (pageHeight - 30) / 2; // 30pt for separator line and text

            if (string.IsNullOrEmpty(details.AmountInWords))
            {
                //TODO: Handle AmountInWords services
                details.AmountInWords = NumberToWords.ConvertAmount((double)details.Amount);
            }
            if (duplicate)
            {
                // --- Draw Original Copy in the top half of the page ---
                // The bounds define the drawable area for this specific voucher instance.
                // X = 0, Y = 0 (relative to page's content area), Width = pageWidth, Height = voucherAreaHeight
                CreateSingleVoucher(graphics, details, new RectangleF(0, 0, pageWidth, voucherAreaHeight), "Original Copy");

                // --- Draw Separator Line and Text between the two vouchers ---
                float separatorY = voucherAreaHeight + 5; // Position below the first voucher

                // Draw the separator line.
                pdfService.DrawSeparatorLine(graphics, separatorY, pageWidth);

                // --- Draw Duplicate Copy in the bottom half of the page ---
                // The bounds for the duplicate copy start below the separator line and text.
                CreateSingleVoucher(graphics, details, new RectangleF(0, separatorY + 20, pageWidth, voucherAreaHeight), "Duplicate Copy");
            }
            else
            {
                voucherAreaHeight = pageHeight - 30;
                // --- Draw Single Copy in the full page height ---
                CreateSinglePageVoucher(graphics, details, new RectangleF(0, 0, pageWidth, pageHeight), "Original/Duplicate Copy");
            }
            return await pdfService.SaveAndLaunchPdf(document, $"{details.VoucherType}_{details.VoucherNumber}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf", "CashVoucher", false);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            await Shell.Current.DisplayAlert("Error", $"Failed to create PDF: {ex.Message}", "OK");
            return string.Empty;
        }
        finally
        {
            pdfService?.Dispose();
        }
    }

    /// <summary>
    /// Creates a  voucher PDF document
    /// </summary>
    /// <param name="details"></param>
    /// <param name="duplicate"></param>
    /// <returns></returns>
    public async Task<string> CreatePdfVoucherAsync(VoucherDetails details, bool duplicate = false)
    {
        try
        {
            await Init();
            using PdfDocument document = pdfService.SetupDocumentSettings(duplicate);

            // Add a page to the document.
            PdfPage page = document.Pages.Add();
            PdfGraphics graphics = page.Graphics;
            float pageWidth = page.GetClientSize().Width;
            float pageHeight = page.GetClientSize().Height;

            // Calculate height for each voucher area, considering space for a separator.
            float voucherAreaHeight = (pageHeight - 30) / 2; // 30pt for separator line and text

            if (string.IsNullOrEmpty(details.AmountInWords))
            {
                //TODO: Handle AmountInWords services
                details.AmountInWords = NumberToWords.ConvertAmount((double)details.Amount);
            }
            if (duplicate)
            {
                // --- Draw Original Copy in the top half of the page ---
                // The bounds define the drawable area for this specific voucher instance.
                // X = 0, Y = 0 (relative to page's content area), Width = pageWidth, Height = voucherAreaHeight
                CreateSingleVoucher(graphics, details, new RectangleF(0, 0, pageWidth, voucherAreaHeight), "Original Copy");

                // --- Draw Separator Line and Text between the two vouchers ---
                float separatorY = voucherAreaHeight + 5; // Position below the first voucher

                // Draw the separator line.
                pdfService.DrawSeparatorLine(graphics, separatorY, pageWidth);

                // --- Draw Duplicate Copy in the bottom half of the page ---
                // The bounds for the duplicate copy start below the separator line and text.
                CreateSingleVoucher(graphics, details, new RectangleF(0, separatorY + 20, pageWidth, voucherAreaHeight), "Duplicate Copy");
            }
            else
            {
                voucherAreaHeight = pageHeight - 30;
                // --- Draw Single Copy in the full page height ---
                CreateSinglePageVoucher(graphics, details, new RectangleF(0, 0, pageWidth, pageHeight), "Original/Duplicate Copy");
            }
            return await pdfService.SaveAndLaunchPdf(document, $"{details.VoucherType}_{details.VoucherNumber}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf", "Voucher", false);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            await Shell.Current.DisplayAlert("Error", $"Failed to create PDF: {ex.Message}", "OK");
            return string.Empty;
        }
        finally
        {
            pdfService?.Dispose();
        }
    }

    public async Task<string> CreatePdfCashVoucherAsync(string cashVoucherNumber, bool duplicate = false)
    {
        var voucher = await PdfService.Db.CashVouchers.Include(c => c.Transaction).Include(c => c.Employee).Where(c => c.VoucherNumber == cashVoucherNumber).FirstOrDefaultAsync();

        if (voucher == null) throw new Exception("Voucher Not Found");

        string fileName = $"CashVoucher_VN:{cashVoucherNumber}_{DateTime.Now:ddmmyyHHmmss}.pdf";
        VoucherDetails voucherDetails = new VoucherDetails
        {
            IsCashVoucher = true,
            VoucherType = Enum.GetName(voucher.VoucherType)!,
            VoucherNumber = voucher.VoucherNumber,
            Date = voucher.OnDate,
            PayeeOrPayerName = voucher.PartyName,
            Narration = voucher.Particulars,
            Amount = voucher.Amount,
            PaymentMethod = "Cash",
            PaymentDetails = voucher.Transaction?.Name ?? $"Cash " + voucher.VoucherType.ToString(),
            AuthorizedSignatory = voucher.Employee?.FullName ?? "Manager",
            Voucher = voucher.VoucherType,
            TransactionType = voucher.Transaction?.Name ?? $"Cash " + voucher.VoucherType.ToString()
        };
        return await CreatePdfCashVoucherAsync(voucherDetails, duplicate);
    }

    public async Task<string> CreatePdfVoucherAsync(string voucherNumber, bool duplicate = false)
    {
        var voucher = await PdfService.Db.Vouchers.Include(c => c.Ledger).Include(c => c.Party).Include(c => c.Employee).Where(c => c.VoucherNumber == voucherNumber).FirstOrDefaultAsync();

        if (voucher == null) throw new Exception("Voucher Not Found");

        string fileName = $"Voucher_VN:{voucherNumber}_{DateTime.Now:ddmmyyHHmmss}.pdf";
        var LastVoucherDetails = new VoucherDetails
        {
            IsCashVoucher = false,
            VoucherType = Enum.GetName(voucher.VoucherType)!,
            VoucherNumber = voucher.VoucherNumber,
            Date = voucher.OnDate,
            PayeeOrPayerName = voucher.PartyName,
            Narration = voucher.Particulars,
            Amount = voucher.Amount,
            PaymentMethod = Enum.GetName(voucher.PaymentMode)!,
            PaymentDetails = voucher.PaymentDetails ?? "",
            AuthorizedSignatory = voucher.Employee?.FullName ?? "Manager",
            Voucher = voucher.VoucherType,
        };
        if (voucher.IsParty)
        {
            LastVoucherDetails.PartyDetails = voucher.Party?.Address ?? "" + "\n" + voucher.Party?.Phone + "\n" + voucher.Party?.GSTIN ?? "";
        }
        return await CreatePdfVoucherAsync(LastVoucherDetails, duplicate);
    }

    //Reports
    public Task<string> CreatePdfExpenseVoucherReportAsync(DateTime periodStart, DateTime periodEnd, bool cashVoucher = false)
    {
        throw new NotImplementedException();
    }

    public Task<string> CreatePdfPaymentVoucherReportAsync(DateTime periodStart, DateTime periodEnd, bool cashVoucher = false)
    {
        throw new NotImplementedException();
    }

    public Task<string> CreatePdfReceiptVoucherReportAsync(DateTime periodStart, DateTime periodEnd, bool cashVoucher = false)
    {
        throw new NotImplementedException();
    }

    #region VoucherPrinter

    /// <summary>
    /// Creates a single voucher PDF document with space for duplicate copy also when use again
    /// </summary>
    /// <param name="graphics"></param>
    /// <param name="details"></param>
    /// <param name="bounds"></param>
    /// <param name="copyType"></param>
    /// <returns></returns>
    private void CreateSingleVoucher(PdfGraphics graphics, VoucherDetails details, RectangleF bounds, string copyType)
    {
        // currentRelativeY tracks the vertical position within the *current voucher's bounds*.
        // All Y coordinates for drawing operations are calculated by adding this relative Y
        // to the absolute starting Y of the current voucher's bounds (bounds.Y).

        float currentRelativeY = 0;
        // --- 1. Draw Header for the Voucher Copy ---
        string title = $"{details.VoucherType} Voucher ({copyType})";
        //Handling Cash Voucher Title
        title = details.IsCashVoucher ? "Cash " + title : title;

        pdfService.AddHeaderToPdf(graphics, title, ref currentRelativeY, bounds);

        pdfService.AddCompanyInfo(graphics, ref currentRelativeY, bounds, details.Date, $"VCH: {details.VoucherNumber}");

        // --- 3. Main Voucher Details Grid ---
        PdfGrid mainGrid = new();
        mainGrid.Columns.Add(2);
        mainGrid.Columns[0].Width = bounds.Width * 0.20f; // Label column width
        mainGrid.Columns[1].Width = bounds.Width * 0.80f; // Value column width

        // Style for the main details grid.
        mainGrid.Style.Font = pdfService?.PdfStyles?.NormalFont!;
        mainGrid.Style.CellPadding = new PdfPaddings(5, 5, 5, 5);
        // Remove all borders for this grid to give a cleaner, card-like look.
        

        // Row for Payee/Payer Name
        string partyLabel = details.VoucherType == "Receipt" ? "Received From:" : "Paid To:";
        string paidLabel = details.VoucherType == "Receipt" ? "Received in " : "Paid in ";
        pdfService?.AddGridRowNoBorder(mainGrid, pdfService?.PdfStyles?.BoldFont!, partyLabel, details.PayeeOrPayerName);

        // Row for Amount in Words
        pdfService?.AddGridRowNoBorder(mainGrid, pdfService?.PdfStyles?.BoldFont!, "The Sum of:", details.AmountInWords);
        string amountLabel = "";


        if (string.IsNullOrEmpty(details.PaymentDetails.Trim()) || details.PaymentMethod == "Cash")
        {
            if (details.IsCashVoucher)
            {
                // pdfService?.AddGridRow(mainGrid, pdfService?.PdfStyles?.NormalFont!, paidLabel + "Cash", $"*[{details.PaymentDetails}]");
                amountLabel = paidLabel + " Cash" + $"*[{details.PaymentDetails}]";
            }
            else
            {
                //pdfService?.AddGridRow(mainGrid, pdfService?.PdfStyles?.NormalFont!, "Made through Cash ", paidLabel + details.PaymentMethod);
                amountLabel = "Made through Cash " + paidLabel + details.PaymentMethod;
            }
        }
        else
        {
            // pdfService?.AddGridRow(mainGrid, pdfService?.PdfStyles?.NormalFont!, $"Made through {details.PaymentMethod}", $"{paidLabel} {details.PaymentMethod}, payment details {details.PaymentDetails}");

            amountLabel = $"Made through {details.PaymentMethod}, {paidLabel} {details.PaymentMethod}, payment details {details.PaymentDetails}";

        }

        // Row for Narration
        pdfService?.AddGridRowLastNoBorder(mainGrid, pdfService?.PdfStyles?.BoldFont!, "On Account of", details.Narration + ",  * " + amountLabel);

        //// Row for Narration
        //pdfService?.AddGridRowNoBorder(mainGrid, pdfService?.PdfStyles?.BoldFont!, "On Account of:", details.Narration);

        //if (string.IsNullOrEmpty(details.PaymentDetails.Trim()) || details.PaymentMethod == "Cash")
        //{
        //    if (details.IsCashVoucher)
        //    {
        //        pdfService?.AddGridRowNoBorder(mainGrid, pdfService?.PdfStyles?.NormalFont!, paidLabel + "Cash", $"*[{details.PaymentDetails}]");
        //    }
        //    else
        //    {
        //        pdfService?.AddGridRowNoBorder(mainGrid, pdfService?.PdfStyles?.NormalFont!, "Made through Cash ", paidLabel + details.PaymentMethod);
        //    }
        //}
        //else
        //{
        //    pdfService?.AddGridRowNoBorder(mainGrid, pdfService?.PdfStyles?.NormalFont!, $"Made through {details.PaymentMethod}", $"{paidLabel} {details.PaymentMethod}, payment details {details.PaymentDetails}");
        //}

        // Draw main grid. Assuming it takes up a fixed height if layoutResult is not returned.
        mainGrid.Draw(graphics, new RectangleF(bounds.X, bounds.Y + currentRelativeY, bounds.Width, bounds.Height - currentRelativeY));
        currentRelativeY += 80; // Estimate height for 3 rows + padding

        pdfService?.AddAmountGrid(graphics, ref currentRelativeY, bounds, details.Amount);
        // Add a note below the amount box.
        var noteLable = details.VoucherType == "Receipt" ? "*Note: All amount is subject \nto clearance of Cheque/DD." : "*All dispute is subject to local jurisdiction.";
        pdfService?.AddFootNote(graphics, ref currentRelativeY, bounds, noteLable);
        currentRelativeY += 30;
        pdfService?.AddSignatureToPdf(graphics, details.AuthorizedSignatory, ref currentRelativeY, bounds);
        // Construct the data string for the QR code.
        string qrData = $"Type:{details.VoucherType}|VN:{details.VoucherNumber}|Date:{details.Date:yyyy-MM-dd}|Amt:{details.Amount:N2}";

        pdfService?.AddQrCodeToPdf(graphics, qrData, ref currentRelativeY, bounds,false);
    }

    /// <summary>
    /// Print or create a single page voucher with the given details.
    /// </summary>
    /// <param name="graphics"></param>
    /// <param name="details"></param>
    /// <param name="bounds"></param>
    /// <param name="copyType"></param>
    /// <returns></returns>
    private void CreateSinglePageVoucher(PdfGraphics graphics, VoucherDetails details, RectangleF bounds, string copyType)
    {
        float currentRelativeY = 0;
        string title = $"{details.VoucherType} Voucher ({copyType})";
        //Handling Cash Voucher Title
        title = details.IsCashVoucher ? "Cash " + title : title;

        pdfService.AddHeaderToPdf(graphics, title, ref currentRelativeY, bounds);

        // --- 2. Draw Company Info & Voucher Details (Top Section) Grid ---
        pdfService.AddCompanyInfo(graphics, ref currentRelativeY, bounds, details.Date, $"VCH: {details.VoucherNumber}");

        //Adjusting to 52 to 70
        currentRelativeY += 18; // Estimate height for 2 rows + padding

        // --- 3. Main Voucher Details Grid ---
        PdfGrid mainGrid = new();
        mainGrid.Columns.Add(2);
        mainGrid.Columns[0].Width = bounds.Width * 0.20f; // Label column width
        mainGrid.Columns[1].Width = bounds.Width * 0.80f; // Value column width

        // Style for the main details grid.
        mainGrid.Style.Font = pdfService?.PdfStyles?.NormalFont!;
        mainGrid.Style.CellPadding = new PdfPaddings(5, 5, 5, 5);

        // Row for Payee/Payer Name
        string partyLabel = details.VoucherType == "Receipt" ? "Received From:" : "Paid To:";
        string paidLabel = details.VoucherType == "Receipt" ? "Received in " : "Paid in ";
        pdfService?.AddGridRow(mainGrid, pdfService?.PdfStyles?.BoldFont!, partyLabel, details.PayeeOrPayerName);

        // Row for Amount in Words
        pdfService?.AddGridRow(mainGrid, pdfService?.PdfStyles?.BoldFont!, "The Sum of","rupees "+ details.AmountInWords);

        string amountLabel = "";
       

        if (string.IsNullOrEmpty(details.PaymentDetails.Trim()) || details.PaymentMethod == "Cash")
        {
            if (details.IsCashVoucher)
            {
                // pdfService?.AddGridRow(mainGrid, pdfService?.PdfStyles?.NormalFont!, paidLabel + "Cash", $"*[{details.PaymentDetails}]");
                amountLabel = paidLabel + " Cash"  + $"*[{details.PaymentDetails}]";
            }
            else
            {
                //pdfService?.AddGridRow(mainGrid, pdfService?.PdfStyles?.NormalFont!, "Made through Cash ", paidLabel + details.PaymentMethod);
                amountLabel= "Made through Cash "+ paidLabel + details.PaymentMethod;
            }
        }
        else
        {
           // pdfService?.AddGridRow(mainGrid, pdfService?.PdfStyles?.NormalFont!, $"Made through {details.PaymentMethod}", $"{paidLabel} {details.PaymentMethod}, payment details {details.PaymentDetails}");
       
           amountLabel = $"Made through {details.PaymentMethod}, {paidLabel} {details.PaymentMethod}, payment details {details.PaymentDetails}";
        
        }

        // Row for Narration
        pdfService?.AddGridRow(mainGrid, pdfService?.PdfStyles?.BoldFont!, "On Account of", details.Narration + " " + amountLabel);
        // Draw main grid. Assuming it takes up a fixed height if layoutResult is not returned.
        mainGrid.Draw(graphics, new RectangleF(bounds.X, bounds.Y + currentRelativeY, bounds.Width, bounds.Height - currentRelativeY));
        currentRelativeY += 115; // Estimate height for 3 rows + padding

        // --- 4. Draw Amount Box ---
        pdfService?.AddAmountGrid(graphics, ref currentRelativeY, bounds, details.Amount);
        // Add a note below the amount box.
        var noteLable = details.VoucherType == "Receipt" ? "*Note: All amount is subject \nto clearance of Cheque/DD." : "*All dispute is subject to local jurisdiction.";
        pdfService?.AddFootNote(graphics, ref currentRelativeY, bounds, noteLable);
        //Adjusting to 25 t0 45
        currentRelativeY += 20; // Add some space after the note

        // --- 5. Signature Section ---
        // Authorized Signatory
        pdfService?.AddSignatureToPdf(graphics, details.AuthorizedSignatory, ref currentRelativeY, bounds);

        // --- 6. Add QR Code ---
        // Construct the data string for the QR code.

        string qrData = $"Type:{details.VoucherType}|VN:{details.VoucherNumber}|Date:{details.Date:yyyy-MM-dd}|Amt:{details.Amount:N2}";
        pdfService?.AddQrCodeToPdf(graphics, qrData, ref currentRelativeY, bounds, true);
    }

    #endregion VoucherPrinter
}