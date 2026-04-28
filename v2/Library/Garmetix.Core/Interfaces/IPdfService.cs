using Garmetix.Core.VM.Reports;
using Garmetix.Models.Reports;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;

namespace Garmetix.Core.Interfaces
{
    //Final Version
    public interface IPdfAccountingService
    {
        Task<string> CreatePdfMonthlyCashFlowAsync(DateTime periodStart, DateTime periodEnd);
        Task<string> CreatePdfQuarterlyCashFlowAsync(DateTime periodStart, DateTime periodEnd);
        Task<string> CreatePdfYearlyCashFlowAsync(DateTime periodStart, DateTime periodEnd);

        Task<string> CreatePdfBalanceSheetAsync(DateTime periodStart, DateTime periodEnd);
        Task<string> CreatePdfTrialBalanceAsync(DateTime periodStart, DateTime periodEnd);

        //Ledger and Party

        Task<string> CreatePdfLedgerReportAsync(DateTime periodStart, DateTime periodEnd);
        Task<string> CreatePdfPartyLedgerReportAsync(DateTime periodStart, DateTime periodEnd);
        MemoryStream GenerateGeneralLedgerPdf(GeneralLedger partyLedger);
        MemoryStream GeneratePartyLedgerPdf(PartyLedger partyLedger);
    }
    internal interface IPdfService
    {
        PdfDocument SetupDocumentSettings(bool printDuplicate = true);

        Task<string> SaveAndLaunchPdf(PdfDocument document, string fileName, string directory = "Vouchers", bool externalSave = false);

        void AddGridRowNoBorder(PdfGrid grid, PdfFont labelFont, string label, string value);

        void AddGridRow(PdfGrid grid, PdfFont labelFont, string label, string value);

        void DrawSeparatorLine(PdfGraphics graphics, float height, float pageWidth);
    }

    public interface IPdfVoucherService
    {
        Task<string> CreatePdfVoucherAsync(string voucherNumber, bool duplicate = false);

        Task<string> CreatePdfCashVoucherAsync(string cashVoucherNumber, bool duplicate = false);

        Task<string> CreatePdfExpenseVoucherReportAsync(DateTime periodStart, DateTime periodEnd, bool cashVoucher = false);

        Task<string> CreatePdfPaymentVoucherReportAsync(DateTime periodStart, DateTime periodEnd, bool cashVoucher = false);

        Task<string> CreatePdfReceiptVoucherReportAsync(DateTime periodStart, DateTime periodEnd, bool cashVoucher = false);
        Task<string> CreatePdfCashVoucherAsync(VoucherDetails details, bool duplicate = false);
        Task<string> CreatePdfVoucherAsync(VoucherDetails details, bool duplicate = false);
    }


    public interface IPdfPayrollService
    {
        Task<string> CreatePdfPaySlip(Guid paySlipNumberId);
        Task<string> CreatePdfSalaryReport();
        Task<string> CreatePdfEmployeeReport();
        Task<string> CreatePdfMontlyAttendence(MonthlyAttendanceReport attendance);
        Task<string> CreatePdfAttendenceReport(AttendanceReport attendance);
        Task<string> CreatePdfSalaryPaymentAsync(SalaryPaymentDetails details, bool duplicate = true);
        Task<string> CreatePdfSalaryPaymentAsync(Guid SalarrPaymentId, bool printDuplicate = true);
        Task<string> CreatePdfPaySlip(PaySlipDetails details);
    }

}