namespace Garmetix.PdfServices.Services.NotImplemented;

public interface IPdfBankingService
{
    Task<string> CreatePdfBankStatementAsync(DateTime periodStart, DateTime periodEnd);
    Task<string> CreatePdfBankAccountFlowAsync(DateTime periodStart, DateTime periodEnd);
    Task<string> CreatePdfBankBalanceSheetAsync(DateTime periodStart, DateTime periodEnd);
    Task<string> CreatePdfBankTranscationsAsync(DateTime periodStart, DateTime periodEnd);
}
public class PdfBankingService : IPdfBankingService
{
    public Task<string> CreatePdfBankAccountFlowAsync(DateTime periodStart, DateTime periodEnd)
    {
        throw new NotImplementedException();
    }

    public Task<string> CreatePdfBankBalanceSheetAsync(DateTime periodStart, DateTime periodEnd)
    {
        throw new NotImplementedException();
    }

    public Task<string> CreatePdfBankStatementAsync(DateTime periodStart, DateTime periodEnd)
    {
        throw new NotImplementedException();
    }

    public Task<string> CreatePdfBankTranscationsAsync(DateTime periodStart, DateTime periodEnd)
    {
        throw new NotImplementedException();
    }
}