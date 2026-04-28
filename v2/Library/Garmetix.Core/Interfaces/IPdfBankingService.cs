namespace Garmetix.Core.Interfaces
{
    public interface IPdfBankingService
    {
        Task<string> CreatePdfBankStatementAsync(DateTime periodStart, DateTime periodEnd);
        Task<string> CreatePdfBankAccountFlowAsync(DateTime periodStart, DateTime periodEnd);
        Task<string> CreatePdfBankBalanceSheetAsync(DateTime periodStart, DateTime periodEnd);
        Task<string> CreatePdfBankTranscationsAsync(DateTime periodStart, DateTime periodEnd);
    }
}
