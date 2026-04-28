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
 * FormModel.cs
 * 
 * This is a base class for all the
 * Forms in the application, it contains the common properties and methods for all the forms, it is an abstract class and should be inherited by all the forms in the application
 */


using Bharat.ToolKits.Extensions;
using Bharat.ToolKits.Helpers;
using Bharat.ToolKits.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Garmetix.Core.DataModels;
using Garmetix.Core.Enums;
using Garmetix.Core.Interfaces;
using Garmetix.Core.Services;
using Garmetix.Core.VM;
using Syncfusion.Maui.DataForm;

namespace Garmetix.Base.PageModels
{
    /// <summary>
    /// This is a base class for all the forms in the application, it contains the common properties and methods for all the forms, it is an abstract class and should be inherited by all the forms in the application
    ///
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract partial class FormModel<TEntity> : PageModelBase, IDataFormSourceProvider where TEntity : class, IEntity
    {
        #region Properties

        /// <summary>
        /// Enable Printing, especially for Invoice and Voucher etc
        /// </summary>
        [ObservableProperty]
        protected bool _enablePrinting = false;
        [ObservableProperty]
        public bool _canShare = false;
        [ObservableProperty]
        private TEntity? _entity = null!; // Initialize with a non-null default value

        [ObservableProperty]
        protected bool _isNew = true;

        [ObservableProperty]
        protected string _title = typeof(TEntity).Name;

        [ObservableProperty]
        protected SfDataForm? _entryForm = null;

        #endregion Properties

        #region Methods

        [RelayCommand]
        protected void ValidateForm(SfDataForm dataform)
        {
            if (EntryForm != null)
            {
                var flag = EntryForm.Validate();
                if (!flag)
                {
                    //Alter usr and log
                }
            }
        }

        /// <summary>
        /// This method is used to handle post save of entity
        /// </summary>
        /// <param name="result">Saved Entity</param>
         
        /// <returns></returns>
        protected bool Save(bool result)
        {
            IsBusy = false;
            var title = typeof(TEntity).Name.Replace("Entry", "").SplitPascalCase_Simple();
            
            //var result = await DataModel.SaveAsync(record, IsNew);
            if (result)
            {
                _ = ServiceHelper.Current.GetService<TimelineDataService>()?.MakeEntryAsync(Entity);
                
                //Reset the View of Entry Form
                ResetView();
                Notify.ShowSuccess(title, $"{title} Saved Successfully", snabackbar: true, speak: true,   isLong: true);
            }
            else
            {

                Notify.ShowError(title, $"Failed to save {title}, Kindly check and try again!", snabackbar: true, speak: true, isLong: true);
            }
            return result;
        }

        /// <summary>
        /// This method is used to navigate to previous page
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        protected async Task Back()
        {
            await Shell.Current.GoToAsync($"..");
        }

        /// <summary>
        /// This Command is used for cancle the form and reset the view
        /// </summary>
        [RelayCommand]
        protected void Cancle()
        {
            if (IsNew)
            {
                InitFormViewModel();
            }
        }

        /// <summary>
        /// This method is used to reset the view
        /// </summary>
        protected void ResetView()
        {
            IsNew = true;
            InitFormViewModel();
        }

        #endregion Methods

        #region events

        [RelayCommand]
        protected void EditEvent(object index)
        {
            //object item  as M;
            //TODO: Implement your edit logic here
            if (index == null)
            {
                _ = Notify.DisplayNotificationAsync("Select the row then swipe...");
                return;
            }
            if (index != null)
            {
                // Navigate to edit page or open edit dialog
                _ = Notify.DisplayNotificationAsync("Edit action triggered");
            }
        }

        [RelayCommand]
        protected void DeleteEvent(object index)
        {
            if (index == null)
            {
                _ = Notify.DisplayNotificationAsync("Select the row then swipe...");
                return;
            }
            //TODO: Implement your delete logic here

            if (index != null)
            {
                //Entities.Remove(deleteEntity );
                // RecordCount = Entities.Count;
                _ = Notify.DisplayNotificationAsync("Delete action triggered");
            }
        }

        /// <summary>
        /// Get Source based on Source Name like Company , Store etc for Combo Box in Forms
        /// </summary>
        /// <param name="sourceName"></param>
        /// <returns></returns>
        public object GetSource(string sourceName)
        {
            try
            {
                if (sourceName == "Store")
                {
                    return CommonDataModel.GetStoreList(_db);
                }
                if (sourceName == "Tax")
                {
                    return CommonDataModel.GetTaxList(_db);
                }
                if (sourceName == "Company")
                {
                    return CommonDataModel.GetCompanyList(_db);
                }
                if (sourceName == "StoreGroup" || sourceName == "Group")
                {
                    return CommonDataModel.GetGroupList(_db);
                }
                if (sourceName == "LedgerGroup")
                {
                    return CommonDataModel.GetLedgerGroupList(_db);
                }
                if (sourceName == "Ledger")
                {
                    return CommonDataModel.GetLedgerList(_db);
                }
                if (sourceName == "Transaction")
                {
                    return CommonDataModel.GetTransactionList(_db);
                }
                if (sourceName == "Employee")
                {
                    return CommonDataModel.GetEmployeeList(_db);
                }
                if (sourceName == "BankAccount" || sourceName == "Account" || sourceName == "AccountNumber")
                {
                    return CommonDataModel.GetBankAccountList(_db);
                }
                if (sourceName == "Bank")
                {
                    return CommonDataModel.GetBankList(_db);
                }
                if (sourceName == "Salesman" || sourceName == "Salesmen")
                {
                    return CommonDataModel.GetSalesmanList(_db);
                }
                if (sourceName == "InvoiceNumber" || sourceName == "InvoiceNumbers")
                {
                    return CommonDataModel.GetInvoiceNumberList(_db);
                }
                if (sourceName == "Vendor" || sourceName == "Vendors")
                {
                    return CommonDataModel.GetVendorList(_db);
                }
                if (sourceName == "Party" || sourceName == "Parties")
                {
                    return CommonDataModel.GetVendorList(_db);
                }
                if (sourceName == "ProductSubCategory" || sourceName == "ProductSubCategories")
                {
                    return CommonDataModel.GetProductSubCategoryList(_db);
                }
                if (sourceName == "ProductCategory" || sourceName == "ProductCategories")
                {
                    return CommonDataModel.GetProductCategoryList(_db);
                }
                if (sourceName == "DueInvoiceNumber" || sourceName == "DueInvoiceNumbers")
                {
                    return CommonDataModel.GetDueInvoiceNumberList(_db);
                }
                if (sourceName == "Product" || sourceName == "Products")
                {
                    return CommonDataModel.GetProductList(_db);
                }
                if (sourceName == "PurchaseInvoice" || sourceName == "Purchase Invoice" || sourceName == "PurchaseInvoices")
                {
                    return CommonDataModel.GetPurchaseInvoiceNumberList(_db);
                }
                return new List<ComboBoxItemVM>();
            }
            catch (Exception ex)
            {
                Notify.ShowError("Source Error", ex.Message, snabackbar: true);
                SentrySdk.CaptureException(ex);
                return new List<ComboBoxItemVM>();
            }
        }

        /// <summary>
        /// This method is used to generate the form items and Control it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GeneratedFormItems(object sender, GenerateDataFormItemEventArgs e)
        {
            if (e.DataFormItem != null)
            {
                if (e.DataFormItem.FieldName == "Id" || e.DataFormItem.FieldName.EndsWith("Id"))
                {
                    e.DataFormItem.IsVisible = false;
                }
                if (e.DataFormItem.FieldName == "EndDate")
                {
                    e.DataFormItem.IsVisible = false;
                }

                if (AppOperations == AppOperation.Store)
                {
                    if (e.DataFormItem.FieldName == "CompanyId" || e.DataFormItem.FieldName == "StoreGroupId" || e.DataFormItem.FieldName == "StoreId" || e.DataFormItem.FieldName == "Company" || e.DataFormItem.FieldName == "StoreGroup" || e.DataFormItem.FieldName == "Store")
                    {
                        e.DataFormItem.IsVisible = false;
                    }
                }
                if (AppOperations == AppOperation.StoreGroup)
                {
                    if ((e.DataFormItem.FieldName == "StoreGroupId" || e.DataFormItem.FieldName == "StoreId" || e.DataFormItem.FieldName == "Company" || e.DataFormItem.FieldName == "StoreGroup" || e.DataFormItem.FieldName == "Store") && e.DataFormItem is DataFormComboBoxItem cbi)
                    {
                        cbi.SelectedValuePath = "Id";
                        cbi.DisplayMemberPath = "Name";
                    }
                    if (e.DataFormItem.FieldName == "CompanyId")
                    {
                        e.DataFormItem.IsVisible = false;
                    }
                }
                if (AppOperations == AppOperation.Company)
                {
                    if ((e.DataFormItem.FieldName == "CompanyId" || e.DataFormItem.FieldName == "StoreGroupId" || e.DataFormItem.FieldName == "StoreId" || e.DataFormItem.FieldName == "Company" || e.DataFormItem.FieldName == "StoreGroup" || e.DataFormItem.FieldName == "Store") && e.DataFormItem is DataFormComboBoxItem cbi)
                    {
                        cbi.SelectedValuePath = "Id";
                        cbi.DisplayMemberPath = "Name";
                    }
                }

                if ((e.DataFormItem.FieldName == "LedgerId" || e.DataFormItem.FieldName == "LedgerGroupId" || e.DataFormItem.FieldName == "Ledger" || e.DataFormItem.FieldName == "LedgerGroup" || e.DataFormItem.FieldName == "Transaction" || e.DataFormItem.FieldName == "TransactionId") && e.DataFormItem is DataFormComboBoxItem cbi2)
                {
                    cbi2.SelectedValuePath = "Id";
                    cbi2.DisplayMemberPath = "Name";
                }
                if ((e.DataFormItem.FieldName == "InvoiceNumber" || e.DataFormItem.FieldName == "Employee" || e.DataFormItem.FieldName == "EmployeId") && e.DataFormItem is DataFormComboBoxItem cbi3)
                {
                    cbi3.SelectedValuePath = "Id";
                    cbi3.DisplayMemberPath = "Name";
                }

                if ((e.DataFormItem.FieldName == "Tax" || e.DataFormItem.FieldName == "Account" || e.DataFormItem.FieldName == "BankAccount" || e.DataFormItem.FieldName == "AccountNumber" || e.DataFormItem.FieldName == "Bank") && e.DataFormItem is DataFormComboBoxItem cbi4)
                {
                    cbi4.SelectedValuePath = "Id";
                    cbi4.DisplayMemberPath = "Name";
                }
                if ((e.DataFormItem.FieldName == "PurchaseInvoice" || e.DataFormItem.FieldName == "Product" || e.DataFormItem.FieldName == "ProductSubCategory" || e.DataFormItem.FieldName == "ProductCategory" || e.DataFormItem.FieldName == "Vendor" || e.DataFormItem.FieldName == "Party") && e.DataFormItem is DataFormComboBoxItem cbi5)
                {
                    cbi5.SelectedValuePath = "Id";
                    cbi5.DisplayMemberPath = "Name";
                }
                if ((e.DataFormItem.FieldName == "DueInvoiceNumber" || e.DataFormItem.FieldName == "InvoiceNumber") && e.DataFormItem is DataFormComboBoxItem cbi6)
                {
                    cbi6.SelectedValuePath = "Name";
                    cbi6.DisplayMemberPath = "Name";
                }
            }
        }

        #endregion events

        #region AbstracrMethods

        /// <summary>
        /// This method is used to save the data to database, it is an abstract method
        /// </summary>
        [RelayCommand]
        protected abstract void SaveButton();

        /// <summary>
        /// This is used to Initialize the Form View Model
        /// </summary>
        public abstract void InitFormViewModel();

        /// <summary>
        /// OnGenerateDataFormItem is used to add the data form items
        /// </summary>
        /// <param name="sender">Refernce of object where the method is called</param>
        /// <param name="e">Event Arguments</param>
        public abstract void OnGenerateDataFormItem(object sender, GenerateDataFormItemEventArgs e);
        [RelayCommand]
        protected virtual async Task MailIt()
        {
            await Notify.ShowError("It is not implemented!", false);
        }

        [RelayCommand]
        protected virtual async Task ShareIt()
        {
            await Notify.ShowError("It is not implemented!", false);

        }


        #endregion AbstractMethods
    }
}