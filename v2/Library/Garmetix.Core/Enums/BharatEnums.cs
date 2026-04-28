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
 * This file contains all the Enums used in Garmetix. Enums are used to define a set of named constants that can be used throughout the application. 
 * This file is included in all platforms, so any changes made here will be reflected in all platforms. 
 * Enums are used to improve code readability and maintainability by providing meaningful names for constant values. 
 * Enums can also be used to enforce type safety and prevent invalid values from being assigned to variables. 
 * Enums can be used in switch statements, if statements, and other control flow structures to make the code more readable and easier to understand. 
 * Enums can also be used in data models, view models, and other classes to represent specific states or categories of data. 
 * Enums can be defined with specific underlying types (e.g., int, byte) to optimize memory usage and performance. 
 * Enums can also have associated methods and properties to provide additional functionality. 
 * Overall, enums are a powerful tool for improving code quality and maintainability in Garmetix.
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
    public enum CARD
    {
        DebitCard,
        CreditCard,
        AmexCard,
        GiftCard,
        Other
    }

    public enum CARDType
    {
        Visa,
        MasterCard,
        Maestro,
        AmexCard,
        Dinners,
        Rupay,
        RupayCredit,
        Others,
    }
    public enum Unit
    {
        Meters,
        Nos,
        Pcs,
        Packets,
        Grams,
        Kgs,
        Liter,
        NoUnit,
        Than, Boxes
    }

    public enum TaxType
    {
        GST,
        SGST,
        CGST,
        IGST,
        VAT,
        CST,
    }

    public enum NotesType
    {
        DebitNote,
        CreditNote,
    }

    public enum InvoiceType
    {
        Sales,
        SalesReturn,
        ManualSale,
        ManualSaleReturn,
    }

    public enum PurchaseInvoiceType
    {
        Purchase,
        PurchaseReturn,
    }
    /// <summary>
    /// ProductType represents a type of a product
    /// </summary>
    public enum ProductType
    {
        Apparels,
        Clothing,
        Electronics,
        Fabric,
        Accessories,
        InnerWear,
        SuitCovers,
        FootWear,
        Readmade,
        Jewellery,
        Cosmetics,
        WinterWear,
        Others
    }
    public enum CompanyType
    {
        Proprietorship, // Proprietorship
        Partnership,
        PrivateLimited,
        PublicLimited,
        LLP, // Limited Liability Partnership
        Others
    }
    public enum StoreCategory
    {
        Cloths,
        Garments,
        Readymade,
        Furniture,
        FuelStation,
        General,
        Retail,
        Wholesale,
        Distributor,
        Others
    }

    //Check  

    public enum Order
    { Asc, Desc }

    public enum UserAccess
    {
        Admin,
        SuperAdmin,
        SuperUser,
        PowerUser,
        User,
        Guest,
    }

    public enum Unit
    {
        Meters,
        Nos,
        Pcs,
        Packets,
        Grams,
        Kgs,
        Liter,
        NoUnit,
        Than, Boxes
    }

    public enum UOM
    {
        Meters,
        Nos,
        Pcs,
        Packets,
        Grams,
        Kgs,
        Liter,
        NoUnit,
        Than,
        Boxes
    }

    public enum AttUnit
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

    public enum TaxType
    {
        GST,
        SGST,
        CGST,
        IGST,
        VAT,
        CST,
    }

    public enum NotesType
    {
        DebitNote,
        CreditNote,
    }

    public enum InvoiceType
    {
        Sales,
        SalesReturn,
        ManualSale,
        ManualSaleReturn,
    }

    public enum PurchaseInvoiceType
    {
        Purchase,
        PurchaseReturn,
    }

    public enum EntryStatus
    {
        Added,
        Approved,
        Rejected,
        Updated,
        Deleted,
        DeleteApproved,
    }

    public enum LedgerEntryType
    {
        Expenses,
        Payment,
        Receipt,
        Salary,
        AdvancePayment,
        AdvanceReceipt,
        ArvindLimited,
        Others,
    }

    public enum NoteType
    {
        DebitNote,
        CreditNote,
    }

    public enum PayMode
    {
        Cash,
        Card,
        RTGS,
        NEFT,
        IMPS,
        Wallets,
        Cheque,
        DemandDraft,
        Others,
        Coupons,
        MixPayments,
        UPI,
        SaleReturn,
    }

    public enum Size2
    {
        S,
        M,
        L,
        XL,
        XXL,
        XXXL,
        T28,
        T3,
        T32,
        T34,
        T36,
        T38,
        T4,
        T41,
        T42,
        T44,
        T46,
        T48,
        FreeSize,
        NS,
        NOTVALID,
        B36,
        B38,
        B4,
        B42,
        B44,
        B46,
        B96,
    }

    public enum Size
    {
        S,
        M,
        L,
        XL,
        XXL,
        XXXL,
        C28,
        C3,
        C32,
        C34,
        C36,
        C38,
        C4,
        C41,
        C42,
        C44,
        C46,
        C48,
        C96,
        FreeSize,
        NS,
        NOTVALID,
        C39,
        C92,
    }

    public enum ProductCategory
    {
        Fabric,
        Apparel,
        Accessories,
        Tailoring,
        Trims,
        PromoItems,
        Coupons,
        GiftVouchers,
        Others,
        SuitCovers,
        InnerWear,
    }

    public enum CARD
    {
        DebitCard,
        CreditCard,
        AmexCard,
        GiftCard,
        Other
    }

    public enum CARDType
    {
        Visa,
        MasterCard,
        Maestro,
        AmexCard,
        Dinners,
        Rupay,
        RupayCredit,
        Others,
    }

    public enum VendorType
    {
        EBO,
        MBO,
        Tailoring,
        NonSalable,
        OtherSaleable,
        Others,
        TempVendor,
        InHouse,
        Distributor,
        Brands,
        BrandAuth,
    }

    public enum DebitCredit
    {
        In,
        Out,
    }

    public enum RolePermission
    {
        Owner,
        GeneralManager,
        GroupManager,
        Accountant,
        CA,
        StoreManager,
        Salesmen,
        Guest,
        Other,
    }


    //Check end

}
