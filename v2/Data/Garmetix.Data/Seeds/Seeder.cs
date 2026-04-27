//Garmetix - A.NET MAUI Application for Garment and Textile Management

using Bharat.ToolKits.Notifications;
using Garmetix.Core.Models.Accounting;
using Garmetix.Models.Accounting;
using Garmetix.Models.Auth;
using Garmetix.Models.Enums;
using Garmetix.Models.HRM;
using Garmetix.Models.Inventory;
using Garmetix.Models.Onboarding;
using Garmetix.Models.Stores;

namespace Garmetix.Databases.Seeds;
//ToDO: Move Database Services
public class Seeder
{
    private DatabaseContext db = DatabaseContext.Instance;
    private readonly ApplicationDatabaseContext appDb = ApplicationDatabaseContext.Instance??new ApplicationDatabaseContext();
    private int count = 0;
    private int saved = 0;

    public string SeedDatabase(Store store, Company company, StoreGroup group, KeyPersonalInfo keyPersonalInfo, string baseCompanyUrl)
    {
        string message = "Seeding Database for " + company.Name + ", " + store.Name + ", " + group.Name;

        //Initialize Data
        //Addng Banks
        message += AddBanks();

        //Adding Taxes
        message += AddTaxes(store);

        //Adding Transactions
        message += AddTransactions(company);

        //Adding Ledger Groups
        message += AddLedgerAndGroups(store, company);
        //Adding owners
        message += AddingOwners(store);
        //Adding Employees
        message += AddingEmployees(store, keyPersonalInfo);
        //Adding Users
        message += InitialUsers(store, baseCompanyUrl);

        return message + " Completed Successfully.";
    }

    public async Task<string> InitAadwikaFashionAmitKumarAsync()
    {
        string Message = "Aadwika Fashion, Amit Kumar Initial Data: ";

        //First Company
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

        //Create Database for Company
        var databaseFilename = company.Code + Constants.DatabaseFilename;
        db = new DatabaseContext(databaseFilename);
        //Add Company, StoreGroup and Store to the database

        appDb.Companies.Add(company);
        appDb.StoreGroups.Add(group);
        appDb.Stores.Add(store);

        appDb.SaveChanges();

        db.Companies.Add(company);
        db.StoreGroups.Add(group);
        db.Stores.Add(store);

        //Adding Employees
        var kpi = new KeyPersonalInfo
        {
            StoreManagerName = "Alok Kumar",
            StoreManagerPhoneNumber = "1234567890",
            StoreManagerEmail = "alok@aadwikafashion.in",

            AccountantEmail = "accountant@aadwikafashion.in",
            AccountantName = "Accountant Manger",
            AccountantPhoneNumber = "9876543210"
        };

        count = 3;
        saved += db.SaveChanges();
        if (saved >= 3)
        {
            Message += "Store, StoreGroup and  Company is added and Database is created. Database Created Successfully.";
            Message += SeedDatabase(store, company, group, kpi, "aadwikafashion.com");
        }
        else
        {
            Message += "Store, StoreGroup and  Company is not added. Database is not created.";
        }

        _ = Task.Run(async () => { await Notify.DisplayNotificationAsync(Message, isLong: false, speak: true); });
        await Notify.DisplayNotificationAsync(Message, isLong: true, speak: false);
        return Message;
    }

    public string InitAadwikaFashionShaliniKumari()
    {
        string Message = "Aadwika Fashion, Shalini Kumari Initial Data: ";

        //First Company
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
        //Create Database for Company
        var databaseFilename = company.Code + Constants.DatabaseFilename;
        db = new DatabaseContext(databaseFilename);
        //Add Company, StoreGroup and Store to the database
        appDb.Companies.Add(company);
        appDb.StoreGroups.Add(group);
        appDb.Stores.Add(store);

        appDb.SaveChanges();

        db.Companies.Add(company);
        db.StoreGroups.Add(group);
        db.Stores.Add(store);

        //Adding Employees
        var kpi = new KeyPersonalInfo
        {
            StoreManagerName = "Alok Kumar",
            StoreManagerPhoneNumber = "1234567890",
            StoreManagerEmail = "alok@aadwikafashion.in",

            AccountantEmail = "accountant@aadwikafashion.in",
            AccountantName = "Accountant Manger",
            AccountantPhoneNumber = "9876543210"
        };

        count = 3;
        saved += db.SaveChanges();
        if (saved >= 3)
        {
            Message += "Store, StoreGroup and  Company is added and Database is created. Database Created Successfully.";
            Message += SeedDatabase(store, company, group, kpi, "aadwikafashion.in");
        }
        else
        {
            Message += "Store, StoreGroup and  Company is not added. Database is not created.";
        }
        _ = Task.Run(async () => { await Notify.DisplayNotificationAsync(Message, isLong: false, speak: true); });
        //await AksNotify.NotifyAsync(Message, isLong: true, speak: false, toast: false);
        return Message;
    }

    private string AddingOwners(Store store)
    {
        var owner = new Employee
        {
            Aadhar = "NA",
            Category = EmployeeCategory.Owner,
            CompanyId = store.CompanyId,
            FirstName = store.Company?.ContactPerson.Split(" ")[0]??"",
            Id = Guid.NewGuid(),
            LastName = store.Company?.ContactPerson.Split(" ")[1]??"",
            Mobile = store.Company?.ContactMobile??"",
            DateOfBirth = DateTime.Now.Date.AddYears(-25),
            EmpId = 1,
            Gender = Gender.Male,
            JoiningDate = (DateTime)(store.Company?.StartDate)  ,
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
            Email = store.Company.Email,
        };
        if (owner.FirstName == "Shalini")
        {
            owner.Title = "Mrs.";
            owner.Gender = Gender.Female;
        }
        db.Employees.Add(owner);

        count++; // Increment count for owner added
        saved += db.SaveChanges();
        if (saved >= count)
        {
            return "Owner Added Successfully";
        }
        else
        {
            return "Owner Not Added";
        }
    }

    private string AddingEmployees(Store store, KeyPersonalInfo kpi)
    {
        var sm = new Employee
        {
            Aadhar = "NA",
            Category = EmployeeCategory.StoreManager,
            CompanyId = store.CompanyId,
            FirstName = kpi.StoreManagerName.Split(" ")[0],
            Id = Guid.NewGuid(),
            LastName = kpi.StoreManagerName.Split(" ")[1],
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

        var accountant = new Employee
        {
            Aadhar = "NA",
            Category = EmployeeCategory.Accounts,
            CompanyId = store.CompanyId,
            FirstName = kpi.AccountantName.Split(" ")[0],
            Id = Guid.NewGuid(),
            LastName = kpi.AccountantName.Split(" ")[1],
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
        count += 3; // Increment count for each employee added
        saved += db.SaveChanges();
        if (saved >= count)
        {
            return "Employees Added Successfully";
        }
        else
        {
            return "Employees Not Added";
        }
    }

    private string InitialUsers(Store store, string baseCompanyUrl)
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
            RemoteUserId = null, PinHash="1234"
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

        db.AppUsers.Add(admin);
        db.AppUsers.Add(owner);
        db.AppUsers.Add(cashier);

        appDb.AppUsers.Add(admin);
        appDb.AppUsers.Add(owner);
        appDb.AppUsers.Add(cashier);
        count += 3; // Increment count for each user added
        try
        {
            appDb.SaveChanges();
            saved += db.SaveChanges();
            if (saved >= count)
            {
                return "Saved user";
            }
            else
            {
                return " Not all users were added successfully";
            }
        }
        catch (Exception)
        {
            return "error: user not added";
        }
    }

    private string AddTaxes(Store store)
    {
        var tax1 = new Tax
        {
            Id = Guid.NewGuid(),
            Name = "GST 5%",
            CompositeRate = 5,
            TaxType = TaxType.GST,



            Deleted = false,
            Synced = false,

        };
        var tax2 = new Tax
        {
            Id = Guid.NewGuid(),
            Name = "GST 12%",
            CompositeRate = 12,
            TaxType = TaxType.GST
        };

        var tax3 = new Tax
        {
            Name = "GST 18%",
            CompositeRate = 18,
            TaxType = TaxType.GST,
            Id = Guid.NewGuid(),
        };
        var tax11 = new Tax
        {
            Id = Guid.NewGuid(),
            Name = "IGST 5%",
            CompositeRate = 5,
            TaxType = TaxType.IGST
        };
        var tax21 = new Tax
        {
            Id = Guid.NewGuid(),
            Name = "IGST 12%",
            CompositeRate = 12,
            TaxType = TaxType.IGST
        };

        var tax31 = new Tax
        {
            Name = "IGST 18%",
            CompositeRate = 18,
            TaxType = TaxType.IGST,
            Id = Guid.NewGuid(),
        };


        var taxcgst = new Tax
        {
            Id = Guid.NewGuid(),
            Name = "CGST 2.5%",
            CompositeRate = 5,
            TaxType = TaxType.GST,
        };
        var taxsgst = new Tax
        {
            Id = Guid.NewGuid(),
            Name = "SGST 2,5%",
            CompositeRate = 12,
            TaxType = TaxType.GST
        };


        db.Taxes.Add(tax1);
        db.Taxes.Add(tax11);
        db.Taxes.Add(tax31);
        db.Taxes.Add(tax2);
        db.Taxes.Add(tax3);
        db.Taxes.Add(tax21);
        db.Taxes.Add(taxcgst);
        db.Taxes.Add(taxsgst);

        count += 8; // Increment count for each tax added
        saved += db.SaveChanges();
        if (saved >= count)
        {
            return "Taxes Added Successfully";
        }
        else
        {
            return "Taxes Not Added";
        }
    }

    private string AddLedgerAndGroups(Store store, Company company)
    {
        string message = "Adding Ledger Groups and Ledgers for " + store.Name + ", " + company.Name + "\n";
        //Ledger Groups
        var snackgroups = new LedgerGroup()
        {
            Category = LedgerCategory.Expenses,
            CompanyId = company.Id,
            Deleted = false,
            Id = Guid.NewGuid(),
            Name = "Snacks & Refeshments",
            Remarks = "Store Snacks & Refeshments Expenses",
            Synced = false
        };
        var storeexpenses = new LedgerGroup()
        {
            Category = LedgerCategory.Expenses,
            CompanyId = company.Id,
            Deleted = false,
            Id = Guid.NewGuid(),
            Name = "Store Expenses",
            Remarks = "Store Expenses",
            Synced = false
        };
        var directExpenses = new LedgerGroup()
        {
            Category = LedgerCategory.Expenses,
            CompanyId = company.Id,
            Deleted = false,
            Id = Guid.NewGuid(),
            Name = "Direct Expenses",
            Remarks = "Store Direct Expenses",
            Synced = false
        };
        var indirectExpenses = new LedgerGroup()
        {
            Category = LedgerCategory.Expenses,
            CompanyId = company.Id,
            Deleted = false,
            Id = Guid.NewGuid(),
            Name = "Indirect Expenses",
            Remarks = "Store Indirect Expenses",
            Synced = false
        };
        var pettyExpenses = new LedgerGroup()
        {
            Category = LedgerCategory.Expenses,
            CompanyId = company.Id,
            Deleted = false,
            Id = Guid.NewGuid(),
            Name = "Petty Expenses",
            Remarks = "Store Petty Expenses",
            Synced = false
        };
        var noGroup = new LedgerGroup()
        {
            Category = LedgerCategory.UnCategory,
            CompanyId = company.Id,
            Deleted = false,
            Id = Guid.NewGuid(),
            Name = "No Group",
            Remarks = "NO GROUP & Party, Group for UnCategory Ledger",
            Synced = false
        };

        var sales = new LedgerGroup()
        {
            Category = LedgerCategory.Income,
            CompanyId = company.Id,
            Deleted = false,
            Id = Guid.NewGuid(),
            Name = "Sales",
            Remarks = "Store Sales",
            Synced = false
        };
        var purchases = new LedgerGroup()
        {
            Category = LedgerCategory.Purchase,
            CompanyId = company.Id,
            Deleted = false,
            Id = Guid.NewGuid(),
            Name = "Purchases",
            Remarks = "Store Purchase",
            Synced = false
        };
        var cashs = new LedgerGroup()
        {
            Category = LedgerCategory.Assets,
            CompanyId = company.Id,
            Deleted = false,
            Id = Guid.NewGuid(),
            Name = "Cash",
            Remarks = "Store Cash(s)",
            Synced = false
        };

        var banks = new LedgerGroup()
        {
            Category = LedgerCategory.Assets,
            CompanyId = company.Id,
            Deleted = false,
            Id = Guid.NewGuid(),
            Name = "Banks",
            Remarks = "Store Bank",
            Synced = false
        };
        var capitalAccounts = new LedgerGroup()
        {
            Id = Guid.NewGuid(),
            Name = "Capital Accounts",
            Category = LedgerCategory.Assets,
            CompanyId = company.Id,
            Deleted = false,
            Synced = false,
            Remarks = "Store Capital Account"
        };

        var loansAndAdvances = new LedgerGroup()
        {
            Id = Guid.NewGuid(),
            Name = "Loans and Adavances",
            Category = LedgerCategory.Assets,
            CompanyId = company.Id,
            Deleted = false,
            Synced = false,
            Remarks = "Store Loan Account"
        };

        var vendors = new LedgerGroup()
        {
            Id = Guid.NewGuid(),
            Name = "Vendors",
            Category = LedgerCategory.Vendor,
            CompanyId = company.Id,
            Deleted = false,
            Synced = false,
            Remarks = "Store Vendor Account"
        };

        var customers = new LedgerGroup()
        {
            Id = Guid.NewGuid(),
            Name = "Customers",
            Category = LedgerCategory.Customer,
            CompanyId = company.Id,
            Deleted = false,
            Synced = false,
            Remarks = "Store Customer Account"
        };
        var employees = new LedgerGroup()
        {
            Id = Guid.NewGuid(),
            Name = "Employees",
            Category = LedgerCategory.Employees,
            CompanyId = company.Id,
            Deleted = false,
            Synced = false,
            Remarks = "Store Employee Account"
        };
        var stocks = new LedgerGroup()
        {
            Id = Guid.NewGuid(),
            Name = "Stock",
            Category = LedgerCategory.Stock,
            CompanyId = company.Id,
            Deleted = false,
            Synced = false,
            Remarks = "Store Stock Account"
        };
        var debitors = new LedgerGroup()
        {
            Id = Guid.NewGuid(),
            Name = "Debitors",
            Category = LedgerCategory.Debitor,
            CompanyId = company.Id,
            Deleted = false,
            Synced = false,
            Remarks = "Store Stock Account"
        };
        var creditors = new LedgerGroup()
        {
            Id = Guid.NewGuid(),
            Name = "Creditors",
            Category = LedgerCategory.Creditor,
            CompanyId = company.Id,
            Deleted = false,
            Synced = false,
            Remarks = "Store Stock Account"
        };
        db.LedgerGroups.Add(cashs);
        db.LedgerGroups.Add(stocks);
        db.LedgerGroups.Add(indirectExpenses);
        db.LedgerGroups.Add(directExpenses);
        db.LedgerGroups.Add(pettyExpenses);
        db.LedgerGroups.Add(debitors);
        db.LedgerGroups.Add(creditors);

        db.LedgerGroups.Add(noGroup);
        db.LedgerGroups.Add(sales);
        db.LedgerGroups.Add(purchases);

        db.LedgerGroups.Add(banks);
        db.LedgerGroups.Add(loansAndAdvances);
        db.LedgerGroups.Add(capitalAccounts);
        db.LedgerGroups.Add(vendors);
        db.LedgerGroups.Add(customers);
        db.LedgerGroups.Add(employees);
        db.LedgerGroups.Add(snackgroups);
        db.LedgerGroups.Add(storeexpenses);

        count += 14; // Increment count for each ledger group added
        saved += db.SaveChanges();
        if (saved >= count)
        {
            message += "Ledger Groups Added Successfully\n";
        }
        else
        {
            message += "Ledger Groups Not Added\n";
        }

        //Ledgers

        var dan = new Ledger()
        {
            OpenningBalance = 0,
            LedgerGroupId = pettyExpenses.Id,
            IsParty = false,
            CreatedBy = "AutoAdmin",
            Deleted = false,
            Synced = false,
            CompanyId = company.Id,
            Name = "Dan",
            Id = Guid.NewGuid(),
            OpenningDate = company.StartDate,
            LedgerType = LedgerType.Expenses,
            CreatedAt = DateTime.UtcNow
        };
        var snacks = new Ledger()
        {
            OpenningBalance = 0,
            LedgerGroupId = snackgroups.Id,
            IsParty = false,
            CreatedBy = "AutoAdmin",
            Deleted = false,
            Synced = false,
            CreatedAt = DateTime.UtcNow,

            CompanyId = company.Id,

            Name = "Snacks & Tea",
            Id = Guid.NewGuid(),
            OpenningDate = company.StartDate,
            LedgerType = LedgerType.Expenses,
        };
        var electricity = new Ledger()
        {
            OpenningBalance = 0,
            LedgerGroupId = storeexpenses.Id,
            CreatedAt = DateTime.UtcNow,
            IsParty = false,
            CreatedBy = "AutoAdmin",
            Deleted = false,
            Synced = false,

            CompanyId = company.Id,

            Name = "Electricity",
            Id = Guid.NewGuid(),
            OpenningDate = company.StartDate,
            LedgerType = LedgerType.Expenses,
        };

        var water = new Ledger()
        {
            OpenningBalance = 0,
            LedgerGroupId = snackgroups.Id,
            IsParty = false,
            CreatedBy = "AutoAdmin",
            CreatedAt = DateTime.UtcNow,
            Deleted = false,
            Synced = false,

            CompanyId = company.Id,

            Name = "Water",
            Id = Guid.NewGuid(),
            OpenningDate = company.StartDate,
            LedgerType = LedgerType.Expenses,
        };

        var printing = new Ledger()
        {
            OpenningBalance = 0,
            LedgerGroupId = storeexpenses.Id,
            CreatedAt = DateTime.UtcNow,
            IsParty = false,
            CreatedBy = "AutoAdmin",
            Deleted = false,
            Synced = false,

            CompanyId = company.Id,
            Name = "Printing & Stationery",
            Id = Guid.NewGuid(),
            OpenningDate = company.StartDate,
            LedgerType = LedgerType.Expenses,
        };

        var transport = new Ledger()
        {
            OpenningBalance = 0,
            LedgerGroupId = directExpenses.Id,
            IsParty = false,
            CreatedBy = "AutoAdmin",
            CreatedAt = DateTime.UtcNow,
            Deleted = false,
            Synced = false,

            CompanyId = company.Id,

            Name = "Transports & Freight Charges",
            Id = Guid.NewGuid(),
            OpenningDate = company.StartDate,
            LedgerType = LedgerType.Expenses,
        };

        var misc = new Ledger()
        {
            OpenningBalance = 0,
            LedgerGroupId = storeexpenses.Id,
            IsParty = false,
            CreatedBy = "AutoAdmin",
            CreatedAt = DateTime.UtcNow,
            Deleted = false,
            Synced = false,

            CompanyId = company.Id,

            Name = "Miscellaneous",
            Id = Guid.NewGuid(),
            OpenningDate = company.StartDate,
            LedgerType = LedgerType.Expenses,
        };

        var noparty = new Ledger()
        {
            OpenningBalance = 0,
            LedgerGroupId = noGroup.Id,
            IsParty = false,
            CreatedBy = "AutoAdmin",
            CreatedAt = DateTime.UtcNow,
            Deleted = false,
            Synced = false,

            CompanyId = company.Id,

            Name = "No Party",
            Id = Guid.NewGuid(),
            OpenningDate = company.StartDate,
            LedgerType = LedgerType.IndirectExpenses,
        };
        var cashinhand = new Ledger()
        {
            OpenningBalance = 0,
            LedgerGroupId = cashs.Id,

            Deleted = false,
            Synced = false,

            CompanyId = company.Id,

            Name = "Cash In Hand",
            Id = Guid.NewGuid(),
            OpenningDate = company.StartDate,
            LedgerType = LedgerType.Cash,
        };

        var salaryPayables = new Ledger()
        {
            OpenningBalance = 0,
            LedgerGroupId = directExpenses.Id,
            IsParty = false,
            CreatedBy = "AutoAdmin",
            CreatedAt = DateTime.UtcNow,
            Deleted = false,
            Synced = false,
            CompanyId = company.Id,
            Name = "Salary Payables",
            Id = Guid.NewGuid(),
            OpenningDate = company.StartDate,
            LedgerType = LedgerType.Expenses,
        };

        var internet = new Ledger()
        {
            OpenningBalance = 0,
            LedgerGroupId = storeexpenses.Id,
            IsParty = false,
            CreatedBy = "AutoAdmin",
            CreatedAt = DateTime.UtcNow,
            Deleted = false,
            Synced = false,
            CompanyId = company.Id,
            Name = "Internet & Mobile Bills",
            Id = Guid.NewGuid(),
            OpenningDate = company.StartDate,
            LedgerType = LedgerType.Expenses,
        };

        var storeMaintanance = new Ledger()
        {
            OpenningBalance = 0,
            LedgerGroupId = storeexpenses.Id,
            IsParty = false,
            CreatedBy = "AutoAdmin",
            CreatedAt = DateTime.UtcNow,
            Deleted = false,
            Synced = false,
            CompanyId = company.Id,
            Name = "Store Maintenance",
            Id = Guid.NewGuid(),
            OpenningDate = company.StartDate,
            LedgerType = LedgerType.Expenses,
        };

        var storeSupplies = new Ledger()
        {
            OpenningBalance = 0,
            LedgerGroupId = storeexpenses.Id,
            IsParty = false,
            CreatedBy = "AutoAdmin",
            CreatedAt = DateTime.UtcNow,
            Deleted = false,
            Synced = false,
            CompanyId = company.Id,
            Name = "Store Supplies",
            Id = Guid.NewGuid(),
            OpenningDate = company.StartDate,
            LedgerType = LedgerType.Expenses,
        };

        var pettyCash = new Ledger()
        {
            OpenningBalance = 0,
            LedgerGroupId = pettyExpenses.Id,
            IsParty = false,
            CreatedBy = "AutoAdmin",
            CreatedAt = DateTime.UtcNow,
            Deleted = false,
            Synced = false,
            CompanyId = company.Id,
            Name = "Petty Cash Expenses",
            Id = Guid.NewGuid(),
            OpenningDate = company.StartDate,
            LedgerType = LedgerType.Expenses,
        };

        db.Ledgers.Add(pettyCash);
        db.Ledgers.Add(storeMaintanance);
        db.Ledgers.Add(storeSupplies);
        db.Ledgers.Add(internet);
        db.Ledgers.Add(salaryPayables);

        db.Ledgers.Add(transport);
        db.Ledgers.Add(misc);
        db.Ledgers.Add(snacks);
        db.Ledgers.Add(dan);
        db.Ledgers.Add(electricity);
        db.Ledgers.Add(water);
        db.Ledgers.Add(printing);

        db.Ledgers.Add(cashinhand);
        db.Ledgers.Add(noparty);

        count += 14; // Increment count for each ledger added
        saved += db.SaveChanges();
        if (saved >= count)
        {
            message += "Ledgers Added Successfully\n";
        }
        else
        {
            message += "Ledgers Not Added\n";
        }

        if (company.Name == "Aadwika Fashion")
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
                BankId = db.Banks.FirstOrDefault(static x => x.Name == "State Bank Of India").Id,
                CompanyId = company.Id,

                AccountHolderName = company.Name,
                AccountNumber = "SBI Current Account",
                AccountType = AccountType.Current,
            };

            db.Ledgers.Add(sbicurledger);
            db.BankAccounts.Add(sbicc);
            count += 2; // Increment count for each bank account and ledger added
            saved += db.SaveChanges();
            if (saved >= count)
            {
                message += "Bank Account and Ledger Added Successfully for Aadwika Fashion.";
            }
            else
            {
                message += "Bank Account and Ledger Not Added Successfully for Aadwika fashion.";
            }
        }
        return message;
    }

    private string AddBanks()
    {
        //Banking

        var sbi = new Bank { Id = Guid.NewGuid(), Name = "State Bank Of India" };
        var icici = new Bank { Id = Guid.NewGuid(), Name = "ICICI Bank" };

        var hdfc = new Bank { Name = "HDFC Bank", Id = Guid.NewGuid() };
        var bob = new Bank { Name = "Bank of Baroda", Id = Guid.NewGuid() };
        var kotak = new Bank { Name = "Kotka Bank", Id = Guid.NewGuid() };
        var pnb = new Bank { Name = "Punjab National Bank", Id = Guid.NewGuid() };

        db.Banks.Add(sbi);
        db.Banks.Add(kotak);
        db.Banks.Add(pnb);
        db.Banks.Add(icici);
        db.Banks.Add(hdfc);
        db.Banks.Add(bob);
        count += 6; // Increment count for each bank added
        saved += db.SaveChanges();
        if (saved >= count)
        {
            return "Banking Data Added Successfully.";
        }
        else
        {
            return "Banking Data Not Added Successfully.";
        }
    }

    private string AddTransactions(Company company)
    {
        //Transcations
        var trans = new Transaction
        {
            Deleted = false,
            Id = Guid.NewGuid(),
            CompanyId = company.Id,
            Synced = false,
            Name = "Petty Cash Expenses"
        };
        var trans1 = new Transaction
        {
            Deleted = false,
            Id = Guid.NewGuid(),
            CompanyId = company.Id,
            Synced = false,
            Name = "Home Expenses"
        };
        var trans2 = new Transaction
        {
            Deleted = false,
            Id = Guid.NewGuid(),
            CompanyId = company.Id,
            Synced = false,
            Name = "Store Expenses"
        };
        var trans3 = new Transaction
        {
            Deleted = false,
            Id = Guid.NewGuid(),
            CompanyId = company.Id,
            Synced = false,
            Name = "Dan & Donations"
        };

        var trans4 = new Transaction
        {
            Deleted = false,
            Id = Guid.NewGuid(),
            CompanyId = company.Id,
            Synced = false,
            Name = "Snacks & Breakfast Expenses"
        };

        var trans5 = new Transaction
        {
            Deleted = false,
            Id = Guid.NewGuid(),
            CompanyId = company.Id,
            Synced = false,
            Name = "Cash In"
        };

        var trans6 = new Transaction
        {
            Deleted = false,
            Id = Guid.NewGuid(),
            CompanyId = company.Id,
            Synced = false,
            Name = "Cash Out"
        };

        db.Transactions.Add(trans6);
        db.Transactions.Add(trans5);
        db.Transactions.Add(trans4);
        db.Transactions.Add(trans3);
        db.Transactions.Add(trans2);
        db.Transactions.Add(trans1);
        db.Transactions.Add(trans);

        count += 7; // Increment count for each transaction added
        saved += db.SaveChanges();
        if (saved >= count)
        {
            return "Transactions Added Successfully.";
        }
        else
        {
            return "Transactions Not Added Successfully.";
        }
    }
}