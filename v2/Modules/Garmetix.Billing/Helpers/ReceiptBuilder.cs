using Garmetix.Core.Models.Inventory;
using System.Text;
namespace Garmetix.Billing.Helpers
{
    /// <summary>
    /// Receipt Builder is thermal invoice 
    /// </summary>
    public static class ReceiptBuilder
    {
        // ====================================================================================
        // 1. THERMAL PRINTER FORMAT (ESC/POS RAW BYTES)
        // ====================================================================================
        public static byte[] GenerateThermalReceiptBytes(Invoice invoice, IEnumerable<InvoiceItem> items)
        {
            List<byte> bytes = new List<byte>();

            // ESC/POS Commands
            byte[] initPrinter = new byte[] { 27, 64 };
            byte[] alignCenter = new byte[] { 27, 97, 1 };
            byte[] alignLeft = new byte[] { 27, 97, 0 };
            byte[] alignRight = new byte[] { 27, 97, 2 };
            byte[] boldOn = new byte[] { 27, 69, 1 };
            byte[] boldOff = new byte[] { 27, 69, 0 };
            byte[] cutPaper = new byte[] { 29, 86, 66, 0 };

            bytes.AddRange(initPrinter);

            // --- HEADER ---
            bytes.AddRange(alignCenter);
            bytes.AddRange(boldOn);
            bytes.AddRange(Encoding.ASCII.GetBytes(StoreInfo.StoreName + "\n"));
            bytes.AddRange(boldOff);
            bytes.AddRange(Encoding.ASCII.GetBytes(StoreInfo.StoreAddress.Replace("\n", " ") + "\n"));
            bytes.AddRange(Encoding.ASCII.GetBytes(StoreInfo.ContactInfo + "\n"));
            bytes.AddRange(Encoding.ASCII.GetBytes("GSTIN: " + StoreInfo.GSTIN + "\n"));
            bytes.AddRange(Encoding.ASCII.GetBytes("--------------------------------\n")); // 32 Chars wide (58mm standard)
            bytes.AddRange(boldOn);
            bytes.AddRange(Encoding.ASCII.GetBytes("TAX INVOICE\n"));
            bytes.AddRange(boldOff);
            bytes.AddRange(Encoding.ASCII.GetBytes("--------------------------------\n"));

            // --- CUSTOMER DETAILS ---
            bytes.AddRange(alignLeft);
            bytes.AddRange(Encoding.ASCII.GetBytes($"Inv No: {invoice.InvoiceNumber}\n"));
            bytes.AddRange(Encoding.ASCII.GetBytes($"Date  : {invoice.OnDate:dd-MMM-yyyy HH:mm}\n"));
            bytes.AddRange(Encoding.ASCII.GetBytes($"Name  : {invoice.CustomerName}\n"));
            bytes.AddRange(Encoding.ASCII.GetBytes($"Mobile: {invoice.CustomerMobileNumber}\n"));
            if (!string.IsNullOrEmpty(invoice.CustomerGSTIN))
                bytes.AddRange(Encoding.ASCII.GetBytes($"GSTIN : {invoice.CustomerGSTIN}\n"));
            bytes.AddRange(Encoding.ASCII.GetBytes("--------------------------------\n"));

            // --- ITEMS TABLE ---
            bytes.AddRange(boldOn);
            bytes.AddRange(Encoding.ASCII.GetBytes("Item - Barcode       \n  Qty  Rate   Total \n"));
            bytes.AddRange(boldOff);
            bytes.AddRange(Encoding.ASCII.GetBytes("--------------------------------\n"));

            foreach (var item in items)
            {
                string name = item.Product?.Name.Length > 12 ? item.Product?.Name.Substring(0, 12) : item.Product?.Name.PadRight(12);
                string barcode = item.Barcode.PadRight(12);
                //string qty = item.Quantity.ToString().PadLeft(3);
                // Expanded PadLeft to 4 to accommodate the decimal point
                string qty = item.BilledQuantity.ToString("0.##").PadLeft(4);
                string rate = item.BasePrice.ToString("0").PadLeft(6);
                string total = item.LineTotal.ToString("0").PadLeft(7);
                bytes.AddRange(Encoding.ASCII.GetBytes($"{name}{barcode}\n {qty} {rate} {total}\n"));
            }
            bytes.AddRange(Encoding.ASCII.GetBytes("--------------------------------\n"));

            // --- TOTALS & GST LOGIC ---
            bytes.AddRange(alignLeft);
            bytes.AddRange(Encoding.ASCII.GetBytes($"Sub Total        : Rs. {invoice.NetAmount,8:F2}\n"));

            decimal totalDiscount = invoice.DiscountAmount + invoice.BillDiscountAmount;
            if (totalDiscount > 0)
                bytes.AddRange(Encoding.ASCII.GetBytes($"Discount         :-Rs. {totalDiscount,8:F2}\n"));

            // Indian GST Split Logic
            if (invoice.InterState)
            {
                bytes.AddRange(Encoding.ASCII.GetBytes($"IGST             : Rs. {invoice.TaxAmount,8:F2}\n"));
            }
            else
            {
                decimal halfTax = invoice.TaxAmount / 2m;
                bytes.AddRange(Encoding.ASCII.GetBytes($"CGST             : Rs. {halfTax,8:F2}\n"));
                bytes.AddRange(Encoding.ASCII.GetBytes($"SGST             : Rs. {halfTax,8:F2}\n"));
            }

            if (invoice.RoundOff != 0)
                bytes.AddRange(Encoding.ASCII.GetBytes($"Round Off        : Rs. {invoice.RoundOff,8:F2}\n"));

            bytes.AddRange(Encoding.ASCII.GetBytes("--------------------------------\n"));
            bytes.AddRange(boldOn);
            bytes.AddRange(Encoding.ASCII.GetBytes($"GRAND TOTAL      : Rs. {invoice.BillAmount,8:F2}\n"));
            bytes.AddRange(boldOff);
            bytes.AddRange(Encoding.ASCII.GetBytes("--------------------------------\n"));

            // --- FOOTER ---
            bytes.AddRange(alignCenter);
            bytes.AddRange(Encoding.ASCII.GetBytes("Thank you for shopping with us!\n"));
            bytes.AddRange(Encoding.ASCII.GetBytes("Visit Again\n\n\n\n"));
            bytes.AddRange(cutPaper);

            return bytes.ToArray();
        }

        // ====================================================================================
        // 2. A5 / WHATSAPP FORMAT (BEAUTIFUL HTML GENERATION)
        // ====================================================================================
        public static string GenerateA5HtmlInvoice(Invoice invoice, IEnumerable<InvoiceItem> items)
        {
            decimal totalDiscount = invoice.DiscountAmount + invoice.BillDiscountAmount;

            string gstRows = "";
            if (invoice.InterState)
            {
                gstRows = $@"
                    <tr><td colspan='5' class='text-right'><b>IGST</b></td><td class='text-right'>₹ {invoice.TaxAmount:F2}</td></tr>";
            }
            else
            {
                decimal halfTax = invoice.TaxAmount / 2m;
                gstRows = $@"
                    <tr><td colspan='5' class='text-right'><b>CGST</b></td><td class='text-right'>₹ {halfTax:F2}</td></tr>
                    <tr><td colspan='5' class='text-right'><b>SGST</b></td><td class='text-right'>₹ {halfTax:F2}</td></tr>";
            }

            string discountRow = totalDiscount > 0 ? $"<tr><td colspan='5' class='text-right'><b>Total Discount</b></td><td class='text-right text-orange'>-₹ {totalDiscount:F2}</td></tr>" : "";
            string roundOffRow = invoice.RoundOff != 0 ? $"<tr><td colspan='5' class='text-right'><b>Round Off</b></td><td class='text-right'>₹ {invoice.RoundOff:F2}</td></tr>" : "";
            string customerGstinRow = !string.IsNullOrEmpty(invoice.CustomerGSTIN) ? $"<br><b>GSTIN:</b> {invoice.CustomerGSTIN}" : "";

            StringBuilder itemRows = new StringBuilder();
            int sNo = 1;
            foreach (var item in items)
            {
                itemRows.Append($@"
                    <tr>
                        <td class='text-center'>{sNo++}</td>
                        <td>{item.Product?.Name}</td>
                        <td class='text-center'>{item.BilledQuantity}</td>
                        <td class='text-right'>{item.BasePrice:F2}</td>
                        <td class='text-center'>{item.TaxPercentage}%</td>
                        <td class='text-right'>{item.LineTotal:F2}</td>
                    </tr>");
            }

            string html = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='utf-8'>
                <style>
                    body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; color: #333; margin: 0; padding: 20px; font-size: 14px; }}
                    .invoice-box {{ max-width: 148mm; margin: auto; padding: 20px; border: 1px solid #ddd; box-shadow: 0 0 10px rgba(0, 0, 0, 0.15); background: #fff; }}
                    .header {{ text-align: center; border-bottom: 2px solid #005A9C; padding-bottom: 10px; margin-bottom: 20px; }}
                    .header h1 {{ margin: 0; color: #005A9C; font-size: 24px; text-transform: uppercase; letter-spacing: 2px; }}
                    .header p {{ margin: 3px 0; font-size: 12px; color: #555; }}
                    .title {{ text-align: center; font-weight: bold; font-size: 16px; margin-bottom: 15px; letter-spacing: 1px; }}
                    .details-table {{ width: 100%; margin-bottom: 20px; font-size: 13px; }}
                    .details-table td {{ vertical-align: top; }}
                    .items-table {{ width: 100%; border-collapse: collapse; margin-bottom: 20px; font-size: 13px; }}
                    .items-table th, .items-table td {{ padding: 8px; border: 1px solid #ddd; }}
                    .items-table th {{ background: #005A9C; color: white; text-align: left; font-weight: bold; }}
                    .text-center {{ text-align: center !important; }}
                    .text-right {{ text-align: right !important; }}
                    .text-orange {{ color: #E67E22; }}
                    .totals-section td {{ padding: 6px 8px; border-bottom: 1px solid #eee; }}
                    .grand-total {{ font-size: 18px; font-weight: bold; color: #005A9C; background-color: #F4F6F9; }}
                    .footer {{ text-align: center; margin-top: 30px; font-size: 12px; color: #777; border-top: 1px solid #ddd; padding-top: 10px; }}
                </style>
            </head>
            <body>
                <div class='invoice-box'>
                    <div class='header'>
                        <h1>{StoreInfo.StoreName}</h1>
                        <p>{StoreInfo.StoreAddress.Replace("\n", ", ")}</p>
                        <p>{StoreInfo.ContactInfo} | GSTIN: {StoreInfo.GSTIN}</p>
                    </div>

                    <div class='title'>TAX INVOICE</div>

                    <table class='details-table'>
                        <tr>
                            <td style='width: 50%;'>
                                <b>Bill To:</b><br>
                                {invoice.CustomerName}<br>
                                Mobile: {invoice.CustomerMobileNumber}
                                {customerGstinRow}
                            </td>
                            <td style='width: 50%; text-align: right;'>
                                <b>Invoice No:</b> {invoice.InvoiceNumber}<br>
                                <b>Date:</b> {invoice.OnDate:dd-MMM-yyyy hh:mm tt}<br>
                                <b>State Code:</b> {StoreInfo.StateCode}
                            </td>
                        </tr>
                    </table>

                    <table class='items-table'>
                        <thead>
                            <tr>
                                <th class='text-center'>#</th>
                                <th>Item Description</th>
                                <th class='text-center'>Qty</th>
                                <th class='text-right'>Rate (₹)</th>
                                <th class='text-center'>GST %</th>
                                <th class='text-right'>Amount (₹)</th>
                            </tr>
                        </thead>
                        <tbody class='totals-section'>
                            {itemRows}
                            <tr>
                                <td colspan='5' class='text-right' style='border-top: 2px solid #005A9C;'><b>Sub Total</b></td>
                                <td class='text-right' style='border-top: 2px solid #005A9C;'>₹ {invoice.NetAmount:F2}</td>
                            </tr>
                            {discountRow}
                            {gstRows}
                            {roundOffRow}
                            <tr class='grand-total'>
                                <td colspan='5' class='text-right'>GRAND TOTAL</td>
                                <td class='text-right'>₹ {invoice.BillAmount:F2}</td>
                            </tr>
                        </tbody>
                    </table>

                    <div class='footer'>
                        <b>Thank you for your business!</b><br>
                        Computer Generated Invoice. No Signature Required.
                    </div>
                </div>
            </body>
            </html>";

            return html;
        }
    }

}
