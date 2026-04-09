using System.Collections.Generic;
using System.Text;
using  Garmetix.AI.Billing.Models;

namespace  Garmetix.AI.Billing.Helpers
{


    public static class StoreInfo
    {
        public static string StoreName { get; set; }="AADWIKA FASHION";
        public static string StoreAddress { get; set; } = "Bhagalpur Road, near TATA Showroom,\n Dumka,\n Jharkhand";
        public static string ContactInfo { get; set; } = "Contact: 9334799099";
        public static string GSTIN { get; set; } = "GSTIN: 20AJHPA7396P1ZV";
    }
    public static class ReceiptBuilder
    {
        public static byte[] GenerateInvoiceBytes(Invoice invoice, IEnumerable<InvoiceItem> items)
        {
            List<byte> bytes = new List<byte>();

            byte[] initPrinter = new byte[] { 27, 64 };
            byte[] alignCenter = new byte[] { 27, 97, 1 };
            byte[] alignLeft = new byte[] { 27, 97, 0 };
            byte[] boldOn = new byte[] { 27, 69, 1 };
            byte[] boldOff = new byte[] { 27, 69, 0 };
            byte[] cutPaper = new byte[] { 29, 86, 66, 0 };

            bytes.AddRange(initPrinter);
            bytes.AddRange(alignCenter);
            bytes.AddRange(boldOn);
            bytes.AddRange(Encoding.ASCII.GetBytes(StoreInfo.StoreName+"\n"));
            bytes.AddRange(boldOff);
            bytes.AddRange(Encoding.ASCII.GetBytes($"{StoreInfo.StoreAddress}\n"));
            bytes.AddRange(Encoding.ASCII.GetBytes("--------------------------------\n"));
            bytes.AddRange(Encoding.ASCII.GetBytes($"{StoreInfo.ContactInfo}\n"));
            bytes.AddRange(Encoding.ASCII.GetBytes($"{StoreInfo.GSTIN}\n"));
            bytes.AddRange(Encoding.ASCII.GetBytes("--------------------------------\n"));


            bytes.AddRange(alignLeft);
            bytes.AddRange(Encoding.ASCII.GetBytes($"Invoice No: {invoice.InvoiceNo}\n"));
            bytes.AddRange(Encoding.ASCII.GetBytes($"Date: {invoice.Date:dd-MMM-yyyy HH:mm}\n"));
            bytes.AddRange(Encoding.ASCII.GetBytes($"Customer: {invoice.CustomerName}\n"));
            bytes.AddRange(Encoding.ASCII.GetBytes($"Mobile: {invoice.MobileNo}\n"));
            bytes.AddRange(Encoding.ASCII.GetBytes("--------------------------------\n"));

            bytes.AddRange(Encoding.ASCII.GetBytes("Item          Qty  Rate   Total \n"));
            bytes.AddRange(Encoding.ASCII.GetBytes("--------------------------------\n"));

            foreach (var item in items)
            {
                string name = item.ProductName.Length > 12 ? item.ProductName.Substring(0, 12) : item.ProductName.PadRight(12);
                string qty = item.Quantity.ToString().PadLeft(3);
                string rate = item.Rate.ToString("0").PadLeft(6);
                string total = item.TotalAmount.ToString("0").PadLeft(7);
                bytes.AddRange(Encoding.ASCII.GetBytes($"{name} {qty} {rate} {total}\n"));
            }

            bytes.AddRange(Encoding.ASCII.GetBytes("--------------------------------\n"));
            bytes.AddRange(alignCenter);
            bytes.AddRange(Encoding.ASCII.GetBytes($"Sub Total: Rs. {invoice.SubTotal:0.00}\n"));
            bytes.AddRange(Encoding.ASCII.GetBytes($"Total GST: Rs. {invoice.TotalTax:0.00}\n"));
            bytes.AddRange(boldOn);
            bytes.AddRange(Encoding.ASCII.GetBytes($"GRAND TOTAL: Rs. {invoice.GrandTotal:0.00}\n"));
            bytes.AddRange(boldOff);
            
            bytes.AddRange(Encoding.ASCII.GetBytes("\nThank you for shopping with us!\n\n\n\n"));
            bytes.AddRange(cutPaper);

            return bytes.ToArray();
        }
    }
}