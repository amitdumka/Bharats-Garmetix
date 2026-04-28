//Garmetix - A.NET MAUI Application for Garment and Textile Management

using Garmetix.Core.Models.Stores;
using Garmetix.Models;
using Garmetix.Models.Auth;
using Microsoft.EntityFrameworkCore;
using SQLite;
using System.ComponentModel.DataAnnotations;

namespace Garmetix.Databases;

public class Notification
{
    [Key, AutoIncrement]
    public int Id { get; set; }

    public string Title { get; set; }="";
    public string Message { get; set; } = "";
    public DateTime Timestamp { get; set; }
    public bool IsRead { get; set; }
}

public class ApplicationDatabaseContext : DbContext
{
    private string DatabasePath { get; set; } = Constants.DatabasePath;
    private static ApplicationDatabaseContext? _instance;
    public static ApplicationDatabaseContext? Instance { get{ return _instance; } }

    public ApplicationDatabaseContext()
    {
        try
        {
            DatabasePath = Constants.DatabasePath;
            Console.WriteLine("DBPath:" + DatabasePath);
            SQLitePCL.Batteries_V2.Init();

            var x = Database.EnsureCreated();
            if (x)
            {
                Console.WriteLine("Database created successfully.");
            }
            else
            {
                Console.WriteLine("Database already exists.");
            }
            _instance = this;
        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine(e.Message);
        }
    }

    public ApplicationDatabaseContext(DbContextOptions<ApplicationDatabaseContext> options)
    {
        try
        {
            DatabasePath = Constants.DatabasePath;
            Console.WriteLine("DBPath:" + DatabasePath);
            SQLitePCL.Batteries_V2.Init();
            Database.GetAppliedMigrationsAsync();

            var x = Database.EnsureCreated();
            _instance = this;
        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine(e.Message);
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        try
        {
            DatabasePath = Constants.DatabasePath;
            optionsBuilder.UseSqlite($"Filename={DatabasePath}");

            base.OnConfiguring(optionsBuilder);
        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine(e.Message);
        }
    }

    public void ApplyMigrations(ApplicationDatabaseContext context)
    {
        try
        {
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }
        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine(e.Message);
        }
    }

    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<StoreGroup> StoreGroups { get; set; }
    public DbSet<Store> Stores { get; set; }

    public DbSet<AppUser> AppUsers { get; set; }

    //Enabling TODO Items
    public DbSet<Category> Categories { get; set; }

    public DbSet<Project> Projects { get; set; } //Enabling TODO Items>
    public DbSet<ProjectTask> ProjectTasks { get; set; } //Enabling TODO Items>
    public DbSet<ProjectsTags> ProjectsTags { get; set; } //Enabling TODO Items>
    public DbSet<Tag> Tags { get; set; } //Enabling TODO Items>
}