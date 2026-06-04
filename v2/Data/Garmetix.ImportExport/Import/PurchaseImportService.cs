using ClosedXML.Excel;
using Garmetix.ImportExport.Models;
using System.Text.Json;

namespace Garmetix.ImportExports.Services
{
    public class PurchaseImportService
    {
        public string GeneratePurchaseImportJson(string excelFilePath, string outputJsonPath)
        {
            if (!File.Exists(excelFilePath))
                throw new FileNotFoundException($"Excel file not found at {excelFilePath}");

            var rawRows = new List<ExcelPurchaseRow>();

            using (var workbook = new XLWorkbook(excelFilePath))
            {
                var sheet = workbook.Worksheets.First(); // Assuming data is on the first sheet
                var rows = sheet.RangeUsed().RowsUsed().Skip(1); // Skip header row

                foreach (var row in rows)
                {
                    // Ensure the row isn't completely empty before processing
                    if (string.IsNullOrWhiteSpace(row.Cell(3).GetString())) continue;

                    var importRow = new ExcelPurchaseRow
                    {
                        SN = row.Cell(1).GetString(),
                        InwardDate = row.Cell(2).GetString(),
                        InwardNumber = row.Cell(3).GetString(),
                        Brand = row.Cell(4).GetString(),
                        Category = row.Cell(5).GetString(),
                        Barcode = row.Cell(6).GetString(),
                        ItemName = row.Cell(7).GetString(),
                        Description = row.Cell(8).GetString(),
                        StyleCode = row.Cell(9).GetString(),
                        ArtNo = row.Cell(10).GetString(),
                        ShadeColor = row.Cell(11).GetString(),
                        Sizes = row.Cell(12).GetString(),
                        Rate = ParseDecimal(row.Cell(13).GetString()),
                        Qty = ParseDecimal(row.Cell(14).GetString()),
                        Discount = ParseDecimal(row.Cell(15).GetString()),
                        Unit = row.Cell(16).GetString(),
                        MRP = ParseDecimal(row.Cell(17).GetString()),
                        BasicRate = ParseDecimal(row.Cell(18).GetString()),
                        BasicAmount = ParseDecimal(row.Cell(19).GetString()),
                        InputTaxRate = ParseDecimal(row.Cell(20).GetString()),
                        InputTaxAmount = ParseDecimal(row.Cell(21).GetString()),
                        CostValue = ParseDecimal(row.Cell(22).GetString()),
                        AdditionalCostRate = ParseDecimal(row.Cell(23).GetString()),
                        AdditionalCostAmount = ParseDecimal(row.Cell(24).GetString()),
                        ActualCost = ParseDecimal(row.Cell(25).GetString()),
                        FreightCost = ParseDecimal(row.Cell(26).GetString()),
                        AcquisitionCost = ParseDecimal(row.Cell(27).GetString()),
                        CostWithAllInclusive = ParseDecimal(row.Cell(28).GetString()),
                        ProjectedMRP = ParseDecimal(row.Cell(29).GetString()),
                        MRPOutputTaxRate = ParseDecimal(row.Cell(30).GetString()),
                        OutputTaxAmount = ParseDecimal(row.Cell(31).GetString()),
                        GSTPurchase = ParseDecimal(row.Cell(32).GetString()),
                        HSNCode = row.Cell(33).GetString(),
                        ItemCode = row.Cell(34).GetString(),
                        BrandCode = row.Cell(35).GetString(),
                        CategoryCode = row.Cell(36).GetString(),
                        SoldPrice = ParseDecimal(row.Cell(37).GetString()),
                        SoldQty = ParseDecimal(row.Cell(38).GetString()),
                        SoldDate = row.Cell(39).GetString(),
                        CurrentStock = ParseDecimal(row.Cell(40).GetString()),
                        CurrentCostValue = ParseDecimal(row.Cell(41).GetString()),
                        ProfitLoss = ParseDecimal(row.Cell(42).GetString()),
                        ProfitLossPercentage = ParseDecimal(row.Cell(43).GetString()),
                        Supplier = row.Cell(44).GetString(),
                        InvoiceDate = row.Cell(45).GetString(),
                        InvoiceNumber = row.Cell(46).GetString(),
                        Status = row.Cell(47).GetString()
                    };
                    rawRows.Add(importRow);
                }
            }

            // Export the structured raw data to JSON as a backup/intermediate step
            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(rawRows, jsonOptions);
            File.WriteAllText(outputJsonPath, jsonString);

            return outputJsonPath;
        }

        private decimal ParseDecimal(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return 0;
            value = value.Replace(",", "").Trim();

            // Handle weird boolean conversions in Excel (sometimes TRUE/FALSE shows up in numeric cols)
            if (value.Equals("TRUE", StringComparison.OrdinalIgnoreCase)) return 1;
            if (value.Equals("FALSE", StringComparison.OrdinalIgnoreCase)) return 0;

            if (decimal.TryParse(value, out decimal result)) return result;
            return 0;
        }
    }
}