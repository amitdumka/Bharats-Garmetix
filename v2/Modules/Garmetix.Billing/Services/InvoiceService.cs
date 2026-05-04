using DocumentFormat.OpenXml.Wordprocessing;
using Garmetix.Core.Models.Inventory;
using Garmetix.Databases;
using Garmetix.Databases.Services;
using Garmetix.Models.Reports;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Garmetix.Billing.Services
{

    /// <summary>
    /// Billing Service is base of Invoice and Purchase Service.
    /// it will handle this as bases for all service
    /// </summary>
    public class BillingService
    {
        protected DatabaseContext _localDb => DatabaseService.Instance.LocalDB;
        public DatabaseContext GetContext() { return _localDb; }
        private BillingService _instance;
        public BillingService Instance => _instance ?? new BillingService();

        public BillingService()
        {
            _instance = this;

        }


        /// <summary>
        ///  Create Stock Item when Purchase or indiredt operation
        /// </summary>
        /// <param name="stock"></param>
        /// <returns></returns>

        public Stock AddStock(Stock stock) { }
        /// <summary>
        /// Remove or Delete Stock 
        /// </summary>
        /// <param name="stock"></param>
        /// <param name="permarnent"></param>
        /// <returns></returns>
        public bool RemoveStock(Stock stock, bool permarnent=false) { return false; }
       /// <summary>
       /// Remove or delete stock
       /// </summary>
       /// <param name="StoreId"></param>
       /// <param name="Barcode"></param>
       /// <param name="permarnent"></param>
       /// <returns></returns>
        public bool RemoveStock(Guid StoreId, string Barcode, bool permarnent = false) { return false; }

        /// <summary>
        /// Update the stock while purchase or Sale
        /// </summary>
        /// <param name="storeid"></param>
        /// <param name="barcode"></param>
        /// <param name="qty"></param>
        /// <param name="sold"></param>
        /// <returns></returns>
        public async Task<bool> UpdateStockAsync(Guid storeid, string barcode, decimal qty, decimal price, bool sold = false)
        {
            try
            {
                

                var result = await GetContext().Stocks.Where(x => x.StoreId == storeid && x.Barcode == barcode).FirstOrDefaultAsync();

                if (result == null) return false;

                if (sold)
                {
                    // Update the price value of sold amount
                    result.SoldQty += qty;

                }
                else

                {
                    //TODO: Update the average Price  of Cost price. 

                    result.PurchaseQty += qty;
                }
                GetContext().Stocks.Update(result);
                return (await GetContext().SaveChangesAsync() > 0);


            }
            catch (Exception ex)
            {
                //Notify the error 
                return false;
            }
        }

        /// <summary>
        /// invoice Service is used to handle Invoice/Billing Services 
        /// It will be extend Billing Service as Base Service. 
        /// </summary>
        public class InvoiceService
        {
            private InvoiceService _instance;
            public InvoiceService Instance => _instance ?? new InvoiceService();

            public InvoiceService()
            {
                _instance = this;
            }



        }

        public class PurchaseService
        {
            private PurchaseService _instance;
            public PurchaseService Instance => _instance ?? new PurchaseService();
            public PurchaseService()
            {
                _instance = this;

            }

        }
    }
}