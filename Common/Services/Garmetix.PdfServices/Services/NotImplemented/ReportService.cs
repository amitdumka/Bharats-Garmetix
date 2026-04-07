using Garmetix.Models.HRM;
using Garmetix.Models.Inventory;

namespace Garmetix.PdfServices.Services.NotImplemented
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