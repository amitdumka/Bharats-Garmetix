using Garmetix.Core.Interfaces;
using Garmetix.Models.Reports;
using Garmetix.PdfServices.Base;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using PointF = Syncfusion.Drawing.PointF;

namespace Garmetix.PdfServices.Services.Accounting;

internal class PdfAccountingService : PdfBaseService, IPdfAccountingService
{
    public PdfAccountingService()
    {
        _ = Init();
    }

     
    public Task<string> CreatePdfLedgerReportAsync(DateTime periodStart, DateTime periodEnd)
    {
        throw new NotImplementedException();
    }

    public Task<string> CreatePdfPartyLedgerReportAsync(DateTime periodStart, DateTime periodEnd)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Generates a PDF document for the given PartyLedger data.
    /// </summary>
    /// <param name="partyLedger">The PartyLedger data to print.</param>
    /// <returns>A MemoryStream containing the generated PDF.</returns>
    public MemoryStream GenerateGeneralLedgerPdf(GeneralLedger partyLedger)
    {
        // 1. Create a new PDF document.
        using PdfDocument document = new();

        // Set page settings (A4 size, margins)
        document.PageSettings.Orientation = PdfPageOrientation.Portrait;
        document.PageSettings.Margins.All = 20;
        document.PageSettings.Size = PdfPageSize.A4;

        // 2. Add the first page to the document.
        PdfPage page = document.Pages.Add();
        PdfGraphics graphics = page.Graphics;

        float currentY = 10;

        // --- Company/Report Title ---
        graphics.DrawString("  Ledger   ", titleFont, blackBrush, new PointF(250, currentY), new PdfStringFormat { Alignment = PdfTextAlignment.Center, LineAlignment = PdfVerticalAlignment.Middle });
        currentY += titleFont.Height - 2;
        graphics.DrawLine(PdfPens.Red, 0, currentY, page.GetClientSize().Width, currentY);
        graphics.DrawString("Aadwika Fashion", titleFont, blackBrush, new PointF(180, currentY));
        currentY += titleFont.Height;
        graphics.DrawString("Bhagalpur Road, Dumka (Jharkhand) 84101", headingFont, blackBrush, new PointF(130, currentY));
        currentY += headingFont.Height + 5;
        graphics.DrawLine(PdfPens.Red, 0, currentY, page.GetClientSize().Width, currentY);
        currentY += 5;

        // --- Party Information ---
        graphics.DrawString("Ledger Information:", headingFont, blackBrush, new PointF(0, currentY));
        currentY += headingFont.Height + 5;

        float labelWidth = 80;
        float valueX = labelWidth + 10;
        graphics.DrawString("Ledger Name:", regularFont, grayBrush, new PointF(0, currentY));
        graphics.DrawString(partyLedger.LedgerName, regularFont, blackBrush, new PointF(valueX, currentY));
        currentY += regularFont.Height + 5;

        graphics.DrawString("Ledger Type:", regularFont, grayBrush, new PointF(0, currentY));
        graphics.DrawString(partyLedger.LegerType, regularFont, blackBrush, new PointF(valueX, currentY));
        currentY += regularFont.Height + 15;

        // --- Ledger Details Table ---
        PdfGrid ledgerGrid = new();
        ledgerGrid.Style.CellPadding = new PdfPaddings(5, 5, 5, 5);
        ledgerGrid.Columns.Add(6);
        ledgerGrid.Columns[0].Width = 70;  // Date
        ledgerGrid.Columns[1].Width = 150; // Particulars
        ledgerGrid.Columns[2].Width = 80;  // Payment Mode
        ledgerGrid.Columns[3].Width = 70;  // Debit
        ledgerGrid.Columns[4].Width = 70;  // Credit
        ledgerGrid.Columns[5].Width = 70;  // Balance

        ledgerGrid.Headers.Add(1);
        PdfGridRow header = ledgerGrid.Headers[0];
        header.Cells[0].Value = "Date";
        header.Cells[1].Value = "Particulars";
        header.Cells[2].Value = "Payment Mode";
        header.Cells[3].Value = "In";
        header.Cells[4].Value = "Out";
        header.Cells[5].Value = "Balance";

        foreach (PdfGridCell cell in header.Cells)
        {
            cell.Style.Font = headingFont;
            cell.Style.BackgroundBrush = tableHeaderBrush;
            cell.Style.TextBrush = blackBrush;
            cell.Style.Borders.All = new PdfPen(blackBrush, 0.5f);
            cell.StringFormat = new PdfStringFormat { Alignment = PdfTextAlignment.Center, LineAlignment = PdfVerticalAlignment.Middle };
            cell.Style.CellPadding = new PdfPaddings(0, 0, 0, 5); ;
        }

        foreach (var info in partyLedger.Ledgers)
        {
            PdfGridRow dataRow = ledgerGrid.Rows.Add();
            dataRow.Cells[0].Value = info.Date.ToShortDateString();
            dataRow.Cells[1].Value = info.Particulars;
            dataRow.Cells[2].Value = info.PaymentMode;
            dataRow.Cells[3].Value = info.In.ToString("N2");
            dataRow.Cells[4].Value = info.Out.ToString("N2");
            // dataRow.Cells[5].Value = info.Balance.ToString("N2");
            dataRow.Cells[5].Value = info.Balance >= 0 ? $"Rs. {info.Balance:N2} DR" : $"Rs. {Math.Abs(info.Balance):N2} CR";
            for (int i = 0; i < dataRow.Cells.Count; i++)
            {
                dataRow.Cells[i].Style.Font = regularFont;
                dataRow.Cells[i].Style.TextBrush = blackBrush;
                dataRow.Cells[i].Style.Borders.All = new PdfPen(new PdfColor(200, 200, 200), 0.1f);
                dataRow.Cells[i].StringFormat = (i >= 3) ?
                    new PdfStringFormat { Alignment = PdfTextAlignment.Right, LineAlignment = PdfVerticalAlignment.Middle } :
                    new PdfStringFormat { Alignment = PdfTextAlignment.Left, LineAlignment = PdfVerticalAlignment.Middle };
            }
            if (ledgerGrid.Rows.IndexOf(dataRow) % 2 == 0)
            {
                foreach (PdfGridCell cell in dataRow.Cells)
                {
                    cell.Style.BackgroundBrush = tableEvenRowBrush;
                }
            }
        }

        // Draw the grid. This handles page breaks automatically.
        PdfLayoutResult result = ledgerGrid.Draw(page, new PointF(0, currentY));

        // ======================= FIX STARTS HERE =======================

        // Get the last page and the final Y position from the layout result.
        // This is the key to drawing on the correct page after the table.
        PdfPage lastPage = result.Page;
        graphics = lastPage.Graphics; // IMPORTANT: Switch graphics context to the last page
        currentY = result.Bounds.Bottom + 20; // Update Y position based on where the grid finished

        // --- Final Balance ---
        // Check if there's enough space on the *last page*. If not, add a new page.
        if (currentY + balanceFont.Height + 20 > lastPage.GetClientSize().Height)
        {
            lastPage = document.Pages.Add(); // Add a new page
            graphics = lastPage.Graphics;    // Get graphics for the new page
            currentY = 20;                   // Reset Y position for the new page
        }

        decimal finalBalance = partyLedger.Ledgers.Count != 0 ? partyLedger.Ledgers.Last().Balance : 0;
       // string balanceText = $"Closing Balance: {finalBalance:N2}";
        string balanceText = finalBalance >= 0 ? $"Closing Balance: Rs. {finalBalance:N2} DR" : $"Closing Balance: Rs. {Math.Abs(finalBalance):N2} CR";
        // string balanceText = $"Closing Balance: {finalBalance:N2}";

        // Draw a separator line before the Balance
        graphics.DrawLine(new PdfPen(blackBrush, 1), new PointF(0, currentY), new PointF(lastPage.GetClientSize().Width, currentY));
        currentY += 10;

        // Create a layout rectangle for robust right-alignment
        RectangleF balanceRect = new(0, currentY, lastPage.GetClientSize().Width, balanceFont.Height + 5);

        // Draw the final Balance prominently using the correct graphics object
        graphics.DrawString(balanceText, balanceFont, new PdfSolidBrush(new PdfColor(192, 0, 0)), balanceRect,
            new PdfStringFormat { Alignment = PdfTextAlignment.Right, LineAlignment = PdfVerticalAlignment.Middle });

        // ======================== FIX ENDS HERE ========================

        // 3. Save the document to a MemoryStream.
        MemoryStream stream = new();
        document.Save(stream);
        stream.Position = 0; // Reset stream position

        return stream;
    }

    /// <summary>
    /// Generates a PDF document for the given PartyLedger data.
    /// </summary>
    /// <param name="partyLedger">The PartyLedger data to print.</param>
    /// <returns>A MemoryStream containing the generated PDF.</returns>
    public MemoryStream GeneratePartyLedgerPdf(PartyLedger partyLedger)
    {
        // 1. Create a new PDF document.
        using PdfDocument document = new();

        // Set page settings (A4 size, margins)
        document.PageSettings.Orientation = PdfPageOrientation.Portrait;
        document.PageSettings.Margins.All = 20;
        document.PageSettings.Size = PdfPageSize.A4;

        // 2. Add the first page to the document.
        PdfPage page = document.Pages.Add();
        PdfGraphics graphics = page.Graphics;

        float currentY = 10;

        // --- Company/Report Title ---
        graphics.DrawString("  Ledger   ", titleFont, blackBrush, new PointF(250, currentY), new PdfStringFormat { Alignment = PdfTextAlignment.Center, LineAlignment = PdfVerticalAlignment.Middle });
        currentY += titleFont.Height - 2;
        graphics.DrawLine(PdfPens.Red, 0, currentY, page.GetClientSize().Width, currentY);
        graphics.DrawString("Aadwika Fashion", titleFont, blackBrush, new PointF(180, currentY));
        currentY += titleFont.Height;
        graphics.DrawString("Bhagalpur Road, Dumka (Jharkhand) 84101", headingFont, blackBrush, new PointF(130, currentY));
        currentY += headingFont.Height + 5;
        graphics.DrawLine(PdfPens.Red, 0, currentY, page.GetClientSize().Width, currentY);
        currentY += 5;

        // --- Party Information ---
        graphics.DrawString("Party Information:", headingFont, blackBrush, new PointF(0, currentY));
        currentY += headingFont.Height + 5;

        float labelWidth = 80;
        float valueX = labelWidth + 10;
        graphics.DrawString("Party Name:", regularFont, grayBrush, new PointF(0, currentY));
        graphics.DrawString(partyLedger.PartyName, regularFont, blackBrush, new PointF(valueX, currentY));
        currentY += regularFont.Height + 5;
        graphics.DrawString("Address:", regularFont, grayBrush, new PointF(0, currentY));
        graphics.DrawString(partyLedger.Address, regularFont, blackBrush, new PointF(valueX, currentY));
        currentY += regularFont.Height + 5;
        graphics.DrawString("Phone:", regularFont, grayBrush, new PointF(0, currentY));
        graphics.DrawString(partyLedger.Phone, regularFont, blackBrush, new PointF(valueX, currentY));
        currentY += regularFont.Height + 5;
        graphics.DrawString("Email:", regularFont, grayBrush, new PointF(0, currentY));
        graphics.DrawString(partyLedger.Email, regularFont, blackBrush, new PointF(valueX, currentY));
        currentY += regularFont.Height + 5;
        graphics.DrawString("GSTIN:", regularFont, grayBrush, new PointF(0, currentY));
        graphics.DrawString(partyLedger.Gstin, regularFont, blackBrush, new PointF(valueX, currentY));
        currentY += regularFont.Height + 5;
        graphics.DrawString("Ledger Type:", regularFont, grayBrush, new PointF(0, currentY));
        graphics.DrawString(partyLedger.LegerType, regularFont, blackBrush, new PointF(valueX, currentY));
        currentY += regularFont.Height + 15;

        // --- Ledger Details Table ---
        PdfGrid ledgerGrid = new();
        ledgerGrid.Style.CellPadding = new PdfPaddings(5, 5, 5, 5);
        ledgerGrid.Columns.Add(6);
        ledgerGrid.Columns[0].Width = 70;  // Date
        ledgerGrid.Columns[1].Width = 150; // Particulars
        ledgerGrid.Columns[2].Width = 80;  // Payment Mode
        ledgerGrid.Columns[3].Width = 70;  // Debit
        ledgerGrid.Columns[4].Width = 70;  // Credit
        ledgerGrid.Columns[5].Width = 70;  // Balance

        ledgerGrid.Headers.Add(1);
        PdfGridRow header = ledgerGrid.Headers[0];
        header.Cells[0].Value = "Date";
        header.Cells[1].Value = "Particulars";
        header.Cells[2].Value = "Payment Mode";
        header.Cells[3].Value = "Debit";
        header.Cells[4].Value = "Credit";
        header.Cells[5].Value = "Balance";

        foreach (PdfGridCell cell in header.Cells)
        {
            cell.Style.Font = headingFont;
            cell.Style.BackgroundBrush = tableHeaderBrush;
            cell.Style.TextBrush = blackBrush;
            cell.Style.Borders.All = new PdfPen(blackBrush, 0.5f);
            cell.StringFormat = new PdfStringFormat { Alignment = PdfTextAlignment.Center, LineAlignment = PdfVerticalAlignment.Middle };
            cell.Style.CellPadding = new PdfPaddings(0, 0, 0, 10); ;
        }

        foreach (var info in partyLedger.Ledgers)
        {
            PdfGridRow dataRow = ledgerGrid.Rows.Add();
            dataRow.Cells[0].Value = info.Date.ToShortDateString();
            dataRow.Cells[1].Value = info.Particulars;
            dataRow.Cells[2].Value = info.PaymentMode;
            dataRow.Cells[3].Value = info.In.ToString("N2");
            dataRow.Cells[4].Value = info.Out.ToString("N2");
            dataRow.Cells[5].Value = info.Balance>=0? $"Rs. {info.Balance:N2} DR" : $"Rs. {Math.Abs(info.Balance):N2} CR";

            for (int i = 0; i < dataRow.Cells.Count; i++)
            {
                dataRow.Cells[i].Style.Font = regularFont;
                dataRow.Cells[i].Style.TextBrush = blackBrush;
                dataRow.Cells[i].Style.Borders.All = new PdfPen(new PdfColor(200, 200, 200), 0.1f);
                dataRow.Cells[i].StringFormat = (i >= 3) ?
                    new PdfStringFormat { Alignment = PdfTextAlignment.Right, LineAlignment = PdfVerticalAlignment.Middle } :
                    new PdfStringFormat { Alignment = PdfTextAlignment.Left, LineAlignment = PdfVerticalAlignment.Middle };
            }
            if (ledgerGrid.Rows.IndexOf(dataRow) % 2 == 0)
            {
                foreach (PdfGridCell cell in dataRow.Cells)
                {
                    cell.Style.BackgroundBrush = tableEvenRowBrush;
                }
            }
        }

        // Draw the grid. This handles page breaks automatically.
        PdfLayoutResult result = ledgerGrid.Draw(page, new PointF(0, currentY));

        // ======================= FIX STARTS HERE =======================

        // Get the last page and the final Y position from the layout result.
        // This is the key to drawing on the correct page after the table.
        PdfPage lastPage = result.Page;
        graphics = lastPage.Graphics; // IMPORTANT: Switch graphics context to the last page
        currentY = result.Bounds.Bottom + 20; // Update Y position based on where the grid finished

        // --- Final Balance ---
        // Check if there's enough space on the *last page*. If not, add a new page.
        if (currentY + balanceFont.Height + 20 > lastPage.GetClientSize().Height)
        {
            lastPage = document.Pages.Add(); // Add a new page
            graphics = lastPage.Graphics;    // Get graphics for the new page
            currentY = 20;                   // Reset Y position for the new page
        }

        decimal finalBalance = partyLedger.Ledgers.Count != 0 ? partyLedger.Ledgers.Last().Balance : 0;
        string balanceText = finalBalance >= 0 ? $"Closing Balance: Rs. {finalBalance:N2} DR" : $"Closing Balance: Rs. {Math.Abs(finalBalance):N2} CR";
       // string balanceText = $"Closing Balance: {finalBalance:N2}";

        // Draw a separator line before the Balance
        graphics.DrawLine(new PdfPen(blackBrush, 1), new PointF(0, currentY), new PointF(lastPage.GetClientSize().Width, currentY));
        currentY += 10;

        // Create a layout rectangle for robust right-alignment
        RectangleF balanceRect = new(0, currentY, lastPage.GetClientSize().Width, balanceFont.Height + 5);

        // Draw the final Balance prominently using the correct graphics object
        graphics.DrawString(balanceText, balanceFont, new PdfSolidBrush(new PdfColor(192, 0, 0)), balanceRect,
            new PdfStringFormat { Alignment = PdfTextAlignment.Right, LineAlignment = PdfVerticalAlignment.Middle });

        // ======================== FIX ENDS HERE ========================

        // 3. Save the document to a MemoryStream.
        MemoryStream stream = new();
        document.Save(stream);
        stream.Position = 0; // Reset stream position

        return stream;
    }

    public Task<string> CreatePdfBalanceSheetAsync(DateTime periodStart, DateTime periodEnd)
    {
        throw new NotImplementedException();
    }

    public Task<string> CreatePdfMonthlyCashFlowAsync(DateTime periodStart, DateTime periodEnd)
    {
        throw new NotImplementedException();
    }

    public Task<string> CreatePdfQuarterlyCashFlowAsync(DateTime periodStart, DateTime periodEnd)
    {
        throw new NotImplementedException();
    }

    public Task<string> CreatePdfTrialBalanceAsync(DateTime periodStart, DateTime periodEnd)
    {
        throw new NotImplementedException();
    }

    public Task<string> CreatePdfYearlyCashFlowAsync(DateTime periodStart, DateTime periodEnd)
    {
        throw new NotImplementedException();
    }
}