using Garmetix.AI.Billing.ViewModels;
using Garmetix.AI.Billing.Views;
using System.Diagnostics;

namespace Garmetix.AI.AISystems;
 
public static class AiSystemModule
{
    

     
    public static void Initialize()
    {
        Routing.RegisterRoute("BankTransactionEntryPage", typeof(Garmetix.AI.Billing.Views.BankTransactionEntryPage));

        // Initialize your systems here
        Routing.RegisterRoute("VoucherEntryPage", typeof(Garmetix.AI.Billing.Views.VoucherEntryPage));
        Routing.RegisterRoute("PartyFormPage", typeof(Garmetix.AI.Billing.Views.PartyFormPage));
        Routing.RegisterRoute("BankFormPage", typeof(Garmetix.AI.Billing.Views.BankFormPage)); // Create this to map to your Bank models
    }

    public static MauiAppBuilder RegisterSystems(MauiAppBuilder builder)
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

        builder.Services.AddTransient<BankTransactionEntryViewModel>();
        builder.Services.AddTransient<BankTransactionEntryPage>();
        // Create a BankFormViewModel & Page similar to Party

        return builder;
    }
}