// Garmetix - A .NET MAUI Application for Garment and Textile Management

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bharat.ToolKits.Notifications;
using Garmetix.Core.Models.Accounting;
using Garmetix.Core.Models.Onboarding;
using Garmetix.Core.Models.Stores;
using Garmetix.Models.Accounting;
using Garmetix.Models.Auth;
using Garmetix.Models.Enums;
using Garmetix.Models.HRM;
using Garmetix.Models.Inventory;

namespace Garmetix.Databases.Seeds;

// ToDO: Move Database Services
public class Seeder2
{
    private DatabaseContext db = DatabaseContext.Instance;
    private readonly ApplicationDatabaseContext appDb = ApplicationDatabaseContext.Instance ?? new ApplicationDatabaseContext();

    public async Task<string> SeedDatabaseAsync(Store store, Company company, StoreGroup group, KeyPersonalInfo keyPersonalInfo, string baseCompanyUrl)
    {
        StringBuilder message = new StringBuilder();
        message.AppendLine($"Seeding Database for {company.Name}, {store.Name}, {group.Name}");

        try
        {
            // Initialize Data sequentially to maintain foreign key integrity
            message.AppendLine(await AddBanksAsync());
            message.AppendLine(await AddTaxesAsync(store));
            message.AppendLine(await AddTransactionsAsync(company));
            message.AppendLine(await AddLedgerAndGroupsAsync(store, company));
            message.AppendLine(await AddingOwnersAsync(store));
            message.AppendLine(await AddingEmployeesAsync(store, keyPersonalInfo));
            message.AppendLine(await InitialUsersAsync(store, baseCompanyUrl));

            message.AppendLine("Database Seeding Completed Successfully.");
        }
        catch (Exception ex)
        {
            message.AppendLine($"Critical error during database seeding: {ex.Message}");
        }

        return message.ToString();
    }

    public async Task<string> InitSamratMenswearAsync()
    {
        string baseMessage = "Aadwika Fashion, Samrat Menswear Initial Data: ";

        try
        {
            var company = new Company()
            {
                Name = "Aadwika Fashion",
                GSTIN = "20AJHPA73096P1ZV",
                Pan = "AJHPA7397P",
                City = "Dumka",
                Code = "SM",
                CompanyType = CompanyType.Proprietorship,
                ContactNumber = "9334799099",
                Country = "India",
                Deleted = false,
                EndDate = null,
                Id = Guid.NewGuid(),
                Synced = false,
                StartDate = DateTime.Now.Date,
                StoreCategory = StoreCategory.Cloths,
                State = "Jharkhand",
                ContactPerson = "Amit Kumar",
                ZipCode = "84101",
                Active = true,
                Address = "Bhagalpur Road, Dumka",
                CIN = "NA",
                ContactMobile = "9334799099",
                Email = "samratmenswear@aadwikafashion.com",
            };

            var group = new StoreGroup()
            {
                Active = true,
                CompanyId = company.Id,
                EndDate = null,
                GroupCode = "MBO-SM",
                Id = Guid.NewGuid(),
                Name = "Samrat Menswear Group",
                StartDate = DateTime.Now.Date,
                StoreCategory = StoreCategory.Cloths,
                Deleted = false,
                Company = company,
                Synced = false
            };

            var store = new Store()
            {
                Country = "India",
                State = "Jharkhand",
                ZipCode = "84101",
                Active = true,
                City = "Dumka",
                CompanyId = company.Id,
                EndDate = null,
                Id = Guid.NewGuid(),
                Name = "Samrat Menswear",
                StartDate = DateTime.Now.Date,
                StoreCategory = StoreCategory.Cloths,
                Deleted = false,
                Synced = false,
                StoreCode = "SM01",
                StoreGroupId = group.Id,
                ContactNumber = "9334799099",
                Email = "samratmenswear@aadwikafashion.com",
                Address = "Bhagalpur Road, Dumka",
                Company = company,
                StoreGroup = group
            };

            var databaseFilename = company.Code + Constants.DatabaseFilename;
            db = new DatabaseContext(databaseFilename);

            appDb.Companies.Add(company);
            appDb.StoreGroups.Add(group);
            appDb.Stores.Add(store);
            await appDb.SaveChangesAsync();

            db.Companies.Add(company);
            db.StoreGroups.Add(group);
            db.Stores.Add(store);

            var kpi = new KeyPersonalInfo
            {
                StoreManagerName = "Alok Kumar",
                StoreManagerPhoneNumber = "1234567890",
                StoreManagerEmail = "alok@aadwikafashion.in",
                AccountantEmail = "accountant@aadwikafashion.in",
                AccountantName = "Accountant Manager",
                AccountantPhoneNumber = "9876543210"
            };

            await db.SaveChangesAsync();

            baseMessage += "Store, StoreGroup and Company added and Database created successfully.\n";
            baseMessage += await SeedDatabaseAsync(store, company, group, kpi, "aadwikafashion.com");

            _ = Task.Run(async () => { await Notify.DisplayNotificationAsync(baseMessage, isLong: false, speak: true); });
            await Notify.DisplayNotificationAsync(baseMessage, isLong: true, speak: false);

            return baseMessage;
        }
        catch (Exception ex)
        {
            return $"{baseMessage} Failed to initialize database. Error: {ex.Message}";
        }
    }

    public async Task<string> InitAadwikaFashionAmitKumarAsync()
    {
        string baseMessage = "Aadwika Fashion, Amit Kumar Initial Data: ";

        try
        {
            var company = new Company()
            {
                Name = "Aadwika Fashion",
                GSTIN = "20AJHPA7396P1ZV",
                Pan = "AJHPA7396P",
                City = "Dumka",
                Code = "AF",
                CompanyType = CompanyType.Proprietorship,
                ContactNumber = "9334799099",
                Country = "India",
                Deleted = false,
                EndDate = null,
                Id = Guid.NewGuid(),
                Synced = false,
                StartDate = DateTime.Now.Date,
                StoreCategory = StoreCategory.Cloths,
                State = "Jharkhand",
                ContactPerson = "Amit Kumar",
                ZipCode = "814101",
                Active = true,
                Address = "Ground Floor, Bhagalpur Road, Dumka",
                CIN = "33AABCA1234A1Z5",
                ContactMobile = "9334799099",
                Email = "aadwikafashion@gmail.com",
            };

            var group = new StoreGroup()
            {
                Active = true,
                CompanyId = company.Id,
                EndDate = null,
                GroupCode = "MBO",
                Id = Guid.NewGuid(),
                Name = "Aadwika Fashion MBO",
                StartDate = DateTime.Now.Date,
                StoreCategory = StoreCategory.Cloths,
                Deleted = false,
                Company = company,
                Synced = false
            };

            var store = new Store()
            {
                Country = "India",
                State = "Jharkhand",
                ZipCode = "814101",
                Active = true,
                City = "Dumka",
                CompanyId = company.Id,
                EndDate = null,
                Id = Guid.NewGuid(),
                Name = "Aadwika Fashion MBO Dumka",
                StartDate = DateTime.Now.Date,
                StoreCategory = StoreCategory.Cloths,
                Deleted = false,
                Synced = false,
                StoreCode = "AFMBO",
                StoreGroupId = group.Id,
                ContactNumber = "9334799099",
                Email = "aadwikafashion@gmail.com",
                Address = "Ground Floor, Bhagalpur Road, Dumka",
                Company = company,
                StoreGroup = group
            };

            var databaseFilename = company.Code + Constants.DatabaseFilename;
            db = new DatabaseContext(databaseFilename);

            appDb.Companies.Add(company);
            appDb.StoreGroups.Add(group);
            appDb.Stores.Add(store);
            await appDb.SaveChangesAsync();

            db.Companies.Add(company);
            db.StoreGroups.Add(group);
            db.Stores.Add(store);

            var kpi = new KeyPersonalInfo
            {
                StoreManagerName = "Alok Kumar",
                StoreManagerPhoneNumber = "1234567890",
                StoreManagerEmail = "alok@aadwikafashion.in",
                AccountantEmail = "accountant@aadwikafashion.in",
                AccountantName = "Accountant Manager",
                AccountantPhoneNumber = "9876543210"
            };

            await db.SaveChangesAsync();

            baseMessage += "Store, StoreGroup and Company added and Database created successfully.\n";
            baseMessage += await SeedDatabaseAsync(store, company, group, kpi, "aadwikafashion.com");

            _ = Task.Run(async () => { await Notify.DisplayNotificationAsync(baseMessage, isLong: false, speak: true); });
            await Notify.DisplayNotificationAsync(baseMessage, isLong: true, speak: false);

            return baseMessage;
        }
        catch (Exception ex)
        {
            return $"{baseMessage} Failed to initialize database. Error: {ex.Message}";
        }
    }

    public async Task<string> InitAadwikaFashionShaliniKumariAsync()
    {
        string baseMessage = "Aadwika Fashion, Shalini Kumari Initial Data: ";

        try
        {
            var company = new Company()
            {
                Name = "Aadwika Fashion",
                GSTIN = "20CLEPK0467L1Z8",
                Pan = "CLEPK0467L",
                Address = "Ground Floor, Bhagalpur Road, Dumka",
                City = "Dumka",
                Code = "AFS",
                CompanyType = CompanyType.Proprietorship,
                ContactNumber = "8409201476",
                Country = "India",
                Deleted = false,
                EndDate = null,
                Id = Guid.NewGuid(),
                Synced = false,
                StartDate = DateTime.Now.Date,
                StoreCategory = StoreCategory.Cloths,
                State = "Jharkhand",
                ContactPerson = "Shalini Kumari",
                ZipCode = "814101",
                Active = true,
                CIN = "NA",
                ContactMobile = "8409201476",
                Email = "aadwikafashion.mbo@gmail.com",
            };

            var group = new StoreGroup()
            {
                Active = true,
                CompanyId = company.Id,
                EndDate = null,
                GroupCode = "MBO",
                Id = Guid.NewGuid(),
                Name = "Aadwika Fashion MBO",
                StartDate = DateTime.Now.Date,
                StoreCategory = StoreCategory.Cloths,
                Deleted = false,
                Synced = false,
                Company = company
            };

            var store = new Store()
            {
                Company = company,
                StoreGroup = group,
                Country = "India",
                State = "Jharkhand",
                ZipCode = "814101",
                Active = true,
                Address = " Ground Floor, Bhagalpur Road, Dumka",
                City = "Dumka",
                CompanyId = company.Id,
                EndDate = null,
                Id = Guid.NewGuid(),
                Name = "Aadwika Fashion MBO Dumka",
                StartDate = DateTime.Now.Date,
                StoreCategory = StoreCategory.Cloths,
                Deleted = false,
                Synced = false,
                StoreCode = "AFSMBO",
                StoreGroupId = group.Id,
                ContactNumber = "8409201476",
                Email = "aadwikafashion.mbo@gmail.com",
            };

            var databaseFilename = company.Code + Constants.DatabaseFilename;
            db = new DatabaseContext(databaseFilename);

            appDb.Companies.Add(company);
            appDb.StoreGroups.Add(group);
            appDb.Stores.Add(store);
            await appDb.SaveChangesAsync();

            db.Companies.Add(company);
            db.StoreGroups.Add(group);
            db.Stores.Add(store);

            var kpi = new KeyPersonalInfo
            {
                StoreManagerName = "Alok Kumar",
                StoreManagerPhoneNumber = "1234567890",
                StoreManagerEmail = "alok@aadwikafashion.in",
                AccountantEmail = "accountant@aadwikafashion.in",
                AccountantName = "Accountant Manager",
                AccountantPhoneNumber = "9876543210"
            };

            await db.SaveChangesAsync();

            baseMessage += "Store, StoreGroup and Company added and Database created successfully.\n";
            baseMessage += await SeedDatabaseAsync(store, company, group, kpi, "aadwikafashion.in");

            _ = Task.Run(async () => { await Notify.DisplayNotificationAsync(baseMessage, isLong: false, speak: true); });
            return baseMessage;
        }
        catch (Exception ex)
        {
            return $"{baseMessage} Failed to initialize database. Error: {ex.Message}";
        }
    }

    private async Task<string> AddingOwnersAsync(Store store)
    {
        try
        {
            var owner = new Employee
            {
                Aadhar = "NA",
                Category = EmployeeCategory.Owner,
                CompanyId = store.CompanyId,
                FirstName = store.Company?.ContactPerson.Split(" ")[0] ?? "",
                Id = Guid.NewGuid(),
                LastName = store.Company?.ContactPerson.Split(" ").Skip(1).FirstOrDefault() ?? "",
                Mobile = store.Company?.ContactMobile ?? "",
                DateOfBirth = DateTime.Now.Date.AddYears(-25),
                EmpId = 1,
                Gender = Gender.Male,
                JoiningDate = (DateTime)(store.Company?.StartDate),
                LeavingDate = null,
                PAN = "NA",
                StoreGroupId = store.StoreGroupId,
                StoreId = store.Id,
                Synced = false,
                Title = "Mr.",
                UpdatedAt = DateTime.Now,
                Working = true,
                CreatedAt = DateTime.Now,
                Deleted = false,
                Email = store.Company?.Email,
            };

            if (owner.FirstName.Equals("Shalini", StringComparison.OrdinalIgnoreCase))
            {
                owner.Title = "Mrs.";
                owner.Gender = Gender.Female;
            }

            db.Employees.Add(owner);
            await db.SaveChangesAsync();
            return "Owner Added Successfully";
        }
        catch (Exception ex)
        {
            return $"Error adding owner: {ex.Message}";
        }
    }

    private async Task<string> AddingEmployeesAsync(Store store, KeyPersonalInfo kpi)
    {
        try
        {
            var smFirstName = kpi.StoreManagerName.Split(" ")[0];
            var smLastName = kpi.StoreManagerName.Split(" ").Skip(1).FirstOrDefault() ?? "";

            var sm = new Employee
            {
                Aadhar = "NA",
                Category = EmployeeCategory.StoreManager,
                CompanyId = store.CompanyId,
                FirstName = smFirstName,
                Id = Guid.NewGuid(),
                LastName = smLastName,
                Mobile = kpi.StoreManagerPhoneNumber,
                DateOfBirth = DateTime.Now.Date.AddYears(-25),
                EmpId = 2,
                Gender = Gender.Male,
                CreatedAt = DateTime.Now,
                JoiningDate = store.StartDate,
                LeavingDate = null,
                PAN = "NA",
                StoreGroupId = store.StoreGroupId,
                StoreId = store.Id,
                Synced = false,
                Title = "Mr.",
                UpdatedAt = DateTime.Now,
                Working = true,
                Deleted = false,
                Email = kpi.StoreManagerEmail,
                CreatedBy = "AutoAdmin",
            };

            var accFirstName = kpi.AccountantName.Split(" ")[0];
            var accLastName = kpi.AccountantName.Split(" ").Skip(1).FirstOrDefault() ?? "";

            var accountant = new Employee
            {
                Aadhar = "NA",
                Category = EmployeeCategory.Accounts,
                CompanyId = store.CompanyId,
                FirstName = accFirstName,
                Id = Guid.NewGuid(),
                LastName = accLastName,
                Mobile = kpi.AccountantPhoneNumber,
                DateOfBirth = DateTime.Now.Date.AddYears(-25),
                EmpId = 3,
                CreatedBy = "AutoAdmin",
                CreatedAt = DateTime.UtcNow,
                Deleted = false,
                Email = kpi.AccountantEmail,
                Gender = Gender.Male,
                JoiningDate = store.StartDate,
                PAN = "NA",
                StoreGroupId = store.StoreGroupId,
                StoreId = store.Id,
                Synced = false,
                Title = "Mr.",
                UpdatedAt = DateTime.UtcNow,
                Working = true,
                LeavingDate = null,
            };

            var manager = new Salesman
            {
                StoreId = store.Id,
                Synced = false,
                Deleted = false,
                EmployeeId = null,
                Id = sm.Id,
                Name = "Manager",
                StoreGroupId = store.StoreGroupId,
                CompanyId = store.CompanyId
            };

            db.Employees.Add(accountant);
            db.Employees.Add(sm);
            db.Salesmen.Add(manager);

            await db.SaveChangesAsync();
            return "Employees Added Successfully";
        }
        catch (Exception ex)
        {
            return $"Error adding employees: {ex.Message}";
        }
    }

    private async Task<string> InitialUsersAsync(Store store, string baseCompanyUrl)
    {
        try
        {
            var admin = new AppUser
            {
                Admin = true,
                UserType = UserType.Admin,
                AppOperation = AppOperation.Company,
                CompanyId = store.CompanyId,
                Email = $"admin@{baseCompanyUrl}",
                EmployeeId = null,
                Id = Guid.NewGuid(),
                Password = "Admin@1234",
                Name = "Admin",
                Role = LoginRole.Admin,
                StoreGroupId = store.StoreGroupId,
                StoreId = store.Id,
                UserName = "Admin",
                RemoteUserId = null,
                PinHash = "1234"
            };

            var owner = new AppUser
            {
                Admin = true,
                UserType = UserType.Owner,
                AppOperation = AppOperation.All,
                CompanyId = store.CompanyId,
                Email = store.Email,
                EmployeeId = null,
                Id = Guid.NewGuid(),
                Password = "Owner@1234",
                RemoteUserId = null,
                Name = store.Company.ContactPerson,
                Role = LoginRole.Member,
                StoreGroupId = store.StoreGroupId,
                StoreId = store.Id,
                UserName = store.Company.ContactPerson.Replace(" ", "").Trim(),
                PinHash = "1234"
            };

            var cashier = new AppUser
            {
                Admin = true,
                UserType = UserType.StoreManager,
                AppOperation = AppOperation.Store,
                CompanyId = store.CompanyId,
                Email = $"storemanager@{baseCompanyUrl}",
                EmployeeId = null,
                Id = Guid.NewGuid(),
                Password = "StoreManager@1234",
                Name = "Store Manager",
                RemoteUserId = null,
                Role = LoginRole.StoreManager,
                StoreGroupId = store.StoreGroupId,
                StoreId = store.Id,
                UserName = "StoreManager",
                PinHash = "1234"
            };

            db.AppUsers.AddRange(admin, owner, cashier);
            appDb.AppUsers.AddRange(admin, owner, cashier);

            await appDb.SaveChangesAsync();
            await db.SaveChangesAsync();

            return "Users Added Successfully";
        }
        catch (Exception ex)
        {
            return $"Error adding initial users: {ex.Message}";
        }
    }

    private async Task<string> AddTaxesAsync(Store store)
    {
        try
        {
            var taxes = new[]
            {
                new Tax { Id = Guid.NewGuid(), Name = "GST 5%", CompositeRate = 5, TaxType = TaxType.GST, Deleted = false, Synced = false },
                new Tax { Id = Guid.NewGuid(), Name = "GST 12%", CompositeRate = 12, TaxType = TaxType.GST },
                new Tax { Id = Guid.NewGuid(), Name = "GST 18%", CompositeRate = 18, TaxType = TaxType.GST },
                new Tax { Id = Guid.NewGuid(), Name = "IGST 5%", CompositeRate = 5, TaxType = TaxType.IGST },
                new Tax { Id = Guid.NewGuid(), Name = "IGST 12%", CompositeRate = 12, TaxType = TaxType.IGST },
                new Tax { Id = Guid.NewGuid(), Name = "IGST 18%", CompositeRate = 18, TaxType = TaxType.IGST },
                new Tax { Id = Guid.NewGuid(), Name = "CGST 2.5%", CompositeRate = 5, TaxType = TaxType.GST },
                new Tax { Id = Guid.NewGuid(), Name = "SGST 2.5%", CompositeRate = 12, TaxType = TaxType.GST }
            };

            db.Taxes.AddRange(taxes);
            await db.SaveChangesAsync();
            return "Taxes Added Successfully";
        }
        catch (Exception ex)
        {
            return $"Error adding taxes: {ex.Message}";
        }
    }

    private async Task<string> AddLedgerAndGroupsAsync(Store store, Company company)
    {
        StringBuilder message = new StringBuilder();
        message.AppendLine($"Adding Ledger Groups and Ledgers for {store.Name}, {company.Name}");

        try
        {
            var snackgroups = new LedgerGroup { Category = LedgerCategory.Expenses, CompanyId = company.Id, Deleted = false, Id = Guid.NewGuid(), Name = "Snacks & Refeshments", Remarks = "Store Snacks & Refeshments Expenses", Synced = false };
            var storeexpenses = new LedgerGroup { Category = LedgerCategory.Expenses, CompanyId = company.Id, Deleted = false, Id = Guid.NewGuid(), Name = "Store Expenses", Remarks = "Store Expenses", Synced = false };
            var directExpenses = new LedgerGroup { Category = LedgerCategory.Expenses, CompanyId = company.Id, Deleted = false, Id = Guid.NewGuid(), Name = "Direct Expenses", Remarks = "Store Direct Expenses", Synced = false };
            var indirectExpenses = new LedgerGroup { Category = LedgerCategory.Expenses, CompanyId = company.Id, Deleted = false, Id = Guid.NewGuid(), Name = "Indirect Expenses", Remarks = "Store Indirect Expenses", Synced = false };
            var pettyExpenses = new LedgerGroup { Category = LedgerCategory.Expenses, CompanyId = company.Id, Deleted = false, Id = Guid.NewGuid(), Name = "Petty Expenses", Remarks = "Store Petty Expenses", Synced = false };
            var noGroup = new LedgerGroup { Category = LedgerCategory.UnCategory, CompanyId = company.Id, Deleted = false, Id = Guid.NewGuid(), Name = "No Group", Remarks = "NO GROUP & Party, Group for UnCategory Ledger", Synced = false };
            var sales = new LedgerGroup { Category = LedgerCategory.Income, CompanyId = company.Id, Deleted = false, Id = Guid.NewGuid(), Name = "Sales", Remarks = "Store Sales", Synced = false };
            var purchases = new LedgerGroup { Category = LedgerCategory.Purchase, CompanyId = company.Id, Deleted = false, Id = Guid.NewGuid(), Name = "Purchases", Remarks = "Store Purchase", Synced = false };
            var cashs = new LedgerGroup { Category = LedgerCategory.Assets, CompanyId = company.Id, Deleted = false, Id = Guid.NewGuid(), Name = "Cash", Remarks = "Store Cash(s)", Synced = false };
            var banks = new LedgerGroup { Category = LedgerCategory.Assets, CompanyId = company.Id, Deleted = false, Id = Guid.NewGuid(), Name = "Banks", Remarks = "Store Bank", Synced = false };
            var capitalAccounts = new LedgerGroup { Id = Guid.NewGuid(), Name = "Capital Accounts", Category = LedgerCategory.Assets, CompanyId = company.Id, Deleted = false, Synced = false, Remarks = "Store Capital Account" };
            var loansAndAdvances = new LedgerGroup { Id = Guid.NewGuid(), Name = "Loans and Adavances", Category = LedgerCategory.Assets, CompanyId = company.Id, Deleted = false, Synced = false, Remarks = "Store Loan Account" };
            var vendors = new LedgerGroup { Id = Guid.NewGuid(), Name = "Vendors", Category = LedgerCategory.Vendor, CompanyId = company.Id, Deleted = false, Synced = false, Remarks = "Store Vendor Account" };
            var customers = new LedgerGroup { Id = Guid.NewGuid(), Name = "Customers", Category = LedgerCategory.Customer, CompanyId = company.Id, Deleted = false, Synced = false, Remarks = "Store Customer Account" };
            var employees = new LedgerGroup { Id = Guid.NewGuid(), Name = "Employees", Category = LedgerCategory.Employees, CompanyId = company.Id, Deleted = false, Synced = false, Remarks = "Store Employee Account" };
            var stocks = new LedgerGroup { Id = Guid.NewGuid(), Name = "Stock", Category = LedgerCategory.Stock, CompanyId = company.Id, Deleted = false, Synced = false, Remarks = "Store Stock Account" };
            var debitors = new LedgerGroup { Id = Guid.NewGuid(), Name = "Debitors", Category = LedgerCategory.Debitor, CompanyId = company.Id, Deleted = false, Synced = false, Remarks = "Store Stock Account" };
            var creditors = new LedgerGroup { Id = Guid.NewGuid(), Name = "Creditors", Category = LedgerCategory.Creditor, CompanyId = company.Id, Deleted = false, Synced = false, Remarks = "Store Stock Account" };

            db.LedgerGroups.AddRange(cashs, stocks, indirectExpenses, directExpenses, pettyExpenses, debitors, creditors, noGroup, sales, purchases, banks, loansAndAdvances, capitalAccounts, vendors, customers, employees, snackgroups, storeexpenses);

            var ledgers = new[]
            {
                new Ledger { OpenningBalance = 0, LedgerGroupId = pettyExpenses.Id, IsParty = false, CreatedBy = "AutoAdmin", Deleted = false, Synced = false, CompanyId = company.Id, Name = "Dan", Id = Guid.NewGuid(), OpenningDate = company.StartDate, LedgerType = LedgerType.Expenses, CreatedAt = DateTime.UtcNow },
                new Ledger { OpenningBalance = 0, LedgerGroupId = snackgroups.Id, IsParty = false, CreatedBy = "AutoAdmin", Deleted = false, Synced = false, CreatedAt = DateTime.UtcNow, CompanyId = company.Id, Name = "Snacks & Tea", Id = Guid.NewGuid(), OpenningDate = company.StartDate, LedgerType = LedgerType.Expenses },
                new Ledger { OpenningBalance = 0, LedgerGroupId = storeexpenses.Id, CreatedAt = DateTime.UtcNow, IsParty = false, CreatedBy = "AutoAdmin", Deleted = false, Synced = false, CompanyId = company.Id, Name = "Electricity", Id = Guid.NewGuid(), OpenningDate = company.StartDate, LedgerType = LedgerType.Expenses },
                new Ledger { OpenningBalance = 0, LedgerGroupId = snackgroups.Id, IsParty = false, CreatedBy = "AutoAdmin", CreatedAt = DateTime.UtcNow, Deleted = false, Synced = false, CompanyId = company.Id, Name = "Water", Id = Guid.NewGuid(), OpenningDate = company.StartDate, LedgerType = LedgerType.Expenses },
                new Ledger { OpenningBalance = 0, LedgerGroupId = storeexpenses.Id, CreatedAt = DateTime.UtcNow, IsParty = false, CreatedBy = "AutoAdmin", Deleted = false, Synced = false, CompanyId = company.Id, Name = "Printing & Stationery", Id = Guid.NewGuid(), OpenningDate = company.StartDate, LedgerType = LedgerType.Expenses },
                new Ledger { OpenningBalance = 0, LedgerGroupId = directExpenses.Id, IsParty = false, CreatedBy = "AutoAdmin", CreatedAt = DateTime.UtcNow, Deleted = false, Synced = false, CompanyId = company.Id, Name = "Transports & Freight Charges", Id = Guid.NewGuid(), OpenningDate = company.StartDate, LedgerType = LedgerType.Expenses },
                new Ledger { OpenningBalance = 0, LedgerGroupId = storeexpenses.Id, IsParty = false, CreatedBy = "AutoAdmin", CreatedAt = DateTime.UtcNow, Deleted = false, Synced = false, CompanyId = company.Id, Name = "Miscellaneous", Id = Guid.NewGuid(), OpenningDate = company.StartDate, LedgerType = LedgerType.Expenses },
                new Ledger { OpenningBalance = 0, LedgerGroupId = noGroup.Id, IsParty = false, CreatedBy = "AutoAdmin", CreatedAt = DateTime.UtcNow, Deleted = false, Synced = false, CompanyId = company.Id, Name = "No Party", Id = Guid.NewGuid(), OpenningDate = company.StartDate, LedgerType = LedgerType.IndirectExpenses },
                new Ledger { OpenningBalance = 0, LedgerGroupId = cashs.Id, Deleted = false, Synced = false, CompanyId = company.Id, Name = "Cash In Hand", Id = Guid.NewGuid(), OpenningDate = company.StartDate, LedgerType = LedgerType.Cash },
                new Ledger { OpenningBalance = 0, LedgerGroupId = directExpenses.Id, IsParty = false, CreatedBy = "AutoAdmin", CreatedAt = DateTime.UtcNow, Deleted = false, Synced = false, CompanyId = company.Id, Name = "Salary Payables", Id = Guid.NewGuid(), OpenningDate = company.StartDate, LedgerType = LedgerType.Expenses },
                new Ledger { OpenningBalance = 0, LedgerGroupId = storeexpenses.Id, IsParty = false, CreatedBy = "AutoAdmin", CreatedAt = DateTime.UtcNow, Deleted = false, Synced = false, CompanyId = company.Id, Name = "Internet & Mobile Bills", Id = Guid.NewGuid(), OpenningDate = company.StartDate, LedgerType = LedgerType.Expenses },
                new Ledger { OpenningBalance = 0, LedgerGroupId = storeexpenses.Id, IsParty = false, CreatedBy = "AutoAdmin", CreatedAt = DateTime.UtcNow, Deleted = false, Synced = false, CompanyId = company.Id, Name = "Store Maintenance", Id = Guid.NewGuid(), OpenningDate = company.StartDate, LedgerType = LedgerType.Expenses },
                new Ledger { OpenningBalance = 0, LedgerGroupId = storeexpenses.Id, IsParty = false, CreatedBy = "AutoAdmin", CreatedAt = DateTime.UtcNow, Deleted = false, Synced = false, CompanyId = company.Id, Name = "Store Supplies", Id = Guid.NewGuid(), OpenningDate = company.StartDate, LedgerType = LedgerType.Expenses },
                new Ledger { OpenningBalance = 0, LedgerGroupId = pettyExpenses.Id, IsParty = false, CreatedBy = "AutoAdmin", CreatedAt = DateTime.UtcNow, Deleted = false, Synced = false, CompanyId = company.Id, Name = "Petty Cash Expenses", Id = Guid.NewGuid(), OpenningDate = company.StartDate, LedgerType = LedgerType.Expenses }
            };

            db.Ledgers.AddRange(ledgers);
            await db.SaveChangesAsync();
            message.AppendLine("Ledger Groups and Ledgers Added Successfully.");

            if (company.Name == "Aadwika Fashion")
            {
                var sbiBank = db.Banks.FirstOrDefault(x => x.Name == "State Bank Of India");

                if (sbiBank != null)
                {
                    var sbicurledger = new Ledger
                    {
                        LedgerGroupId = banks.Id,
                        LedgerType = LedgerType.BankAccount,
                        Name = "State Bank of India (SBI) Current Account",
                        Synced = false,
                        IsParty = false,
                        CreatedBy = "AutoAdmin",
                        CreatedAt = DateTime.UtcNow,
                        CompanyId = company.Id,
                        Deleted = false,
                        Id = Guid.NewGuid(),
                        OpenningBalance = 0,
                        OpenningDate = company.StartDate,
                    };

                    var sbicc = new BankAccount
                    {
                        Deleted = false,
                        Id = Guid.NewGuid(),
                        Branch = company.City,
                        ClosingBalance = 0,
                        ClosingDate = null,
                        IFSCode = "NA",
                        LedgerId = sbicurledger.Id,
                        OpeningBalance = 0,
                        OpeningDate = company.StartDate,
                        Synced = false,
                        Active = true,
                        BankId = sbiBank.Id,
                        CompanyId = company.Id,
                        AccountHolderName = company.Name,
                        AccountNumber = "SBI Current Account",
                        AccountType = AccountType.Current,
                    };

                    db.Ledgers.Add(sbicurledger);
                    db.BankAccounts.Add(sbicc);
                    await db.SaveChangesAsync();
                    message.AppendLine("Bank Account and Ledger Added Successfully for Aadwika Fashion.");
                }
            }

            return message.ToString();
        }
        catch (Exception ex)
        {
            return $"Error adding ledgers and groups: {ex.Message}";
        }
    }

    private async Task<string> AddBanksAsync()
    {
        try
        {
            var banks = new[]
            {
                new Bank { Id = Guid.NewGuid(), Name = "State Bank Of India" },
                new Bank { Id = Guid.NewGuid(), Name = "ICICI Bank" },
                new Bank { Id = Guid.NewGuid(), Name = "HDFC Bank" },
                new Bank { Id = Guid.NewGuid(), Name = "Bank of Baroda" },
                new Bank { Id = Guid.NewGuid(), Name = "Kotka Bank" }, // Kept original spelling
                new Bank { Id = Guid.NewGuid(), Name = "Punjab National Bank" }
            };

            db.Banks.AddRange(banks);
            await db.SaveChangesAsync();
            return "Banking Data Added Successfully.";
        }
        catch (Exception ex)
        {
            return $"Error adding banking data: {ex.Message}";
        }
    }

    private async Task<string> AddTransactionsAsync(Company company)
    {
        try
        {
            var transactions = new[]
            {
                new Transaction { Deleted = false, Id = Guid.NewGuid(), CompanyId = company.Id, Synced = false, Name = "Petty Cash Expenses" },
                new Transaction { Deleted = false, Id = Guid.NewGuid(), CompanyId = company.Id, Synced = false, Name = "Home Expenses" },
                new Transaction { Deleted = false, Id = Guid.NewGuid(), CompanyId = company.Id, Synced = false, Name = "Store Expenses" },
                new Transaction { Deleted = false, Id = Guid.NewGuid(), CompanyId = company.Id, Synced = false, Name = "Dan & Donations" },
                new Transaction { Deleted = false, Id = Guid.NewGuid(), CompanyId = company.Id, Synced = false, Name = "Snacks & Breakfast Expenses" },
                new Transaction { Deleted = false, Id = Guid.NewGuid(), CompanyId = company.Id, Synced = false, Name = "Cash In" },
                new Transaction { Deleted = false, Id = Guid.NewGuid(), CompanyId = company.Id, Synced = false, Name = "Cash Out" }
            };

            db.Transactions.AddRange(transactions);
            await db.SaveChangesAsync();
            return "Transactions Added Successfully.";
        }
        catch (Exception ex)
        {
            return $"Error adding transactions: {ex.Message}";
        }
    }
}