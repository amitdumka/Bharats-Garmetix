namespace Garmetix.Enums
{
    // All the code in this file is included in all platforms.
    public enum AppOperation
    { Company, StoreGroup, Store, All, None }
    public enum Permission
    { R, W, M, D, RW, RWM, RWMD, N, S }
    public enum LoginRole
    { Admin, StoreManager, Salesman, Accountant, RemoteAccountant, Member, PowerUser };

    public enum UserType
    { Admin, Owner, StoreManager, Sales, Accountant, CA, Guest, PowerUser, Employees }
    public enum Gender
    {
        Male,
        Female,
        TransGender
    }

    public enum PaymentMode
    {
        Cash,
        Card,
        UPI,
        Wallets,
        IMPS,

        RTGS,
        NEFT,
        Cheque,
        DemandDraft,

        CreditNote,
        DebitNote,

        Coupons,

        MixPayments,
        SaleReturn,

        Others,
    }
}
