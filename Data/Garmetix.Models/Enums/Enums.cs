/*
 * Author: Amit Kumar
 * Date: 01-06-2023
 * Project: Garmetix
 * File: Enums.cs
 * Description: This file contains all the enums used in the application.
  * Garmetix
 * Author: Amit Kumar
 * https://garmetix.com/
 * Copyright (c) 2025. All rights reserved.
 * Version: 5.0.0
 * License: https://garmetix.com/license
 * Website: https://garmetix.com/ 
 * */

namespace Garmetix.Models.Enums
{
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
    public enum CardType //TODO: this is not complete, need to add more card types and abstract this to a separate class if needed in future
    {
        Debit,
        Credit,
        Prepaid,
        Other
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