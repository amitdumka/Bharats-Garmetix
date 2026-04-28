using Garmetix.Core.Models.HRM;
using Garmetix.Core.Models.Inventory;

namespace Garmetix.Core.Interfaces
{
    public interface IReportService
    {
        Task<string> CreatePdfInvoiceAsync(Invoice invoice);
        Task<string> CreatePdfPayslipAsync(Guid employeeId, SalaryPaySlip paySlip);
        Task<string> CreatePdfVouchersAsync(string voucherNumber);
        Task<string> CreatePdfCashVoucherAsync(string cashVoucherNumber);

        Task<string> CreatePdfLedgerAsync(Guid ledgerid);
        Task<string> CreatePdfPartyLedgerAsync(Guid partyId);

        Task<string> CreatePdfMonthlyAttendaceAsync(MonthlyAttendance monthlyAttendace);
        Task<string> CreatePdfAttendaceReportAsync(Employee employee, List<Attendance> attendances);
        Task<string> CreatePdfSalaryReportAsync(Guid employeeId);

        Task<string> CreatePdfEmployeeReport(Guid employeeId);

        //Tenvative 
        Task<string> CreatePdfPurchaseReportAsync(DateTime period);
        Task<string> CreatePdfSalesReportAsync(DateTime period);
        Task<string> CreatePdfAccountingReportAsync(DateTime period);
        Task<string> CreatePdfStockReportAsync(DateTime period);

        Task<string> CreatePdfCustomerDueReportAsync(DateTime period);

        //Yearly
        Task<string> CreatePdfYearlySalesReportAsync(int year);
        Task<string> CreatePdfYearlyPurchaseReportAsync(int year);
        Task<string> CreatePdfYearlyAccountingReportAsync(int year);
        Task<string> CreatePdfYearlyStockReportAsync(int year);
        Task<string> CreatePdfYearlyCustomerDueReportAsync(int year);

        //Financial Yearl repoort
        Task<string> CreatePdfFinancialYearlySalesReportAsync(int year);
        Task<string> CreatePdfFinancialYearlyPurchaseReportAsync(int year);
        Task<string> CreatePdfFinancialYearlyAccountingReportAsync(int year);
        Task<string> CreatePdfFinancialYearlyStockReportAsync(int year);
        Task<string> CreatePdfFinancialYearlyCustomerDueReportAsync(int year);

        //Quarterly
        Task<string> CreatePdfQuarterlySalesReportAsync(int Quarter);
        Task<string> CreatePdfQuarterlyPurchaseReportAsync(int Quarter);
        Task<string> CreatePdfQuarterlyAccountingReportAsync(int Quarter);
        Task<string> CreatePdfQuarterlyStockReportAsync(int Quarter);
        Task<string>CreatePdfQuarterlyCustomerDueReportAsync(int Quarter);

        Task<string> CreatePdfMonthlySalesReportAsync(int month);
        Task<string> CreatePdfMonthlyPurchaseReportAsync(int month);
        Task<string> CreatePdfMonthlyAccountingReportAsync(int month);
        Task<string> CreatePdfMonthlyStockReportAsync(int month);




    }
}
