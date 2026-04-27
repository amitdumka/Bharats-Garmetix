/*
 * Garmetix
 * Author: Amit Kumar
 * https://garmetix.com/
 * Copyright (c) 2026. All rights reserved.
 * Version: 6.0.0
 * License: https://garmetix.com/license
 * Website: https://garmetix.com/
*/


using Garmetix.Core.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Garmetix.Models.DayOperations;

public class PettyCashSheet : BaseEntity
{
    [ForeignKey("Store")]
    [Display(Name = "Store", AutoGenerateField = false)] public Guid StoreId { get; set; }
    [Display(Name = "Date")] public DateTime OnDate { get; set; }
    [Display(Name = "Opening Balance")] public decimal OpeningBalance { get; set; } = 0;
    [Display(Name = "Sales")] public decimal Sales { get; set; } = 0;
    [Display(Name = "Receipts")] public decimal Receipts { get; set; } = 0;
    [Display(Name = "Due Receipts")] public decimal DueReceipts { get; set; } = 0;
    [Display(Name = "Bank Withdrawal")] public decimal BankWithdrawal { get; set; } = 0;
    [Display(Name = "Expenses")] public decimal Expenses { get; set; } = 0;
    [Display(Name = "Payments")] public decimal Payments { get; set; } = 0;
    [Display(Name = "Customer Due")] public decimal CustomerDue { get; set; } = 0;
    [Display(Name = "Bank Deposit")] public decimal BankDeposit { get; set; } = 0;
    [Display(Name = "Non Cash Sale")] public decimal NonCashSale { get; set; } = 0;
    [Display(Name = "Cash In Hand")] public decimal CashInHand { get; set; } = 0;


    [Display(Name = "Created By", AutoGenerateField = false)] public string? CreatedBy { get; set; } = "AutoAdmin";

}

public class DayBegin : BaseEntity
{
    [ForeignKey("Store")]
    [Display(Name = "Store", AutoGenerateField = false)] public Guid StoreId { get; set; }
    [Display(Name = "Date")] public DateTime OnDate { get; set; }
    [Display(Name = "Opening Balance")] public decimal OpeningBalance { get; set; }
    [Display(Name = "Cash Detail", AutoGenerateField = false)] public Guid CashDetailId { get; set; }
    [Display(Name = "Created By", AutoGenerateField = false)] public string? CreatedBy { get; set; } = "AutoAdmin";
}
public class DayEnd : BaseEntity
{
    [ForeignKey("Store")]
    [Display(Name = "Store", AutoGenerateField = false)] public Guid StoreId { get; set; }
    [Display(Name = "Date")] public DateTime OnDate { get; set; }
    [Display(Name = "Closing Balance")] public decimal ClosingBalance { get; set; }
    [Display(Name = "Cash Detail", AutoGenerateField = false)] public Guid CashDetailId { get; set; }
    [Display(Name = "Created By", AutoGenerateField = false)] public string? CreatedBy { get; set; } = "AutoAdmin";
}


public class CashDetail : BaseEntity
{
    [ForeignKey("Store")]
    [Display(Name = "Store", AutoGenerateField = false)] public Guid StoreId { get; set; }
    [Display(Name = "Date")] public DateTime OnDate { get; set; }
    [Display(Name = "Amount")] public decimal Amount { get; set; }
    [Display(Name = "N2000")] public int N2000 { get; set; } = 0;
    [Display(Name = "N500")] public int N500 { get; set; } = 0;
    [Display(Name = "N200")] public int N200 { get; set; } = 0;
    [Display(Name = "N100")] public int N100 { get; set; } = 0;
    [Display(Name = "N50")] public int N50 { get; set; } = 0;
    [Display(Name = "N20")] public int NC20 { get; set; } = 0;
    [Display(Name = "N10")] public int NC10 { get; set; } = 0;
    [Display(Name = "N5")] public int NC5 { get; set; } = 0;
    [Display(Name = "N2")] public int NC2 { get; set; } = 0;
    [Display(Name = "N1")] public int NC1 { get; set; } = 0;

    [Display(Name = "Created By", AutoGenerateField = false)] public string? CreatedBy { get; set; } = "AutoAdmin";



}
