/*
 * Garmetix
 * Author: Amit Kumar
 * https://garmetix.com/
 * Copyright (c) 2026. All rights reserved.
 * Version: 6.0.0
 * License: https://garmetix.com/license
 * Website: https://garmetix.com/
*/

/*
 * Garmetix - Accounting Module
 * 
 */

using Garmetix.Accounting.PageModels.Parties;
using Garmetix.Accounting.PageModels.Vouchers;

namespace Garmetix.Accounting
{
    public static class AccountingModule
    {
        public static void EnableAccountingRoutes()
        {
            RouterHelper.AddRoute(typeof(EntryVoucherPage));
            RouterHelper.AddRoute(typeof(EntryCashVoucherPage));
            RouterHelper.AddRoute(typeof(EntryLedgerPage));
            RouterHelper.AddRoute(typeof(EntryLedgerGroupPage));
            RouterHelper.AddRoute(typeof(EntryPartyPage));
            RouterHelper.AddRoute(typeof(EntryDueRecoveryPage));
            RouterHelper.AddRoute(typeof(EntryCustomerDuePage));
            RouterHelper.AddRoute(typeof(EntryTransactionPage));

            RouterHelper.AddRoute(typeof(EntryBankPage));
            RouterHelper.AddRoute(typeof(EntryBankAccountDetailPage));
            RouterHelper.AddRoute(typeof(EntryBankAccountPage));
            RouterHelper.AddRoute(typeof(EntryVendorBankAccountPage));
            RouterHelper.AddRoute(typeof(EntryBankAccountListPage));

            RouterHelper.AddRoute(typeof(EntryBankTransactionPage));
            RouterHelper.AddRoute(typeof(EntryChequeLogPage));

         

        }

        public static MauiAppBuilder UseBanking(this MauiAppBuilder builder)
        {
            //Mobile Page
            builder.Services.AddSingleton<Pages.Mobile.BankPage>();
            builder.Services.AddSingleton<Pages.Mobile.BankAccountPage>();
            builder.Services.AddSingleton<Pages.Mobile.BankAccountListPage>();
            builder.Services.AddSingleton<Pages.Mobile.VendorBankAccountPage>();


            builder.Services.AddSingleton<BankPageModel>();
            builder.Services.AddSingleton<BankAccountPageModel>();
            builder.Services.AddSingleton<BankAccountDetailPageModel>();
            builder.Services.AddSingleton<VendorBankAccountPageModel>();
            builder.Services.AddSingleton<BankAccountListPageModel>();

            builder.Services.AddSingleton<Pages.BankPage>();
            builder.Services.AddSingleton<BankAccountDetailPage>();

            builder.Services.AddSingleton<Pages.BankAccountPage>();
            builder.Services.AddSingleton<Pages.BankAccountListPage>();
            builder.Services.AddSingleton<Pages.VendorBankAccountPage>();

            // Entry Pages and Form Models
            builder.Services.AddTransient<EntryBankPage>();
            builder.Services.AddTransient<EntryBankAccountPage>();
            builder.Services.AddTransient<EntryBankAccountListPage>();
            builder.Services.AddTransient<EntryBankAccountDetailPage>();
            builder.Services.AddTransient<EntryVendorBankAccountPage>();
            builder.Services.AddTransient<EntryBankTransactionPage>();
            builder.Services.AddTransient<EntryChequeLogPage>();

            builder.Services.AddTransient<BankFormModel>();
            builder.Services.AddTransient<BankAccountFormModel>();
            builder.Services.AddTransient<BankAccountDetailFormModel>();
            builder.Services.AddTransient<VendorBankAccountFormModel>();
            builder.Services.AddTransient<BankAccountListFormModel>();
            builder.Services.AddTransient<BankTransactionFormModel>();
            builder.Services.AddTransient<ChequeLogFormModel>();

            //Bank Transcations ..
            //TODO:no Mobile page Enabled, Create mobile and enable it
            builder.Services.AddSingleton<BankTransactionPageModel>();
            builder.Services.AddSingleton<ChequeLogPageModel>();
            builder.Services.AddSingleton<BankTransactionPage>();
            builder.Services.AddSingleton<ChequeLogPage>();

            return builder;
        }

        public static MauiAppBuilder UseAccounting(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<VoucherPageModel>();
            builder.Services.AddSingleton<VoucherPage>();


            builder.Services.AddSingleton<CashVoucherPageModel>();
            builder.Services.AddSingleton<CashVoucherPage>();


            builder.Services.AddSingleton<PartyPageModel>();
            builder.Services.AddSingleton<Pages.PartyPage>();
            builder.Services.AddSingleton<Pages.Mobile.PartyPage>();

            builder.Services.AddSingleton<LedgerPageModel>();
            builder.Services.AddSingleton<Pages.LedgerPage>();
            builder.Services.AddSingleton<Pages.Mobile.LedgerPage>();

            builder.Services.AddSingleton<LedgerGroupPageModel>();
            builder.Services.AddSingleton<Pages.Mobile.LedgerGroupPage>();
            builder.Services.AddSingleton<Pages.LedgerGroupPage>();

            builder.Services.AddSingleton<Pages.Mobile.TransactionPage>();
            builder.Services.AddSingleton<Pages.TransactionPage>();
            builder.Services.AddSingleton<TransactionPageModel>();

            builder.Services.AddSingleton<CustomerDuePageModel>();
            builder.Services.AddSingleton<Pages.CustomerDuePage>();
            builder.Services.AddSingleton<Pages.Mobile.CustomerDuePage>();

            builder.Services.AddSingleton<DueRecoveryPageModel>();
            builder.Services.AddSingleton<Pages.DueRecoveryPage>();
            builder.Services.AddSingleton<Pages.Mobile.DueRecoveryPage>();

            //Entry Pages and Form Models

            builder.Services.AddTransient<EntryVoucherPage>();
            builder.Services.AddTransient<EntryCashVoucherPage>();
            builder.Services.AddTransient<EntryLedgerPage>();
            builder.Services.AddTransient<EntryLedgerGroupPage>();
            builder.Services.AddTransient<EntryPartyPage>();
            builder.Services.AddTransient<EntryDueRecoveryPage>();
            builder.Services.AddTransient<EntryCustomerDuePage>();
            builder.Services.AddTransient<EntryTransactionPage>();

            builder.Services.AddTransient<VoucherFormModel>();
            builder.Services.AddTransient<CashVoucherFormModel>();
            builder.Services.AddTransient<LedgerFormModel>();
            builder.Services.AddTransient<LedgerGroupFormModel>();
            builder.Services.AddTransient<PartyFormModel>();
            builder.Services.AddTransient<DueRecoveryFormModel>();
            builder.Services.AddTransient<CustomerDueFormModel>();
            builder.Services.AddTransient<TransactionFormModel>();



            return builder;
        }
    }
}