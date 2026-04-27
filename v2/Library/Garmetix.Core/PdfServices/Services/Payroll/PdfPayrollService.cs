using Bharat.ToolKits.Helpers; 
using Garmetix.Models.Reports;
using Garmetix.PdfServices.Base;
using Garmetix.PdfServices.Interfaces;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using PointF = Syncfusion.Drawing.PointF;

namespace Garmetix.PdfServices.Services.Payroll
{
    internal class PdfPayrollService : PdfBaseService, IPdfPayrollService
    {
        public PdfPayrollService()
        {
            //TODO: Make the PDF Service Singleton and make dispose of pdfstynling for fonts, 
            //TODO: remove the neccessity of reinitializing the service and use of fontstreamer
            _ = Init();
        }

        public async Task<string> CreatePdfSalaryPaymentAsync(Guid SalarrPaymentId, bool printDuplicate = true)
        {
            var salaryPayment = PdfService.Db.SalaryPayments.Include(x => x.Employee).First(x => x.Id == SalarrPaymentId);
            if (salaryPayment == null) { throw new Exception("Salary Payment not found"); }

            SalaryPaymentDetails details = new()
            {
                AuthorizedSignatory = salaryPayment.CreatedBy ?? "",
                Amount = salaryPayment.Amount,
                Narration = salaryPayment.Remarks ?? "",
                VouherNumber = salaryPayment.VoucherNumber,
                Date = salaryPayment.OnDate,
                PaymentMode = salaryPayment.PaymentMode.ToString(),
                StaffName = salaryPayment.Employee?.FullName ?? "",
                OnAccount = salaryPayment.SalaryComponent.ToString(),
                AmountInWords = NumberToWords.ConvertAmount((double)salaryPayment.Amount),
                Period = NumberToWords.ConvertMonthYearToString(salaryPayment.SalaryMonth),
            };

            return await CreatePdfSalaryPaymentAsync(details, printDuplicate);
        }

        public async Task<string> CreatePdfSalaryPaymentAsync(SalaryPaymentDetails details, bool duplicate = true)
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
                if (string.IsNullOrEmpty(details.AmountInWords))
                {
                    details.AmountInWords = NumberToWords.ConvertAmount((double)details.Amount);
                }
                // Calculate height for each voucher area, considering space for a separator.
                float voucherAreaHeight = (pageHeight - 30) / 2; // 30pt for separator line and text
                if (duplicate)
                {
                    // --- Draw Original Copy in the top half of the page ---
                    // The bounds define the drawable area for this specific voucher instance.
                    // X = 0, Y = 0 (relative to page's content area), Width = pageWidth, Height = voucherAreaHeight
                    await CreateSingleSalaryPaymentVoucher(graphics, details, new RectangleF(0, 0, pageWidth, voucherAreaHeight), "Original Copy");

                    // --- Draw Separator Line and Text between the two vouchers ---
                    float separatorY = voucherAreaHeight + 5; // Position below the first voucher

                    // Draw the separator line.
                    pdfService.DrawSeparatorLine(graphics, separatorY, pageWidth);

                    // --- Draw Duplicate Copy in the bottom half of the page ---
                    // The bounds for the duplicate copy start below the separator line and text.
                    await CreateSingleSalaryPaymentVoucher(graphics, details, new RectangleF(0, separatorY + 20, pageWidth, voucherAreaHeight), "Duplicate Copy");
                }
                else
                {
                    voucherAreaHeight = pageHeight - 30;
                    // --- Draw Single Copy in the full page height ---
                    await CreateSinglePageSalaryPaymentVoucher(graphics, details, new RectangleF(0, 0, pageWidth, pageHeight), "Original/Duplicate Copy");
                }
                // --- Save and Launch the PDF ---
                return await pdfService.SaveAndLaunchPdf(document, $"Salary_Payment_{details.StaffName}_{details.Period}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf", "SalaryPayment");
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during PDF creation and display an alert.
                await Shell.Current.DisplayAlertAsync("Error", $"Failed to create PDF: {ex.Message}", "OK");
                return string.Empty;
            }
            finally
            {
            }
        }

        // Report
        private Task CreateSinglePageSalaryPaymentVoucher(PdfGraphics graphics, SalaryPaymentDetails details, RectangleF bounds, string copyType)
        {
            // currentRelativeY tracks the vertical position within the *current voucher's bounds*.
            // All Y coordinates for drawing operations are calculated by adding this relative Y
            // to the absolute starting Y of the current voucher's bounds (bounds.Y).

            float currentRelativeY = 0;
            // --- 1. Draw Header for the Voucher Copy ---
            string title = $"Salary Payment Voucher ({copyType})";
            pdfService.AddHeaderToPdf(graphics, title, ref currentRelativeY, bounds);
            pdfService.AddCompanyInfo(graphics, ref currentRelativeY, bounds, details.Date, $"VCH: {details.VouherNumber}");
            //Adjusting to 52 to 70
            currentRelativeY += 18; // Estimate height for 2 rows + padding
                                    // --- 3. Main Voucher Details Grid ---
            PdfGrid mainGrid = new();
            mainGrid.Columns.Add(2);
            mainGrid.Columns[0].Width = bounds.Width * 0.20f; // Label column width
            mainGrid.Columns[1].Width = bounds.Width * 0.80f; // Value column width

            // Style for the main details grid.
            mainGrid.Style.Font = pdfService.PdfStyles!.NormalFont!;
            mainGrid.Style.CellPadding = new PdfPaddings(5, 5, 5, 5);

            // Row for Payee/Payer Name
            pdfService.AddGridRow(mainGrid, pdfService.PdfStyles.BoldFont!, "Paid To:", details.StaffName);

            // Row for Amount in Words
            pdfService.AddGridRow(mainGrid, pdfService.PdfStyles.BoldFont!, "The Sum of:", details.AmountInWords);

            // Row for Narration
            pdfService.AddGridRow(mainGrid, pdfService.PdfStyles.BoldFont!, "On Account of:", $"{details.OnAccount}, for the Period {details.Period}");

            pdfService.AddGridRow(mainGrid, pdfService.PdfStyles.NormalFont!, $"Made through {details.PaymentMode}", $"{details.Narration}");

            // Draw main grid. Assuming it takes up a fixed height if layoutResult is not returned.
            mainGrid.Draw(graphics, new RectangleF(bounds.X, bounds.Y + currentRelativeY, bounds.Width, bounds.Height - currentRelativeY));
            currentRelativeY += 115; // Estimate height for 3 rows + padding

            pdfService.AddAmountGrid(graphics, ref currentRelativeY, bounds, details.Amount);
            // Add a note below the amount box.
            var noteLable = "*All dispute is subject to local jurisdiction.";
            pdfService.AddFootNote(graphics, ref currentRelativeY, bounds, noteLable);
            //Adjusting to 25 t0 45
            currentRelativeY += 20; // Add some space after the note

            pdfService.AddSignatureToPdf(graphics, details.AuthorizedSignatory, ref currentRelativeY, bounds);
            // Construct the data string for the QR code.
            string qrData = $"Type:SalaryPayment|SN:{details.StaffName}|Date:{details.Date:yyyy-MM-dd}|Amt:{details.Amount:N2}";

            pdfService.AddQrCodeToPdf(graphics, qrData, ref currentRelativeY, bounds, true);

            return Task.CompletedTask;
        }

        private Task CreateSingleSalaryPaymentVoucher(PdfGraphics graphics, SalaryPaymentDetails details, RectangleF bounds, string copyType)
        {
            // currentRelativeY tracks the vertical position within the *current voucher's bounds*.
            // All Y coordinates for drawing operations are calculated by adding this relative Y
            // to the absolute starting Y of the current voucher's bounds (bounds.Y).

            float currentRelativeY = 0;
            // --- 1. Draw Header for the Voucher Copy ---
            string title = $"Salary Payment Voucher ({copyType})";
            pdfService.AddHeaderToPdf(graphics, title, ref currentRelativeY, bounds);
            pdfService.AddCompanyInfo(graphics, ref currentRelativeY, bounds, details.Date, $"VCH: {details.VouherNumber}");

            // --- 3. Main Voucher Details Grid ---
            PdfGrid mainGrid = new();
            mainGrid.Columns.Add(2);
            mainGrid.Columns[0].Width = bounds.Width * 0.20f; // Label column width
            mainGrid.Columns[1].Width = bounds.Width * 0.80f; // Value column width

            // Style for the main details grid.
            mainGrid.Style.Font = pdfService.PdfStyles?.NormalFont!;
            mainGrid.Style.CellPadding = new PdfPaddings(5, 5, 5, 5);

            // Row for Payee/Payer Name
            pdfService.AddGridRow(mainGrid, pdfService.PdfStyles?.BoldFont!, "Paid To:", details.StaffName);

            // Row for Amount in Words
            pdfService.AddGridRow(mainGrid, pdfService.PdfStyles?.BoldFont!, "The Sum of:", details.AmountInWords);

            // Row for Narration
            pdfService.AddGridRow(mainGrid, pdfService.PdfStyles?.BoldFont!, "On Account of:", $"{details.OnAccount}, for the Period {details.Period}");

            pdfService.AddGridRow(mainGrid, pdfService.PdfStyles?.NormalFont!, $"Made through {details.PaymentMode}", $"{details.Narration}");

            // Draw main grid. Assuming it takes up a fixed height if layoutResult is not returned.
            mainGrid.Draw(graphics, new RectangleF(bounds.X, bounds.Y + currentRelativeY, bounds.Width, bounds.Height - currentRelativeY));
            currentRelativeY += 95; // Estimate height for 3 rows + padding

            pdfService.AddAmountGrid(graphics, ref currentRelativeY, bounds, details.Amount);
            // Add a note below the amount box.
            var noteLable = "*All dispute is subject to local jurisdiction.";
            pdfService.AddFootNote(graphics, ref currentRelativeY, bounds, noteLable);

            pdfService.AddSignatureToPdf(graphics, details.AuthorizedSignatory, ref currentRelativeY, bounds);
            // Construct the data string for the QR code.
            string qrData = $"Type:SalaryPayment|SN:{details.StaffName}|Date:{details.Date:yyyy-MM-dd}|Amt:{details.Amount:N2}";

            pdfService.AddQrCodeToPdf(graphics, qrData, ref currentRelativeY, bounds);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Create PDF for Attendance Report
        /// </summary>
        /// <param name="attendance"></param>
        /// <returns></returns>
        public async Task<string> CreatePdfAttendenceReport(AttendanceReport attendance)
        {
            if (attendance == null) return "Error : Attendane not found!";
            await Init();
            using var document = PdfService.CreateNewPdfDocument(PdfPageSize.A4);
            // 2. Add the first page to the document.
            PdfPage page = document.Pages.Add();
            PdfGraphics graphics = page.Graphics;

            float currentY = 10;
            currentY = PdfService.CompanyReportTitle(ref graphics, currentY, "Attendance Report", page.GetClientSize().Width);
            currentY = PdfService.PartyInformation(ref graphics, currentY, "Employee Infomation", attendance.EmployeeName, attendance.Department.ToString(), attendance.Mobile, attendance.Email, "", "", false);
            graphics.DrawLine(PdfPens.Red, 0, currentY, page.GetClientSize().Width, currentY);
            currentY += 10;
            graphics.DrawString($"Period: {attendance.Date:Y}", PdfStyles.HeadingFont, PdfStyles.BlackBrush, new PointF(0, currentY));
            currentY += 20;
            graphics.DrawLine(PdfPens.Red, 0, currentY, page.GetClientSize().Width, currentY);
            currentY += 10;

            // ---   Details Table ---
            PdfGrid detailGrid = new();
            detailGrid.Style.CellPadding = new PdfPaddings(5, 5, 5, 5);
            detailGrid.Columns.Add(6);
            detailGrid.Columns[0].Width = 70;  // SN
            detailGrid.Columns[1].Width = 100; // Date
            detailGrid.Columns[2].Width = 80;  // EntryTime
            detailGrid.Columns[3].Width = 70;  // Remarks
            detailGrid.Columns[4].Width = 70;  // Status

            detailGrid.Headers.Add(1);
            PdfGridRow header = detailGrid.Headers[0];
            header.Cells[0].Value = "#";
            header.Cells[1].Value = "Date";
            header.Cells[2].Value = "Status";
            header.Cells[3].Value = "Entry Time";
            header.Cells[4].Value = "Remarks";

            foreach (PdfGridCell cell in header.Cells)
            {
                cell.Style.Font = PdfStyles.HeadingFont;
                cell.Style.BackgroundBrush = PdfStyles.TableHeaderBrush;
                cell.Style.TextBrush = PdfStyles.BlackBrush;
                cell.Style.Borders.All = new PdfPen(PdfStyles.BlackBrush, 0.5f);
                cell.StringFormat = new PdfStringFormat { Alignment = PdfTextAlignment.Center, LineAlignment = PdfVerticalAlignment.Middle };
                cell.Style.CellPadding = new PdfPaddings(0, 0, 0, 10); ;
            }
            int RowCount = 0;
            if (attendance.Attendances != null)
                foreach (var info in attendance.Attendances!)
                {
                    PdfGridRow dataRow = detailGrid.Rows.Add();
                    dataRow.Cells[1].Value = info.Date.ToShortDateString();
                    dataRow.Cells[2].Value = info.Status.ToString();
                    dataRow.Cells[3].Value = info.EntryTimne;
                    dataRow.Cells[4].Value = info.Remarks;
                    dataRow.Cells[0].Value = ++RowCount;

                    for (int i = 0; i < dataRow.Cells.Count; i++)
                    {
                        dataRow.Cells[i].Style.Font = PdfStyles.RegularFont;
                        dataRow.Cells[i].Style.TextBrush = PdfStyles.BlackBrush;
                        dataRow.Cells[i].Style.Borders.All = new PdfPen(new PdfColor(200, 200, 200), 0.1f);
                        dataRow.Cells[i].StringFormat = (i >= 3) ?
                            new PdfStringFormat { Alignment = PdfTextAlignment.Right, LineAlignment = PdfVerticalAlignment.Middle } :
                            new PdfStringFormat { Alignment = PdfTextAlignment.Left, LineAlignment = PdfVerticalAlignment.Middle };
                    }
                    if (detailGrid.Rows.IndexOf(dataRow) % 2 == 0)
                    {
                        foreach (PdfGridCell cell in dataRow.Cells)
                        {
                            cell.Style.BackgroundBrush = PdfStyles.TableEvenRowBrush;
                        }
                    }
                }

            // Draw the grid. This handles page breaks automatically.
            PdfLayoutResult result = detailGrid.Draw(page, new PointF(0, currentY)); // ======================= FIX STARTS HERE =======================

            // Get the last page and the final Y position from the layout result.
            // This is the key to drawing on the correct page after the table.
            PdfPage lastPage = result.Page;
            graphics = lastPage.Graphics; // IMPORTANT: Switch graphics context to the last page
            currentY = result.Bounds.Bottom + 20; // Update Y position based on where the grid finished graphics.DrawLine(PdfPens.Red, 0, currentY, page.GetClientSize().Width, currentY);
            currentY += 20;

            graphics.DrawString("--------------------- Summary---------------  ", PdfStyles.HeadingFont, PdfStyles.BlackBrush, new PointF(250, currentY), new PdfStringFormat { Alignment = PdfTextAlignment.Center, LineAlignment = PdfVerticalAlignment.Middle });

            PdfGrid summaryGrid = new PdfGrid();
            summaryGrid.Columns.Add(2);
            summaryGrid.Columns[0].Width = 0.5f;
            summaryGrid.Columns[1].Width = 0.5f;

            var srow1 = summaryGrid.Rows.Add();

            srow1.Cells[0].Value = $"Presnet:{attendance?.Present}";
            srow1.Cells[1].Value = $"Absent:{attendance?.Absent}";

            var srow2 = summaryGrid.Rows.Add();

            srow2.Cells[0].Value = $"Working Day:{attendance?.WorkingDays}";
            srow2.Cells[1].Value = $"Days In Month:{attendance?.DaysInMonth}";
            var srow3 = summaryGrid.Rows.Add();

            srow3.Cells[0].Value = $"Count:{attendance?.Attendances?.Count}";
            if (attendance?.Attendances?.Count != attendance?.DaysInMonth)
                srow3.Cells[1].Value = $"Missing Days:{(attendance?.DaysInMonth - attendance?.Attendances?.Count)}";

            currentY += 20;
            graphics.DrawLine(PdfPens.Red, 0, currentY, page.GetClientSize().Width, currentY);
            result = summaryGrid.Draw(page, new PointF(0, currentY));

            // 3. Save the document to a MemoryStream.
            MemoryStream stream = new();
            document.Save(stream);
            stream.Position = 0; // Reset stream position

            //Save to disk
            // 3. Save and Open the PDF (Platform-specific implementation needed)
            string fileName = $"AttendanceReport{attendance?.EmployeeName.Replace(" ", "_")}_{attendance?.Date:yyyyMM}_{DateTime.Now:yyyyMMddHHmmss}.pdf";

            string filePath = Path.Combine(FileSystem.CacheDirectory, "Bharat-Garmetix", Preferences.Get("CompanyName", "AadwikaFashion").Replace(" ", "_"), "AttendanceReports", fileName); // Use CacheDirectory for temporary storage

            // For Android, iOS, Mac Catalyst, Windows
            // You might need permission requests for saving to external storage on Android
            await File.WriteAllBytesAsync(filePath, stream.ToArray());
            return filePath;
        }

        public async Task<string> CreatePdfEmployeeReport()
        {
            await Init();
            throw new NotImplementedException();
        }

        /// <summary>
        ///  Create PDF for Attendance Report Monthly Attendance Summary
        /// </summary>
        /// <param name="attendance"></param>
        /// <returns></returns>
        public async Task<string> CreatePdfMontlyAttendence(MonthlyAttendanceReport attendance)
        {
            await Init();
            using var document = PdfService.CreateNewPdfDocument(PdfPageSize.A4);
            // 2. Add the first page to the document.
            PdfPage page = document.Pages.Add();
            PdfGraphics graphics = page.Graphics;

            float currentY = 10;
            currentY = PdfService.CompanyReportTitle(ref graphics, currentY, "Attendance Report", page.GetClientSize().Width);
            currentY = PdfService.PartyInformation(ref graphics, currentY, "Employee Infomation", attendance.EmployeeName, attendance.Department.ToString(), attendance.Mobile, attendance.Email, "", "", false);
            graphics.DrawLine(PdfPens.Red, 0, currentY, page.GetClientSize().Width, currentY);
            currentY += 10;
            graphics.DrawString($"Period: {attendance.Date:Y}", PdfStyles.HeadingFont, PdfStyles.BlackBrush, new PointF(0, currentY));
            currentY += 20;
            graphics.DrawLine(PdfPens.Red, 0, currentY, page.GetClientSize().Width, currentY);
            currentY += 10;

            // ---   Details Table ---
            PdfGrid detailGrid = new();
            detailGrid.Style.CellPadding = new PdfPaddings(5, 5, 5, 5);
            detailGrid.Columns.Add(6);
            detailGrid.Columns[0].Width = 70;  // SN
            detailGrid.Columns[1].Width = 100; // Date
            detailGrid.Columns[2].Width = 80;  // EntryTime
            detailGrid.Columns[3].Width = 70;  // Remarks
            detailGrid.Columns[4].Width = 70;  // Status

            detailGrid.Headers.Add(1);
            PdfGridRow header = detailGrid.Headers[0];
            header.Cells[0].Value = "#";
            header.Cells[1].Value = "Status";
            header.Cells[2].Value = "Nos";
            header.Cells[2].Value = "Count";

            foreach (PdfGridCell cell in header.Cells)
            {
                cell.Style.Font = PdfStyles.HeadingFont;
                cell.Style.BackgroundBrush = PdfStyles.TableHeaderBrush;
                cell.Style.TextBrush = PdfStyles.BlackBrush;
                cell.Style.Borders.All = new PdfPen(PdfStyles.BlackBrush, 0.5f);
                cell.StringFormat = new PdfStringFormat { Alignment = PdfTextAlignment.Center, LineAlignment = PdfVerticalAlignment.Middle };
                cell.Style.CellPadding = new PdfPaddings(0, 0, 0, 10); ;
            }

            var row1 = detailGrid.Rows.Add();
            row1.Cells[0].Value = "1";
            row1.Cells[1].Value = "Present";
            row1.Cells[2].Value = attendance.Present;
            row1.Cells[3].Value = attendance.Present;

            var row2 = detailGrid.Rows.Add();
            row2.Cells[0].Value = "2";
            row2.Cells[1].Value = "Half Day";
            row2.Cells[2].Value = attendance.HalfDay;
            row2.Cells[3].Value = attendance.HalfDay / 2;

            var row3 = detailGrid.Rows.Add();
            row3.Cells[0].Value = "3";
            row3.Cells[1].Value = "Holiday";
            row3.Cells[2].Value = attendance.Holiday;
            row3.Cells[3].Value = attendance.Holiday;

            var row5 = detailGrid.Rows.Add();
            row5.Cells[0].Value = "4";
            row5.Cells[1].Value = "Paid Leave";
            row5.Cells[2].Value = attendance.PaidLeave;
            row5.Cells[3].Value = attendance.PaidLeave;

            var row4 = detailGrid.Rows.Add();
            row4.Cells[0].Value = "5";
            row4.Cells[1].Value = "Casual Leave";
            row4.Cells[2].Value = attendance.CasualLeave;
            row4.Cells[3].Value = $"-{attendance.CasualLeave}";

            var row6 = detailGrid.Rows.Add();
            row6.Cells[0].Value = "6";
            row6.Cells[1].Value = "Absent";
            row6.Cells[2].Value = attendance.Absent;
            row6.Cells[3].Value = $"-{attendance.Absent}";

            var row7 = detailGrid.Rows.Add();
            row7.Cells[0].Value = "7";
            row7.Cells[1].Value = "Weekly Leave";
            row7.Cells[2].Value = attendance.WeeklyOff;
            row7.Cells[3].Value = "0";

            var row = detailGrid.Rows.Add();
            row.Cells[0].Value = "8";
            row.Cells[1].Value = "Sunday";
            row.Cells[2].Value = attendance.Sunday;
            row.Cells[3].Value = attendance.Sunday;

            // Draw the grid. This handles page breaks automatically.
            PdfLayoutResult result = detailGrid.Draw(page, new PointF(0, currentY));
            currentY += result.Bounds.Bottom + 20;
            //graphics.DrawLine(new PdfPen( PdfStyles.BlackBrush,2), new PointF(0,currentY),new PointF(page.GetClientSize().Width,currentY));
            graphics.DrawLine(PdfPens.Red, 0, currentY, page.GetClientSize().Width, currentY);
            currentY += 20;
            graphics.DrawString("--------------------- Summary---------------  ", PdfStyles.HeadingFont, PdfStyles.BlackBrush, new PointF(250, currentY), new PdfStringFormat { Alignment = PdfTextAlignment.Center, LineAlignment = PdfVerticalAlignment.Middle });

            PdfGrid summaryGrid = new PdfGrid();
            summaryGrid.Columns.Add(2);
            summaryGrid.Columns[0].Width = 0.5f;
            summaryGrid.Columns[1].Width = 0.5f;

            var srow1 = summaryGrid.Rows.Add();

            srow1.Cells[0].Value = $"Presnet:{attendance.BillableDays}";
            srow1.Cells[1].Value = $"Absent:{attendance.NoOfAbsentDays}";

            var srow2 = summaryGrid.Rows.Add();

            srow2.Cells[0].Value = $"Working Day:{attendance.WorkingDays}";
            srow2.Cells[1].Value = $"Days In Month:{attendance.DaysInMonth}";
            currentY += 20;
            graphics.DrawLine(PdfPens.Red, 0, currentY, page.GetClientSize().Width, currentY);
            result = summaryGrid.Draw(page, new PointF(0, currentY));

            // 3. Save the document to a MemoryStream.
            MemoryStream stream = new();
            document.Save(stream);
            stream.Position = 0; // Reset stream position

            //Save to disk
            // 3. Save and Open the PDF (Platform-specific implementation needed)
            string fileName = $"MonthlyAttendance_{attendance.EmployeeName.Replace(" ", "_")}_{attendance.Date:yyyyMM}_{DateTime.Now:yyyyMMddHHmmss}.pdf";

            string filePath = Path.Combine(FileSystem.CacheDirectory, "Bharat-Garmetix", Preferences.Get("CompanyName", "AadwikaFashion").Replace(" ", "_"), "MonthlyAttendance", fileName); // Use CacheDirectory for temporary storage

            // For Android, iOS, Mac Catalyst, Windows
            // You might need permission requests for saving to external storage on Android
            await File.WriteAllBytesAsync(filePath, stream.ToArray());
            return filePath;
        }

        public Task<string> CreatePdfPaySlip(PaySlipDetails details)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreatePdfPaySlip(Guid paySlipId)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreatePdfSalaryReport()
        {
            throw new NotImplementedException();
        }
    }
}