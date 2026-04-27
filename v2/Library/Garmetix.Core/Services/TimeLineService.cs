// Services/TimelineDataService.cs
using Garmetix.Core.Sessions;
using Garmetix.Models;
using Garmetix.Models.Accounting;
using Garmetix.Models.Bases;
using Garmetix.Models.HRM;
using System.Diagnostics;
using System.Text.Json;

namespace Garmetix.Core.Services
{
    public class TimelineDataService
    {
        private readonly string _companyName = SessionService.CompanyName()??"Garmetix";// StorageOps.GetPref("CompanyCode", "DMY");

        // Define the filename for your JSON data
        private readonly string _fileName = SessionService.StoreName() + "_timelineData.json";

        // Get the full path to the data file in the application's local data directory
        private string FilePath
        {
            get
            {
                // FileSystem.AppDataDirectory is the recommended place for app-specific data
                // This path is platform-specific (e.g., Android, iOS, Windows)
                return Path.Combine(FileSystem.AppDataDirectory, _companyName, "TimeLineData", _fileName);
            }
        }
        private static readonly JsonSerializerOptions s_writeOptions = new()
        {
            WriteIndented = true
        };
        private static readonly JsonSerializerOptions caseInsensitive_writeOptions = new()
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true // Makes the JSON file human-readable
            };
        // Constructor
        public TimelineDataService()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
            // Ensure the file exists when the service is initialized
            if (!File.Exists(FilePath))
            {
                // Create an empty JSON array if the file doesn't exist
                File.WriteAllText(FilePath, "[]");
            }
        }

        /// <summary>
        /// Reads all timeline entries from the JSON file.
        /// </summary>
        /// <returns>A list of TimelineEntry objects.</returns>
        public async Task<List<TimelineEntry>> GetAllEntriesAsync()
        {
            try
            {
                // Read the entire content of the file asynchronously
                string json = await File.ReadAllTextAsync(FilePath);

                // Deserialize the JSON string into a list of TimelineEntry objects
                // Use JsonSerializerOptions to handle case-insensitive property matching
                // and pretty printing for readability in the file.
                
                return JsonSerializer.Deserialize<List<TimelineEntry>>(json, caseInsensitive_writeOptions) ?? [];
            }
            catch (FileNotFoundException)
            {
                // If the file is not found (should be handled by constructor, but good practice)
                return [];
            }
            catch (JsonException ex)
            {
                // Handle JSON deserialization errors (e.g., malformed JSON)
                Console.WriteLine($"Error deserializing JSON: {ex.Message}");
                // Optionally, you might want to back up the corrupted file and start fresh
                return [];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred while reading entries: {ex.Message}");
                return [];
            }
        }

        public async Task MakeEntryAsync<T>(T entity)
        {
            TimelineEntry entry = new();
            try
            {
                var type = typeof(T);
                switch (type.Name)
                {
                    case "Voucher":
                        if (entity is not Voucher voucher) break; ;
                        entry.Id = voucher.Id;
                        entry.EntryDate = voucher.OnDate;
                        entry.Title = $"Voucher {voucher.VoucherType} {voucher.VoucherNumber} for {voucher.PartyName}";
                        entry.Description = $"{voucher.Particulars},Amount: {voucher.Amount}";
                        break;

                    case "CashVoucher":
                        if (entity is not CashVoucher cashVoucher) break; ;
                        entry.Id = cashVoucher.Id;
                        entry.EntryDate = cashVoucher.OnDate;
                        entry.Title = $"Cash Voucher {cashVoucher.VoucherNumber} for {cashVoucher.PartyName}";
                        entry.Description = $"{cashVoucher.VoucherType}-{cashVoucher.Particulars}, Amount: {cashVoucher.Amount}";
                        break;

                    case "SalaryPayment":
                        if (entity is not SalaryPayment salaryPayment) break; ;
                        entry.Id = salaryPayment.Id;
                        entry.EntryDate = salaryPayment.OnDate;
                        entry.Title = $"Salary Payment {salaryPayment.VoucherNumber} for {salaryPayment.Employee?.FullName}";
                        entry.Description = $"{salaryPayment.SalaryComponent}-{salaryPayment.Remarks}, Amount: {salaryPayment.Amount}";
                        break;

                    case "BankTransaction":
                        if (entity is not BankTransaction bankTranscation) break; ;
                        entry.Id = bankTranscation.Id;
                        entry.EntryDate = bankTranscation.OnDate;
                        entry.Title = $"Bank Transaction {bankTranscation.BankAccount?.AccountNumber} for {bankTranscation.Narration}";
                        entry.Description = $"{bankTranscation.PersonName}, Amount: {bankTranscation.Amount}";
                        break;

                    default:
                        try
                        {
                            if (entity== null) break; 
                            entry.Id = (entity as CEntity)!.Id;
                            entry.Title = type.Name;
                            entry.Description = entity.ToString() ?? "";
                            break;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            break;
                        }
                }
                await SaveOrUpdateEntryAsync(entry);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Saves a new timeline entry or updates an existing one.
        /// </summary>
        /// <param name="entry">The TimelineEntry object to save or update.</param>
        public async Task SaveOrUpdateEntryAsync(TimelineEntry entry)
        {
            // First, get all existing entries
            List<TimelineEntry> entries = await GetAllEntriesAsync();

            // Check if an entry with the same ID already exists
            TimelineEntry existingEntry = entries.FirstOrDefault(e => e.Id == entry.Id)!;

            if (existingEntry != null)
            {
                // If it exists, update its properties
                existingEntry.EntryDate = entry.EntryDate;
                existingEntry.Title = entry.Title;
                existingEntry.Description = entry.Description;
            }
            else
            {
                // If it's a new entry, add it to the list
                entries.Add(entry);
            }

            // Write the updated list back to the JSON file
            await WriteAllEntriesAsync(entries);
        }

        /// <summary>
        /// Deletes a timeline entry by its ID.
        /// </summary>
        /// <param name="entryId">The ID of the entry to delete.</param>
        public async Task DeleteEntryAsync(Guid entryId)
        {
            List<TimelineEntry> entries = await GetAllEntriesAsync();
            int initialCount = entries.Count;

            // Remove the entry with the matching ID
            entries.RemoveAll(e => e.Id == entryId);

            // Only write back if an entry was actually removed
            if (entries.Count < initialCount)
            {
                await WriteAllEntriesAsync(entries);
            }
        }

        /// <summary>
        /// Writes the entire list of timeline entries to the JSON file.
        /// This method is private as it's an internal helper.
        /// </summary>
        /// <param name="entries">The list of TimelineEntry objects to write.</param>
        private async Task WriteAllEntriesAsync(List<TimelineEntry> entries)
        {
            try
            {
                
                string json = JsonSerializer.Serialize(entries, s_writeOptions);

                // Write the JSON string back to the file, overwriting the old content
                await File.WriteAllTextAsync(FilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while writing entries: {ex.Message}");
                // Handle or log the error appropriately
            }
        }
    }
}