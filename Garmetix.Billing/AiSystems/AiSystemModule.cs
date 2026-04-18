using System.Diagnostics;

namespace AISystems;
//// MauiProgram.cs
builder.Services.AddTransient<BankTransactionEntryViewModel>();
builder.Services.AddTransient<BankTransactionEntryPage>();

// AppShell.xaml.cs
Routing.RegisterRoute("BankTransactionEntryPage", typeof(Views.BankTransactionEntryPage));
public static class AiSystemModule 
{
    public static void InitDatabase()
    {
        await _database.CreateTableAsync<LedgerGroup>();
await _database.CreateTableAsync<Ledger>();
await _database.CreateTableAsync<Party>();
await _database.CreateTableAsync<Bank>();
await _database.CreateTableAsync<BankAccount>();
await _database.CreateTableAsync<Voucher>();
await _database.CreateTableAsync<CashVoucher>();
    }
    public static void Initialize() 
    {
        // Initialize your systems here
        Routing.RegisterRoute("VoucherEntryPage", typeof(Views.VoucherEntryPage));
Routing.RegisterRoute("PartyFormPage", typeof(Views.PartyFormPage));
Routing.RegisterRoute("BankFormPage", typeof(Views.BankFormPage)); // Create this to map to your Bank models
    }

    public static void RegisterSystems() 
    {
        // Register your systems here
        // Registries (Listings)
builder.Services.AddTransient<VoucherRegistryViewModel>();
builder.Services.AddTransient<VoucherRegistryPage>();
builder.Services.AddTransient<PartyRegistryViewModel>(); // Create similar to VoucherRegistry
builder.Services.AddTransient<PartyRegistryPage>();
builder.Services.AddTransient<BankRegistryViewModel>(); // Create similar to VoucherRegistry
builder.Services.AddTransient<BankRegistryPage>();

// Entry Forms
builder.Services.AddTransient<VoucherEntryViewModel>();
builder.Services.AddTransient<VoucherEntryPage>();
builder.Services.AddTransient<PartyFormViewModel>();
builder.Services.AddTransient<PartyFormPage>();
// Create a BankFormViewModel & Page similar to Party
    }
}