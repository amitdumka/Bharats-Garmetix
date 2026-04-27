/*
 * Garmetix
 * Author: Amit Kumar
 * https://garmetix.com/
 * Copyright (c) 2026. All rights reserved.
 * Version: 6.0.0
 * License: https://garmetix.com/license
 * Website: https://garmetix.com/
*/


// Base of all Enums used in Garmetix. This file is included in all platforms. 
namespace Garmetix.Core.Enums
{
    // All the code in this file is included in all platforms.
    public enum AppOperation { Company, StoreGroup, Store, All, None }
    public enum Permission { R, W, M, D, RW, RWM, RWMD, N, S }
    public enum LoginRole { Admin, StoreManager, Salesman, Accountant, RemoteAccountant, Member, PowerUser };

    public enum UserType { Admin, Owner, StoreManager, Sales, Accountant, CA, Guest, PowerUser, Employees }
    public enum Gender { Male, Female, TransGender }

    public enum PaymentMode
    {
        Cash, Card, UPI, Wallets, IMPS, RTGS, NEFT, Cheque, DemandDraft, CreditNote,
        DebitNote, Coupons, MixPayments, SaleReturn, Others,
    }

    public enum AccountType { Saving, Current, CashCredit, OverDraft, Others, Loan, CF, }

    public enum TransactionType { Deposit, Withdraw }
    public enum TransactionMode { Cash, Cheque, NEFT, RTGS, UPI, NetBanking, IMPS, DD, ATM, Swipe, Other }

    public enum VoucherType
    {
        Payment,
        Receipt,
        Expense,
    }

    public enum LedgerType
    {
        Assest,
        Cash,
        BankAccount,
        Loan,
        Expenses,
        DirectExpenses,
        IndirectExpenses,
        Income,
        DirectIncome,
        InDirectExpenses,
        Purcahase,
        Sale,
        StockItem,
        Employee,
        CaptialAccount,
    }

    public enum LedgerCategory
    {
        Credit,
        Debit,
        Income,
        Expenses,
        Assets,
        Bank,
        Loan,
        Purchase,
        Sale,
        Vendor,
        Customer,
        UnCategory,
        Employees,
        Stock,
        Debitor,
        Creditor
    }

    public enum PartyType
    {
        Customer,
        Supplier,
        Employee,
        Vendor,
        Debitor,
        Creditor,
        Others,
    }
    public enum ExpenseType
    {
        Travel,
        Transport,
        Ticket,
        Food,
        Lodging,
        Entertainment,
        Medical,
        OtherExpenses,
        Fuel,
        Miscellanous,
        Others
    }
    public enum TripType
    {
        Purchasing, Travel, Sales, Marketing, Service, Others, Project, Training, Conference, Meeting, Research, Development, Personal
    }
    public enum Status
    {
        Pending, Ongoing, Running,
        Approved, Success, Error,
        Failed, InProgress, Started, Ended,
        Processing, Waiting,
        Rejected, Completed, Cancelled, PartiallyApproved, PartiallyRejected,
        PartiallyCompleted, Unknown
    }

    // Add more enums as needed for the application.
     
    
    public enum CardType //TODO: this is not complete, need to add more card types and abstract this to a separate class if needed in future
    {
        Debit,
        Credit,
        Prepaid,
        Other
    }
    public enum SalaryComponent
    {
        NetSalary,
        LastPcs,
        WOWBill,
        SundaySalary,
        Incentive,
        Others,
        Advance,
        PaidLeave,
        SickLeave,
        SalaryAdvance,
        Receipts,
    }

    public enum EmployeeCategory
    {
        Salesman,
        StoreManager,
        HouseKeeping,
        Owner,
        Accounts,
        TailorMaster,
        Tailors,
        TailoringAssistance,
        Others,
    }

    public enum AttendanceStatus
    {
        Present,
        Absent,
        HalfDay,
        Sunday,
        Holiday,
        StoreClosed,
        SundayHoliday,
        SickLeave,
        PaidLeave,
        CasualLeave,
        OnLeave,
        Leave,
        WorkFromHome
    }

}
