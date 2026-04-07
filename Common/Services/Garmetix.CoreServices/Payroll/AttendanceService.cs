using Bharat.ToolKits.Extensions;
using Bharat.ToolKits.Notifications;
using Garmetix.Databases;
using Garmetix.Databases.Services;
using Garmetix.Models.HRM;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Garmetix.CoreServices.Payroll
{
    public class AttendanceInfo
    {
        // Primary key for the database
        public int Id { get; set; }

        // Property to store the Date from your JSON
        public string Date { get; set; }=string.Empty;

        // Properties for Alok, Keli, and Suman
        public int Alok { get; set; }

        public int Keli { get; set; }
        public int Suman { get; set; }
    }

    public class AttendanceService
    {
        public static DatabaseContext Db => DatabaseService.Instance.LocalDB;

        /// <summary>
        /// Check for duplicate attendance
        /// </summary>
        /// <param name="attendance"></param>
        /// <returns></returns>
        public static bool DuplicateAttendaceCheck(Attendance attendance)
        {
            return Db.Attendances.Any(a => a.OnDate.Date == attendance.OnDate.Date && a.EmployeeId == attendance.EmployeeId);
        }

        public static AttendanceStatus ToStatus(int value)
        {
            if (value == 0) return AttendanceStatus.Absent;
            else if (value == 2) return AttendanceStatus.Present;
            else if (value == 1) return AttendanceStatus.HalfDay;
            return AttendanceStatus.Absent;
        }

        /// <summary>
        /// Reads a JSON file from the Raw folder and returns its content as a string.
        /// </summary>
        /// <param name="fileName">The name of the JSON file (e.g., "attendance.json").</param>
        /// <returns>The JSON content as a string, or null if the file is not found.</returns>
        public static async Task<string?> ReadJsonFileFromRawFolder(string fileName)
        {//TODO: move to one place in library
            try
            {
                // MAUI's Raw assets are accessed via FileSystem.OpenAppPackageFileAsync
                using Stream fileStream = await FileSystem.OpenAppPackageFileAsync(fileName);
                using StreamReader reader = new(fileStream);
                return await reader.ReadToEndAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading JSON file {fileName}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// <param name="attendanceRecords">The list of Attendance objects to save.</param>
        /// <param name="fileName">The name of the JSON file (e.g., "output_attendance.json").</param>
        /// <returns>True if successful, false otherwise.</returns></summary>
        public static async Task<bool> SaveAttendanceDataToJsonFile(List<Attendance> attendanceRecords, string fileName)
        {
            if (attendanceRecords == null || attendanceRecords.Count == 0)
            {
                Console.WriteLine("No attendance records to save.");
                return false;
            }

            try
            {
                // Serialize the list of Attendance objects to a JSON string
                var jsonString = JsonSerializer.Serialize(attendanceRecords, new JsonSerializerOptions
                {
                    WriteIndented = true // For pretty-printing the JSON
                });

                // Get the full path to the file in the application's data directory
                string filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

                // Write the JSON string to the file
                await File.WriteAllTextAsync(filePath, jsonString);

                Console.WriteLine($"Attendance data successfully saved to: {filePath}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving attendance data to JSON file {fileName}: {ex.Message}");
                return false;
            }
        }

        public static async Task<bool> UpdateAttendanceDataFromJsonFile(string fileName)
        {
            try
            {
                var jsonData = await ReadJsonFileFromRawFolder(fileName);

                // Parse the JSON data into a list of Attendance objects
                var attendanceRecords = JsonSerializer.Deserialize<List<Attendance>>(jsonData!, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // Allows matching "Date" to "Date" etc.
                });
                if (attendanceRecords == null || attendanceRecords.Count == 0)
                {
                    Console.WriteLine("No attendance records found in JSON data.");
                    return false;
                }

                foreach (var attendance in attendanceRecords)
                {
                    var existingAttendance = Db.Attendances.FirstOrDefault(a => a.OnDate.Date == attendance.OnDate.Date && a.EmployeeId == attendance.EmployeeId);
                    if (existingAttendance != null)
                    {
                        existingAttendance.CheckInTime = attendance.CheckInTime;
                        existingAttendance.CheckOutTime = attendance.CheckOutTime;
                        existingAttendance.Status = attendance.Status;
                        existingAttendance.Remarks += attendance.Remarks;
                        existingAttendance.Synced = false;
                        existingAttendance.UpdatedAt = DateTime.UtcNow;
                        existingAttendance.CreatedBy += "\tAutoAdmin";
                    }
                    else
                    {
                        Db.Attendances.Add(attendance);
                    }
                }

                return await Db.SaveChangesAsync() > 0;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); return false; }
        }

        /// <summary>
        /// Parses a JSON string into a list of Attendance objects and stores them in the database.
        /// Existing records with the same date will be updated; new records will be created.
        /// </summary>
        /// <param name="jsonData">The JSON string containing attendance data.</param>
        /// <returns>A list of Attendance objects that were processed.</returns>
        public static async Task<bool> UploadAttendanceData(string jsonData)
        {
            bool returnValue = false;
            if (string.IsNullOrEmpty(jsonData))
            {
                Console.WriteLine("No JSON data provided to process.");
                return returnValue;
            }

            List<AttendanceInfo> attendanceRecords = [];
            try
            {
                // Deserialize the JSON string into a list of Attendance objects
                attendanceRecords = JsonSerializer.Deserialize<List<AttendanceInfo>>(jsonData, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // Allows matching "Date" to "Date" etc.
                })!;
                var emplist = Db.Employees.Select(c => new { c.Id, c.FirstName }).ToList();
                var alokid = emplist.FirstOrDefault(c => c.FirstName == "Alok")?.Id;
                var kelid = emplist.FirstOrDefault(c => c.FirstName == "Keli")?.Id;
                var sumanid = emplist.FirstOrDefault(c => c.FirstName == "Suman")?.Id;
                if (attendanceRecords == null || attendanceRecords.Count == 0)
                {
                    Console.WriteLine("No attendance records found in JSON data.");
                    return returnValue;
                }
                List<Attendance> attendances = [];

                attendanceRecords = attendanceRecords.OrderBy(c => c.Date).ToList();
                 
                foreach (var attendanceRecord in attendanceRecords)
                {
                    var alok = new Attendance
                    {
                        CheckInTime = new TimeSpan(10, 0, 0),
                        Synced = false,
                        CheckOutTime = new TimeSpan(20, 0, 0),
                        Remarks = "Auto Generated",
                        EmployeeId = alokid!.Value,
                        Id = Guid.NewGuid(),
                        OnDate = DateTime.Parse(attendanceRecord.Date.Trim()).Date,
                        Status = ToStatus(attendanceRecord.Alok),
                        CompanyId = DatabaseService.CompanyId,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "AutoAdmin",
                        Deleted = false,
                        UpdatedAt = DateTime.UtcNow,
                        EntryTime = "10:00:00 AM",
                        StoreId = DatabaseService.StoreId,
                        StoreGroupId = DatabaseService.StoreGroupId
                    };
                    var keli = new Attendance
                    {
                        CheckInTime = new TimeSpan(10, 0, 0),
                        Synced = false,
                        CheckOutTime = new TimeSpan(20, 0, 0),
                        Remarks = "Auto Generated",
                        EmployeeId = kelid!.Value,
                        Id = Guid.NewGuid(),
                        OnDate = DateTime.Parse(attendanceRecord.Date.Trim()).Date,
                        Status = ToStatus(attendanceRecord.Keli),
                        CompanyId = DatabaseService.CompanyId,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = "AutoAdmin",
                        Deleted = false,
                        UpdatedAt = DateTime.UtcNow,
                        EntryTime = "10:00:00 AM",
                        StoreId = DatabaseService.StoreId,
                        StoreGroupId = DatabaseService.StoreGroupId
                    };
                    //var suman = new Attendance
                    //{
                    //    CheckInTime = new TimeSpan(10, 0, 0),
                    //    Synced = false,
                    //    CheckOutTime = new TimeSpan(20, 0, 0),
                    //    Remarks = "Auto Generated",
                    //    EmployeeId = sumanid!.Value,
                    //    Id = Guid.NewGuid(),
                    //    OnDate = DateTime.Parse(attendanceRecord.Date.Trim()).Date,
                    //    Status = ToStatus(attendanceRecord.Suman),
                    //    CompanyId = DatabaseService.CompanyId,
                    //    CreatedAt = DateTime.UtcNow,
                    //    CreatedBy = "AutoAdmin",
                    //    Deleted = false,
                    //    UpdatedAt = DateTime.UtcNow,
                    //    EntryTime = "10:00:00 AM",
                    //    StoreId = DatabaseService.StoreId,
                    //    StoreGroupId = DatabaseService.StoreGroupId
                    //};

                    attendances.Add(alok);
                    attendances.Add(keli);
                   // attendances.Add(suman);
                }
                foreach (var attendance in attendances)
                {
                    var existing = Db.Attendances.FirstOrDefault(c => c.OnDate == attendance.OnDate && c.EmployeeId == attendance.EmployeeId);
                    if (existing != null)
                    {
                        existing.Status = attendance.Status;
                        existing.UpdatedAt = DateTime.UtcNow;
                        Db.Attendances.Update(existing);
                    }
                    else
                    {
                        Db.Attendances.Add(attendance);
                    }
                }

                // Save all changes to the database
                returnValue = await Db.SaveChangesAsync() > 0;
                _ = await SaveAttendanceDataToJsonFile(attendances, "april_june_2025_attendance.json");
                //Temp: remvie
                Console.WriteLine("Attendance data successfully stored/updated in the database.");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON deserialization error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing and storing attendance data: {ex.Message}");
            }

            return returnValue;
        }

        /// <summary>
        /// Save Monthly attendace to file
        /// </summary>
        /// <param name="attendance"></param>
        /// <returns></returns>
        public static async Task<string> SaveMonthlyAttendaceJsonFile(MonthlyAttendance attendance)
        {
            try
            {
                // Serialize the attendance object to JSON
                var json = JsonSerializer.Serialize(attendance, new JsonSerializerOptions
                {
                    WriteIndented = true, // --- KEY CHANGE ---
                                          // Handle cyclic references by ignoring objects that have already been serialized.
                                          // This requires .NET 6 or later, which is standard for .NET MAUI.
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });

                // Build a file name using EmployeeId and OnDate (month-year)
                var fileName = $"MonthlyAttendance_{attendance.EmployeeId}_{attendance.OnDate:yyyyMM}.json";

                // Choose a directory to save the file (e.g., app data folder)
                var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

                var filePath = Path.Combine(folder, "Bharat-Garmetix", Preferences.Get("CompanyName", "DefaultCompany").Replace(" ", "_"), "Monthly Attendance", fileName);

                // Write the JSON to the file asynchronously
                await File.WriteAllTextAsync(filePath, json);

                return filePath;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await Notify.DisplayToastAsync("Error saving attendance file: " + ex.Message);
                return "Error: " + ex.Message;
            }
        }

        /// <summary>
        /// Generate Monthly attendance
        /// </summary>
        /// <param name="empid"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public static async Task<MonthlyAttendance?> GenerateMonthlyAttendance(Guid empid, DateTime month)
        {
            try
            {
                var attendances = Db.Attendances.Where(a => a.OnDate.Month == month.Month && a.OnDate.Year == month.Year && a.EmployeeId == empid)
                    .Select(c => new { c.OnDate, c.Status })
                    .ToList();

                //Calulate total working days
                MonthlyAttendance monthlyAttendance = new()
                {
                    CompanyId = DatabaseService.CompanyId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "AutoAdmin",
                    Deleted = false,
                    EmployeeId = empid,
                    Id = Guid.NewGuid(),
                    UpdatedAt = DateTime.UtcNow,
                    OnDate = month,
                    StoreId = DatabaseService.StoreId,
                    Synced = false,
                    StoreGroupId = DatabaseService.StoreGroupId,
                    NoOfWorkingDays = DateHelper.NoOfWorkingDays(month),
                    Present = attendances.Count(a => a.Status == AttendanceStatus.Present),
                    HalfDay = attendances.Count(a => a.Status == AttendanceStatus.HalfDay),
                    Absent = attendances.Count(a => a.Status == AttendanceStatus.Absent),
                    CasualLeave = attendances.Count(a => a.Status == AttendanceStatus.CasualLeave),
                    Holidays = attendances.Count(a => a.Status == AttendanceStatus.Holiday),
                    PaidLeave = attendances.Count(a => a.Status == AttendanceStatus.PaidLeave),
                    Sunday = attendances.Count(a => a.Status == AttendanceStatus.Sunday),
                    Remarks = "Monthly Attendance Generated",
                    WeeklyLeave = attendances.Count(a => a.Status == AttendanceStatus.SundayHoliday),
                };

                _ = Db.MonthlyAttendances.Add(monthlyAttendance);
                int result = await Db.SaveChangesAsync();
                return result > 0 ? monthlyAttendance : null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                _ = Notify.DisplayToastAsync("Error :" + ex.Message);
                return null;
            }
        }
    }
}