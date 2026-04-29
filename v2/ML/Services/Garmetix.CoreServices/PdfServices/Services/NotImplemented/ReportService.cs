using Garmetix.Core.Interfaces;
using Garmetix.Core.Models.HRM;
using Garmetix.Core.Models.Inventory;

 

namespace Garmetix.PdfServices.Services.NotImplemented
{
    public partial class ReportService : IReportService
    {
        public Task<string> CreatePdfAccountingReportAsync(DateTime period)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreatePdfAttendaceReportAsync(Employee employee, List<Attendance> attendances)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreatePdfCashVoucherAsync(string cashVoucherNumber)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreatePdfCustomerDueReportAsync(DateTime period)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreatePdfEmployeeReport(Guid employeeId)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreatePdfFinancialYearlyAccountingReportAsync(int year)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreatePdfFinancialYearlyCustomerDueReportAsync(int year)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreatePdfFinancialYearlyPurchaseReportAsync(int year)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreatePdfFinancialYearlySalesReportAsync(int year)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreatePdfFinancialYearlyStockReportAsync(int year)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreatePdfInvoiceAsync(Invoice invoice)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreatePdfLedgerAsync(Guid ledgerid)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreatePdfMonthlyAccountingReportAsync(int month)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreatePdfMonthlyAttendaceAsync(MonthlyAttendance monthlyAttendace)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreatePdfMonthlyPurchaseReportAsync(int month)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreatePdfMonthlySalesReportAsync(int month)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreatePdfMonthlyStockReportAsync(int month)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreatePdfPartyLedgerAsync(Guid partyId)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreatePdfPayslipAsync(Guid employeeId, SalaryPaySlip paySlip)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreatePdfPurchaseReportAsync(DateTime period)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreatePdfQuarterlyAccountingReportAsync(int Quarter)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreatePdfQuarterlyCustomerDueReportAsync(int Quarter)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreatePdfQuarterlyPurchaseReportAsync(int Quarter)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreatePdfQuarterlySalesReportAsync(int Quarter)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreatePdfQuarterlyStockReportAsync(int Quarter)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreatePdfSalaryReportAsync(Guid employeeId)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreatePdfSalesReportAsync(DateTime period)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreatePdfStockReportAsync(DateTime period)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreatePdfVouchersAsync(string voucherNumber)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreatePdfYearlyAccountingReportAsync(int year)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreatePdfYearlyCustomerDueReportAsync(int year)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreatePdfYearlyPurchaseReportAsync(int year)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreatePdfYearlySalesReportAsync(int year)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreatePdfYearlyStockReportAsync(int year)
        {
            throw new NotImplementedException();
        }
    }
}