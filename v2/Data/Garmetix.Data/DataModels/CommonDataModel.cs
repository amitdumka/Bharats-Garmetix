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
 * CommonDataModel.cs
 *
 *CommonDataModel class provides static methods to retrieve lists of various entities (like Employees, Stores, Companies, etc.) from the database and return them as lists of ComboBoxItemVM for use in UI components.
 * Dependency: This class depends on the DatabaseContext to access the database and retrieve the necessary data. It also uses the ComboBoxItemVM class from the Garmetix.Core.VM namespace to represent items in combo boxes.
 * 
 * This file is part of Garmetix.
 * 
 * Garmetix is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * Garmetix is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with Garmetix.  If not, see <http://www.gnu.org/licenses/>.
 */

using Garmetix.Core.VM;
using Garmetix.Databases;

namespace Garmetix.Core.DataModels
{

    /// <summary>
    /// 
    /// </summary>
    public class CommonDataModel
    {
        public static List<ComboBoxItemVM> GetEmployeeList(DatabaseContext db, Guid CompanyId) => db.Employees.Where(c => c.CompanyId == CompanyId).Select(c => new ComboBoxItemVM { Id = c.Id, Name = c.FullName }).ToList();

        public static List<ComboBoxItemVM> GetEmployeeList(DatabaseContext db) => db.Employees.Select(c => new ComboBoxItemVM { Id = c.Id, Name = c.FullName }).ToList();

        public static List<ComboBoxItemVM> GetStoreList(DatabaseContext db) => db.Stores.Select(c => new ComboBoxItemVM { Id = c.Id, Name = c.Name }).ToList();

        public static List<ComboBoxItemVM> GetCompanyList(DatabaseContext db) => db.Companies.Select(c => new ComboBoxItemVM { Id = c.Id, Name = c.Name }).ToList();

        public static List<ComboBoxItemVM> GetGroupList(DatabaseContext db) => db.StoreGroups.Select(c => new ComboBoxItemVM { Id = c.Id, Name = c.Name }).ToList();

        public static List<ComboBoxItemVM> GetLedgerGroupList(DatabaseContext db) => db.LedgerGroups.Select(c => new ComboBoxItemVM { Id = c.Id, Name = c.Name }).ToList();

        public static List<ComboBoxItemVM> GetLedgerList(DatabaseContext db) => db.Ledgers.Select(c => new ComboBoxItemVM { Id = c.Id, Name = c.Name }).ToList();

        public static List<ComboBoxItemVM> GetTransactionList(DatabaseContext db) => db.Transactions.Select(c => new ComboBoxItemVM { Id = c.Id, Name = c.Name }).ToList();

        public static List<ComboBoxItemVM> GetSalesmanList(DatabaseContext db, Guid StoreId) => db.Salesmen.Where(c => c.StoreId == StoreId && !c.Deleted).Select(c => new ComboBoxItemVM { Id = c.Id, Name = c.Name }).ToList();

        public static List<ComboBoxItemVM> GetSalesmanList(DatabaseContext db) => db.Salesmen.Select(c => new ComboBoxItemVM { Id = c.Id, Name = c.Name }).ToList();

        public static List<ComboBoxItemVM> GetBankAccountList(DatabaseContext db, Guid CompanyId) => db.BankAccounts.Where(c => c.CompanyId == CompanyId).Select(c => new ComboBoxItemVM { Id = c.Id, Name = c.AccountHolderName + " (" + c.AccountNumber + ")" }).ToList();

        public static List<ComboBoxItemVM> GetBankAccountList(DatabaseContext db) => db.BankAccounts.Select(c => new ComboBoxItemVM { Id = c.Id, Name = c.AccountHolderName + " (" + c.AccountNumber + ")" }).ToList();

        public static List<ComboBoxItemVM> GetBankList(DatabaseContext db) => db.Banks.Select(c => new ComboBoxItemVM { Id = c.Id, Name = c.Name }).ToList();

        public static List<ComboBoxItemVM> GetVendorList(DatabaseContext db) => db.Vendors.Select(c => new ComboBoxItemVM { Id = c.Id, Name = c.Name }).ToList();

        public static List<ComboBoxItemVM> GetPartiesList(DatabaseContext db) => db.Parties.Select(c => new ComboBoxItemVM { Id = c.Id, Name = c.Name }).ToList();

        public static List<ComboBoxItemVM> GetInvoiceNumberList(DatabaseContext db) => db.Invoices.Select(c => new ComboBoxItemVM { Id = c.Id, Name = c.InvoiceNumber }).ToList();

        public static List<ComboBoxItemVM> GetPurchaseInvoiceNumberList(DatabaseContext db) => db.PurchaseInvoices.Select(c => new ComboBoxItemVM { Id = c.Id, Name = c.InvoiceNumber }).ToList();

        public static List<ComboBoxItemVM> GetDueInvoiceNumberList(DatabaseContext db) => db.CustomerDues.Where(c => !c.Paid).Select(c => new ComboBoxItemVM { Id = c.Id, Name = c.InvoiceNumber }).ToList();

        public static List<ComboBoxItemVM> GetProductList(DatabaseContext db) => db.Products.Select(c => new ComboBoxItemVM { Id = c.Id, Name = c.Name }).ToList();

        public static List<ComboBoxItemVM> GetProductCategoryList(DatabaseContext db) => db.ProductCategories.Select(c => new ComboBoxItemVM { Id = c.Id, Name = c.Name }).ToList();

        public static List<ComboBoxItemVM> GetProductSubCategoryList(DatabaseContext db) => db.ProductSubCategories.Select(c => new ComboBoxItemVM { Id = c.Id, Name = c.Name }).ToList();

        public static List<ComboBoxItemVM> GetTaxList(DatabaseContext db) => db.Taxes.Select(c => new ComboBoxItemVM { Id = c.Id, Name = c.Name }).ToList();
    }
}