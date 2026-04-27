using ClosedXML.Excel;

using Garmetix.Databases;

using Garmetix.Databases.Services;
using Garmetix.Models.Bases;
using Syncfusion.XlsIO;
using Syncfusion.XlsIO.Implementation;
using System.Data;
using System.Reflection;
using System.Text.Json;

using System.Text.Json.Serialization;
using IRange = Syncfusion.XlsIO.IRange;
using JsonSerializer = System.Text.Json.JsonSerializer; // Adjust namespace as needed

namespace Garmetix.DataServices.ImportExports
{
    public static class DataImportExport
    {
        private static DatabaseContext _db => DatabaseService.Instance.LocalDB;

        //TODO: Implement for just passing class name it will export the class data Model
        /// <summary>
        /// Imports data from a JSON file, updates the database, and reports changes.
        /// This method assumes the entities in JSON have an 'Id' property.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="filePath">The full path to the JSON file.</param>
        /// <returns>A report string detailing the changes.</returns>
        public static async Task<string> ImportAndUpdateDatabaseFromJson<T>(string filePath, string sheetName = "Sheet1") where T : class
        {
            var report = new System.Text.StringBuilder();
            var changesMade = false;

            try
            {// Configure the serializer options.
                var options = new JsonSerializerOptions
                {
                    // Make the JSON output human-readable.
                    WriteIndented = true,

                    // --- KEY CHANGE ---
                    // Handle cyclic references by ignoring objects that have already been serialized.
                    // This requires .NET 6 or later, which is standard for .NET MAUI.
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                };
                string jsonString = await File.ReadAllTextAsync(filePath);
                var importedEntities = JsonSerializer.Deserialize<List<T>>(jsonString, options);

                if (importedEntities == null || !importedEntities.Any())
                {
                    report.AppendLine("No data found in the JSON file or file is empty.");
                    return report.ToString();
                }

                var dbSet = _db.Set<T>();
                var idProperty = typeof(T).GetProperty("Id"); // Assumes 'Id' as primary key

                if (idProperty == null)
                {
                    report.AppendLine("Error: Entity type must have an 'Id' property for comparison.");
                    return report.ToString();
                }

                foreach (var importedEntity in importedEntities)
                {
                    var importedId = (Guid)idProperty.GetValue(importedEntity)!; // Cast to int, adjust if ID type is different
                    var existingEntity = await dbSet.FindAsync(importedId);

                    if (existingEntity == null)
                    {
                        // New entity: Add to database
                        await dbSet.AddAsync(importedEntity);
                        report.AppendLine($"Added new entity with ID: {importedId}");
                        changesMade = true;
                    }
                    else
                    {
                        //// Existing entity: Check for changes and update
                        bool entityChanged = false;
                        foreach (var prop in typeof(T).GetProperties())
                        {
                            if (prop.CanRead && prop.CanWrite && prop.Name != "Id") // Don't compare ID
                            {
                                var importedValue = prop.GetValue(importedEntity);
                                var existingValue = prop.GetValue(existingEntity);

                                if (!object.Equals(importedValue, existingValue))
                                {
                                    prop.SetValue(existingEntity, importedValue);
                                    report.AppendLine($"Updated ID {importedId}: Property '{prop.Name}' changed from '{existingValue}' to '{importedValue}'.");
                                    entityChanged = true;
                                }
                            }
                        }

                        if (entityChanged)
                        {
                            dbSet.Update(existingEntity);
                            changesMade = true;
                        }
                    }
                }

                if (changesMade)
                {
                    await _db.SaveChangesAsync();
                    report.AppendLine("Database updated successfully from JSON.");
                }
                else
                {
                    report.AppendLine("No changes detected in the JSON file compared to the database.");
                }
            }
            catch (FileNotFoundException)
            {
                report.AppendLine($"Error: File not found at '{filePath}'.");
            }
            catch (JsonException ex)
            {
                report.AppendLine($"Error parsing JSON file: {ex.Message}");
            }
            catch (Exception ex)
            {
                report.AppendLine($"An error occurred during JSON import: {ex.Message}");
            }

            return report.ToString();
        }

        /// <summary>
        /// Imports data from an Excel file, updates the database, and reports changes.
        /// This method assumes the first column is the primary key (Id).
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="filePath">The full path to the Excel file.</param>
        /// <returns>A report string detailing the changes.</returns>
        public static async Task<string> ImportAndUpdateDatabaseFromExcel<T>(string filePath, string sheetName = "Sheet1") where T : class

        {
            var report = new System.Text.StringBuilder();
            var changesMade = false;

            try
            {
                using (var workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheet(1); // Get the first worksheet

                    var headerRow = worksheet.Row(1);
                    var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                              .Where(p => p.CanWrite && p.CanRead) // Ensure property can be written and read
                                              .ToList();

                    // Create a dictionary to map column names to property infos
                    var columnPropertyMap = new Dictionary<int, PropertyInfo>();
                    // Add a null check for LastCellUsed()
                    var lastCellInHeader = headerRow.LastCellUsed();
                    if (lastCellInHeader != null)
                    {
                        // Changed this line to use lastCellInHeader.Column.ColumnNumber()
                        for (int i = 1; i <= lastCellInHeader.WorksheetColumn().ColumnNumber(); i++)
                        {
                            var columnName = headerRow.Cell(i).Value.ToString();
                            var property = properties.FirstOrDefault(p => p.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase));
                            if (property != null)
                            {
                                columnPropertyMap[i] = property;
                            }
                        }
                    }
                    else
                    {
                        report.AppendLine("Error: The Excel file's first sheet has no header row or is empty.");
                        return report.ToString();
                    }

                    // Assume the first column in Excel is the Id for comparison
                    var idProperty = properties.FirstOrDefault(p => p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));
                    if (idProperty == null)
                    {
                        report.AppendLine("Error: Entity type must have an 'Id' property for comparison.");
                        return report.ToString();
                    }

                    foreach (var row in worksheet.RowsUsed().Skip(1)) // Skip header row
                    {
                        if (row.Cell(1).IsEmpty()) continue; // Skip empty rows

                        // Get the ID from the first column of the Excel row
                        int excelId;
                        if (!int.TryParse(row.Cell(1).Value.ToString(), out excelId))
                        {
                            report.AppendLine($"Skipping row {row.RowNumber()}: Invalid ID format.");
                            continue;
                        }

                        // Find the existing entity in the database
                        var existingEntity = await _db.Set<T>().FindAsync(excelId);

                        if (existingEntity == null)
                        {
                            // New entity: Create and add to database
                            var newEntity = (T)new Object();
                            bool isNewEntityValid = true;
                            foreach (var kvp in columnPropertyMap)
                            {
                                var cellValue = row.Cell(kvp.Key).Value.ToString();
                                try
                                {
                                    var convertedValue = Convert.ChangeType(cellValue, kvp.Value.PropertyType);
                                    kvp.Value.SetValue(newEntity, convertedValue);
                                }
                                catch (Exception ex)
                                {
                                    report.AppendLine($"Warning: Could not convert value '{cellValue}' for property '{kvp.Value.Name}' in row {row.RowNumber()}. Error: {ex.Message}");
                                    isNewEntityValid = false;
                                    break;
                                }
                            }

                            if (isNewEntityValid)
                            {
                                await _db.Set<T>().AddAsync(newEntity);
                                report.AppendLine($"Added new entity with ID: {excelId}");
                                changesMade = true;
                            }
                        }
                        else
                        {
                            // Existing entity: Check for changes and update
                            bool rowChanged = false;
                            foreach (var kvp in columnPropertyMap)
                            {
                                var property = kvp.Value;
                                var excelValue = row.Cell(kvp.Key).Value.ToString();
                                var dbValue = property.GetValue(existingEntity)?.ToString();

                                // Compare values (case-insensitive for strings, and type-aware for others)
                                object? convertedExcelValue = null;
                                try
                                {
                                    convertedExcelValue = Convert.ChangeType(excelValue, property.PropertyType);
                                }
                                catch
                                {
                                    // Conversion failed, treat as different or log warning
                                    if (excelValue != dbValue)
                                    {
                                        report.AppendLine($"Warning: Value conversion failed for property '{property.Name}' in row {row.RowNumber()}. Treating as change.");
                                        rowChanged = true;
                                    }
                                    continue;
                                }

                                if (!object.Equals(convertedExcelValue, property.GetValue(existingEntity)))
                                {
                                    property.SetValue(existingEntity, convertedExcelValue);
                                    report.AppendLine($"Updated ID {excelId}: Property '{property.Name}' changed from '{dbValue}' to '{excelValue}'.");
                                    rowChanged = true;
                                }
                            }

                            if (rowChanged)
                            {
                                // Mark entity as modified if any property changed
                                _db.Set<T>().Update(existingEntity);
                                changesMade = true;
                            }
                        }
                    }

                    if (changesMade)
                    {
                        await _db.SaveChangesAsync();
                        report.AppendLine("Database updated successfully.");
                    }
                    else
                    {
                        report.AppendLine("No changes detected in the Excel file compared to the database.");
                    }
                }
            }
            catch (FileNotFoundException)
            {
                report.AppendLine($"Error: File not found at '{filePath}'.");
            }
            catch (Exception ex)
            {
                report.AppendLine($"An error occurred during import: {ex.Message}");
                // Consider logging the full exception details in a real application
            }

            return report.ToString();
        }

        public static async Task<int> ImportAndUpdateDatabaseFromExcel<T>(T? data, string filePath, string sheetName = "Sheet1", bool datatable = false) where T : class, IEntity, new()
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"The Excel file was not found at: {filePath}");
            }
            try
            {
                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    Syncfusion.XlsIO.IApplication application = excelEngine.Excel;
                    application.DefaultVersion = ExcelVersion.Xlsx;
                    FileStream inputStream = new FileStream(Path.GetFullPath(@"Data/InputTemplate.xlsx"), FileMode.Open, FileAccess.Read);
                    IWorkbook workbook = application.Workbooks.Open(inputStream);
                    IWorksheet worksheet = workbook.Worksheets[0];

                    // Get the used range of the worksheet
                    IRange usedRange = worksheet.UsedRange;
                    int endRow = usedRange.LastRow;
                    int endCol = usedRange.LastColumn;

                    if (endRow == 0 || endCol == 0)
                    {
                        Console.WriteLine("No data found in the worksheet.");
                        throw new Exception("No data found in the worksheet.");
                    }
                    //Event to choose an action while exporting data from Excel to data table.
                    // worksheet.ExportDataTableEvent += ExportDataTable_EventAction();
                    if (datatable)
                    {
                        //Read data from the worksheet and Export to the DataTable
                        DataTable customersTable = worksheet.ExportDataTable(worksheet.UsedRange, ExcelExportDataTableOptions.ColumnNames | ExcelExportDataTableOptions.ComputedFormulaValues);
                        List<T>? entities = null;
                        if (await CreateOrUpdateDatabase(entities, customersTable))
                        {
                            return customersTable.Rows.Count;
                        }
                    }
                    else
                    {
                        //Export worksheet data into Collection Objects
                        List<T> collectionObjects = worksheet.ExportData<T>(1, 1, endRow, endCol);
                        if (CreateOrUpdateDatabase(collectionObjects))
                        {
                            return collectionObjects.Count;
                        }
                    }

                    //Dispose streams
                    inputStream.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during import: {ex.Message}");
                // Consider logging the full exception details in a real application
            }
            return -1;
        }

        #region MoveTo

        /// <summary>
        /// File Picker for Excel File
        /// </summary>
        /// <returns></returns>
        public static async Task<string> AskForExcelFile()
        {
            try
            {
                // Step 1: Let the user pick an Excel file
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Select Excel File",
                    FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.iOS, new[] { "com.microsoft.excel.xlsx" } }, // UTType for XLSX
                        { DevicePlatform.Android, new[] { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" } }, // MIME type
                        { DevicePlatform.WinUI, new[] { ".xlsx" } },
                        { DevicePlatform.macOS, new[] { "xlsx" } }
                    })
                });

                if (result == null)
                {
                    await Shell.Current.DisplayAlertAsync("Cancelled", "File picking cancelled.", "OK");
                    return "";
                }

                string excelFilePath = result.FullPath;
                await Shell.Current.DisplayAlertAsync("File Selected", $"Selected file: {excelFilePath}", "OK");
                return excelFilePath; ;
                // Step 2: Call the generic function to read and save
                // Make sure your Excel file has headers like "Id", "Name", "Category", "Price", "Stock", "LastUpdated", "IsActive"
                // And corresponding data below them.
                // await ExcelService.ImportAndUpdateDatabaseSF<Product>(excelFilePath, _db);

                // await Shell.Current.DisplayAlertAsync("Success", "Excel data imported successfully!", "OK");
            }
            catch (FileNotFoundException ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", $"File not found: {ex.Message}", "OK"); return "";
            }
            catch (InvalidOperationException ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", $"Operation failed: {ex.Message}", "OK"); return "";
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", $"An unexpected error occurred: {ex.Message}", "OK");
                return "";
            }
        }

        //Template fuction Event Action of ExportDataTable , can be control the data while exporting
        private static void ExportDataTable_EventAction(ExportDataTableEventArgs e)
        {
            if (e.ExcelValue != null && e.ExcelValue.ToString() == "Owner")
                e.ExportDataTableAction = ExportDataTableActions.SkipRow;

            if (e.DataTableColumnIndex == 0 && e.ExcelRowIndex == 5 && e.ExcelColumnIndex == 1)
                e.ExportDataTableAction = ExportDataTableActions.StopExporting;

            if (e.ExcelValue != null && e.ExcelValue.ToString() == "Mexico D.F.")
                e.DataTableValue = "Mexico";

            if (e.ColumnType.ToString() == "Double" && e.ExcelValue != null)
                e.DataTableValue = 30;
        }

        /// <summary>
        /// Create or Update Database Function Move this function to database Service for best performance and reusability
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static bool CreateOrUpdateDatabase<T>(List<T> entities) where T : class, IEntity, new()
        {
            var dbSet = _db.Set<T>();
            foreach (var entity in entities)
            {
                var existingEntity = dbSet.Find(entity.Id);
                if (existingEntity == null)
                {
                    dbSet.Add(entity);
                }
                else
                {
                    dbSet.Update(entity);
                }
            }
            return true;
        }

        public static List<T> ConvertDataTableToList<T>(DataTable dataTable) where T : class, new()
        {
            string JsonData = JsonSerializer.Serialize(dataTable);

            var entities = JsonSerializer.Deserialize<List<T>>(JsonData);
            return entities ?? [];
        }

        /// <summary>
        /// Convert DataTable to List of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static async Task<List<T>> ConvertDataTableToListAsync<T>(DataTable dataTable) where T : class, new()
        {
            // Offload the potentially long-running serialization and deserialization
            // to a background thread to avoid blocking the UI thread in a .NET MAUI app.
#pragma warning disable CS8603 // Possible null reference return.
            return await Task.Run(() =>
            {
                // Serialize the DataTable to a JSON string.
                // This process converts the DataTable's structure and data into a JSON format.
                string jsonData = JsonSerializer.Serialize(dataTable);

                // Deserialize the JSON string back into a List of type T.
                // The JsonSerializer will attempt to map the JSON properties to the properties
                // of the class T. Ensure that the property names in your class T match
                // the column names in your DataTable for successful deserialization.
                var entities = JsonSerializer.Deserialize<List<T>>(jsonData);

                // Return the list of deserialized entities.
                return entities;
            });
#pragma warning restore CS8603 // Possible null reference return.
        }

        /// <summary>
        /// Create or Update Database Function Move this function to database Service for best performance and reusability
        /// Use DataTable as input
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <param name="listdata"></param>
        /// <returns></returns>
        public static async Task<bool> CreateOrUpdateDatabase<T>(List<T>? entities, DataTable listdata) where T : class, IEntity, new()
        {
            //string JsonData = JsonSerializer.Serialize(listdata);

            //entities = JsonSerializer.Deserialize<List<T>>(JsonData);

            entities = await ConvertDataTableToListAsync<T>(listdata);

            var dbSet = _db.Set<T>();
            if (entities == null)
            {
                return false;
            }
            foreach (var entity in entities)
            {
                var existingEntity = dbSet.Find(entity.Id);
                if (existingEntity == null)
                {
                    dbSet.Add(entity);
                }
                else
                {
                    dbSet.Update(entity);
                }
            }
            return true;
        }

        #endregion MoveTo

        /// <summary>
        /// Exports a collection of entities to a JSON file.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="data">The collection of entities to export.</param>
        /// <param name="filePath">The full path where the JSON file will be saved.</param>
        /// <returns>True if export is successful, false otherwise.</returns>
        public static async Task<bool> ExportToJson<T>(this Object entity, IEnumerable<T> data, string filePath) where T : class
        {
            try
            {
                //  var options = new JsonSerializerOptions { WriteIndented = true };
                // Configure the serializer options.
                var options = new JsonSerializerOptions
                {
                    // Make the JSON output human-readable.
                    WriteIndented = true,

                    // --- KEY CHANGE ---
                    // Handle cyclic references by ignoring objects that have already been serialized.
                    // This requires .NET 6 or later, which is standard for .NET MAUI.
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                };

                string jsonString = JsonSerializer.Serialize(data, options);
                await File.WriteAllTextAsync(filePath, jsonString);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting to JSON: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Exports a collection of entities to an Excel file.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="data">The collection of entities to export.</param>
        /// <param name="filePath">The full path where the Excel file will be saved.</param>
        /// <returns>True if export is successful, false otherwise.</returns>
        public static bool ExportToExcel<T>(this Object entity, IEnumerable<T> data, string filePath) where T : class
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Data");

                    // Get properties of the entity to use as column headers
                    var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                              .Where(p => p.CanRead) // Ensure property can be read
                                              .ToList();

                    // Add headers
                    for (int i = 0; i < properties.Count; i++)
                    {
                        worksheet.Cell(1, i + 1).Value = properties[i].Name;
                    }

                    // Add data rows
                    int row = 2;
                    foreach (var item in data)
                    {
                        for (int i = 0; i < properties.Count; i++)
                        {
                            var value = properties[i].GetValue(item);
                            worksheet.Cell(row, i + 1).Value = value != null ? value.ToString() : string.Empty;
                        }
                        row++;
                    }

                    workbook.SaveAs(filePath);
                }
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception (e.g., using a logging framework or Console.WriteLine)
                Console.WriteLine($"Error exporting to Excel: {ex.Message}");
                return false;
            }
        }
    }
}