
using Garmetix.Databases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
// Add using statements for Newtonsoft.Json
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace Garmetix.DataServices.DatabaseBackupServices;




/// <summary>
/// A service to handle backing up and restoring all data from an Entity Framework Core DbContext.
/// Uses Newtonsoft.Json to robustly handle complex object relationships and cycles.
/// </summary>
public class JsonDatabaseBackupService
{
    private readonly DbContext _context;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the DatabaseBackupService.
    /// </summary>
    /// <param name="serviceProvider">The service provider to resolve DbContext instances.</param>
    public JsonDatabaseBackupService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        // Resolve a new DbContext instance to avoid threading issues.
        _context = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<DatabaseContext>();
    }

    /// <summary>
    /// Backs up all data from all tables in the DbContext to a single JSON file using Newtonsoft.Json.
    /// </summary>
    /// <param name="backupFilePath">The path where the backup JSON file will be saved.</param>
    /// <returns>A task that represents the asynchronous backup operation.</returns>
    public async Task BackupAllDataAsync(string backupFilePath)
    {
        try
        {
            var backupData = new Dictionary<string, object>();
            var dbSetProperties = _context.GetType().GetProperties()
                .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));

            foreach (var dbSetProperty in dbSetProperties)
            {
                var entityType = dbSetProperty.PropertyType.GetGenericArguments()[0];
                var tableName = _context.Model.FindEntityType(entityType)?.GetTableName();
                if (tableName != null)
                {
                    var dbSet =  (IQueryable<object>)dbSetProperty.GetValue(_context);
                    var tableData = await dbSet.ToListAsync();
                    backupData[tableName] = tableData;
                }
            }

            // Configure Newtonsoft.Json to preserve references, which handles cycles and duplicates.
            var settings = new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                Formatting = Formatting.Indented
            };

            string json = JsonConvert.SerializeObject(backupData, settings);
            await File.WriteAllTextAsync(backupFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred during backup: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Restores data from a JSON backup file using Newtonsoft.Json.
    /// This method now correctly handles object references and cycles.
    /// </summary>
    /// <param name="backupFilePath">The path to the backup JSON file.</param>
    /// <returns>A task that represents the asynchronous restore operation.</returns>
    public async Task RestoreDataAsync(string backupFilePath)
    {
        if (!File.Exists(backupFilePath))
        {
            Console.WriteLine("Backup file not found.");
            return;
        }

        try
        {
            string json = await File.ReadAllTextAsync(backupFilePath);

            // Use the same settings for deserialization to correctly interpret the references.
            var settings = new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };

            // Deserialize to JToken to handle the wrapper object created by PreserveReferencesHandling.
            var backupData = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(json, settings);

            if (backupData == null)
            {
                Console.WriteLine("Failed to deserialize backup data.");
                return;
            }

            var sortedEntityTypes = GetSortedEntityTypes();

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                // For SQLite:
                await _context.Database.ExecuteSqlRawAsync("PRAGMA foreign_keys = OFF;");

                // Delete data in reverse topological order to avoid FK violations.
                foreach (var entityType in sortedEntityTypes.AsEnumerable().Reverse())
                {
                    var tableName = entityType.GetTableName();
                    if (!string.IsNullOrEmpty(tableName))
                    {
                        await _context.Database.ExecuteSqlRawAsync($"DELETE FROM [{tableName}]");
                    }
                }

                // Insert data in topological order.
                foreach (var entityType in sortedEntityTypes)
                {
                    var tableName = entityType.GetTableName();
                    if (tableName != null && backupData.TryGetValue(tableName, out var tableDataToken) && tableDataToken.Type != JTokenType.Null)
                    {
                        var entityList = (IEnumerable<object>)tableDataToken.ToObject(typeof(List<>).MakeGenericType(entityType.ClrType), JsonSerializer.Create(settings));

                        if (entityList != null)
                        {
                            // *** FIX: Check for null primary keys before adding to context ***
                            var primaryKeyProperty = entityType.FindPrimaryKey()?.Properties.FirstOrDefault()?.PropertyInfo;

                            if (primaryKeyProperty == null)
                            {
                                // This entity type has no primary key defined in the model. Add all its data.
                                _context.AddRange(entityList);
                            }
                            else
                            {
                                var validEntities = new List<object>();
                                foreach (var entity in entityList)
                                {
                                    if (entity != null && primaryKeyProperty.GetValue(entity) != null)
                                    {
                                        validEntities.Add(entity);
                                    }
                                    else
                                    {
                                        // Log a warning for the problematic data ("blank row")
                                        Console.WriteLine($"WARNING: Skipping entity of type '{entityType.DisplayName()}' because its Primary Key is null.");
                                    }
                                }
                                if (validEntities.Any())
                                {
                                    _context.AddRange(validEntities);
                                }
                            }
                        }
                    }
                }

                // EF Core will track all the added entities and correctly wire up the relationships in memory
                // before sending the insert commands to the database.
                await _context.SaveChangesAsync();

                // For SQLite:
                await _context.Database.ExecuteSqlRawAsync("PRAGMA foreign_keys = ON;");

                await transaction.CommitAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred during restore: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Sorts entity types based on their dependencies (foreign keys).
    /// This ensures that principal entities are processed before dependent entities.
    /// </summary>
    /// <returns>A list of entity types sorted in an order safe for insertion.</returns>
    private List<IEntityType> GetSortedEntityTypes()
    {
        var entityTypes = _context.Model.GetEntityTypes().ToList();
        var dependencyGraph = new Dictionary<IEntityType, List<IEntityType>>();
        var inDegree = new Dictionary<IEntityType, int>();

        foreach (var et in entityTypes)
        {
            dependencyGraph[et] = new List<IEntityType>();
            inDegree[et] = 0;
        }

        foreach (var et in entityTypes)
        {
            foreach (var fk in et.GetForeignKeys())
            {
                var principalEntityType = fk.PrincipalEntityType;
                if (principalEntityType != et)
                {
                    dependencyGraph[principalEntityType].Add(et);
                    inDegree[et]++;
                }
            }
        }

        var queue = new Queue<IEntityType>(entityTypes.Where(et => inDegree[et] == 0));
        var sortedList = new List<IEntityType>();

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            sortedList.Add(current);

            foreach (var dependent in dependencyGraph[current])
            {
                inDegree[dependent]--;
                if (inDegree[dependent] == 0)
                {
                    queue.Enqueue(dependent);
                }
            }
        }

        if (sortedList.Count != entityTypes.Count)
        {
            Console.WriteLine("Warning: A circular dependency may exist. Disabling FK constraints during restore is critical.");
            // Add remaining types that are part of a cycle. The FK disable/enable handles this.
            var remaining = entityTypes.Except(sortedList);
            sortedList.AddRange(remaining);
        }

        return sortedList;
    }
}

// <summary>
// A service to handle backing up and restoring all data from an Entity Framework Core DbContext.
// Uses Newtonsoft.Json to robustly handle complex object relationships and cycles.
// </summary>

//public class JsonDatabaseBackupService
//{
//    private readonly DbContext _context;
//    private readonly IServiceProvider _serviceProvider;

//    /// <summary>
//    /// Initializes a new instance of the DatabaseBackupService.
//    /// </summary>
//    /// <param name="serviceProvider">The service provider to resolve DbContext instances.</param>
//    public JsonDatabaseBackupService(IServiceProvider serviceProvider)
//    {
//        _serviceProvider = serviceProvider;
//        // Resolve a new DbContext instance to avoid threading issues.
//        _context = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<DatabaseContext>();
//    }

//    /// <summary>
//    /// Backs up all data from all tables in the DbContext to a single JSON file using Newtonsoft.Json.
//    /// </summary>
//    /// <param name="backupFilePath">The path where the backup JSON file will be saved.</param>
//    /// <returns>A task that represents the asynchronous backup operation.</returns>
//    public async Task BackupAllDataAsync(string backupFilePath)
//    {
//        try
//        {
//            var backupData = new Dictionary<string, object>();
//            var dbSetProperties = _context.GetType().GetProperties()
//                .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));

//            foreach (var dbSetProperty in dbSetProperties)
//            {
//                var entityType = dbSetProperty.PropertyType.GetGenericArguments()[0];
//                var tableName = _context.Model.FindEntityType(entityType)?.GetTableName();
//                if (tableName != null)
//                {
//                    var dbSet = (IQueryable<object>)dbSetProperty.GetValue(_context);
//                    var tableData = await dbSet.ToListAsync();
//                    backupData[tableName] = tableData;
//                }
//            }

//            // Configure Newtonsoft.Json to preserve references, which handles cycles and duplicates.
//            var settings = new JsonSerializerSettings
//            {
//                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
//                Formatting = Formatting.Indented
//            };

//            string json = JsonConvert.SerializeObject(backupData, settings);
//            await File.WriteAllTextAsync(backupFilePath, json);
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"An error occurred during backup: {ex.Message}");
//            throw;
//        }
//    }

//    /// <summary>
//    /// Restores data from a JSON backup file using Newtonsoft.Json.
//    /// This method now correctly handles object references and cycles.
//    /// </summary>
//    /// <param name="backupFilePath">The path to the backup JSON file.</param>
//    /// <returns>A task that represents the asynchronous restore operation.</returns>
//    /// <summary>
//    /// Restores data from a JSON backup file using Newtonsoft.Json.
//    /// This method now correctly handles object references and cycles.
//    /// </summary>
//    /// <param name="backupFilePath">The path to the backup JSON file.</param>
//    /// <returns>A task that represents the asynchronous restore operation.</returns>
//    public async Task RestoreDataAsync(string backupFilePath)
//    {
//        if (!File.Exists(backupFilePath))
//        {
//            Console.WriteLine("Backup file not found.");
//            return;
//        }

//        try
//        {
//            string json = await File.ReadAllTextAsync(backupFilePath);

//            // Use the same settings for deserialization to correctly interpret the references.
//            var settings = new JsonSerializerSettings
//            {
//                PreserveReferencesHandling = PreserveReferencesHandling.Objects
//            };

//            // *** FIX: Deserialize to JToken instead of JArray ***
//            // This allows Newtonsoft to handle the wrapper object ({ "$id": ..., "$values": [...] })
//            // created by PreserveReferencesHandling.Objects.
//            var backupData = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(json, settings);

//            if (backupData == null)
//            {
//                Console.WriteLine("Failed to deserialize backup data.");
//                return;
//            }

//            var sortedEntityTypes = GetSortedEntityTypes();

//            using (var transaction = await _context.Database.BeginTransactionAsync())
//            {
//                // For SQLite:
//                await _context.Database.ExecuteSqlRawAsync("PRAGMA foreign_keys = OFF;");

//                // Delete data in reverse topological order to avoid FK violations.
//                foreach (var entityType in sortedEntityTypes.AsEnumerable().Reverse())
//                {
//                    var tableName = entityType.GetTableName();
//                    if (!string.IsNullOrEmpty(tableName))
//                    {
//                        await _context.Database.ExecuteSqlRawAsync($"DELETE FROM [{tableName}]");
//                    }
//                }

//                // Insert data in topological order.
//                foreach (var entityType in sortedEntityTypes)
//                {
//                    var tableName = entityType.GetTableName();
//                    if (tableName != null && backupData.TryGetValue(tableName, out var tableDataToken))
//                    {
//                        // Now, convert the JToken into a list of the specific entity type.
//                        // The ToObject method is smart enough to read the "$values" from the JToken
//                        // and correctly deserialize the list of entities.
//                        var entityList = tableDataToken.ToObject(typeof(List<>).MakeGenericType(entityType.ClrType), JsonSerializer.Create(settings));

//                        if (entityList != null)
//                        {
//                            _context.AddRange((IEnumerable<object>)entityList);
//                        }
//                    }
//                }

//                // EF Core will track all the added entities and correctly wire up the relationships in memory
//                // before sending the insert commands to the database.
//                await _context.SaveChangesAsync();

//                // For SQLite:
//                await _context.Database.ExecuteSqlRawAsync("PRAGMA foreign_keys = ON;");

//                await transaction.CommitAsync();
//            }
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"An error occurred during restore: {ex.Message}");
//            throw;
//        }
//    }

//    /// <summary>
//    /// Sorts entity types based on their dependencies (foreign keys).
//    /// This ensures that principal entities are processed before dependent entities.
//    /// </summary>
//    /// <returns>A list of entity types sorted in an order safe for insertion.</returns>
//    private List<IEntityType> GetSortedEntityTypes()
//    {
//        var entityTypes = _context.Model.GetEntityTypes().ToList();
//        var dependencyGraph = new Dictionary<IEntityType, List<IEntityType>>();
//        var inDegree = new Dictionary<IEntityType, int>();

//        foreach (var et in entityTypes)
//        {
//            dependencyGraph[et] = new List<IEntityType>();
//            inDegree[et] = 0;
//        }

//        foreach (var et in entityTypes)
//        {
//            foreach (var fk in et.GetForeignKeys())
//            {
//                var principalEntityType = fk.PrincipalEntityType;
//                if (principalEntityType != et)
//                {
//                    dependencyGraph[principalEntityType].Add(et);
//                    inDegree[et]++;
//                }
//            }
//        }

//        var queue = new Queue<IEntityType>(entityTypes.Where(et => inDegree[et] == 0));
//        var sortedList = new List<IEntityType>();

//        while (queue.Count > 0)
//        {
//            var current = queue.Dequeue();
//            sortedList.Add(current);

//            foreach (var dependent in dependencyGraph[current])
//            {
//                inDegree[dependent]--;
//                if (inDegree[dependent] == 0)
//                {
//                    queue.Enqueue(dependent);
//                }
//            }
//        }

//        if (sortedList.Count != entityTypes.Count)
//        {
//            Console.WriteLine("Warning: A circular dependency may exist. Disabling FK constraints during restore is critical.");
//            // Add remaining types that are part of a cycle. The FK disable/enable handles this.
//            var remaining = entityTypes.Except(sortedList);
//            sortedList.AddRange(remaining);
//        }

//        return sortedList;
//    }
//}

// <summary>
// A service to handle backing up and restoring all data from an Entity Framework Core DbContext.
// </summary>
//public class JsonDatabaseBackupService
//{
//    private readonly DbContext _context;
//    private readonly IServiceProvider _serviceProvider;

//    /// <summary>
//    /// Initializes a new instance of the DatabaseBackupService.
//    /// </summary>
//    /// <param name="serviceProvider">The service provider to resolve DbContext instances.</param>
//    public JsonDatabaseBackupService(IServiceProvider serviceProvider)
//    {
//        _serviceProvider = serviceProvider;
//        // Resolve a new DbContext instance to avoid threading issues.
//        _context = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<DatabaseContext>();
//    }

//    /// <summary>
//    /// Backs up all data from all tables in the DbContext to a single JSON file.
//    /// </summary>
//    /// <param name="backupFilePath">The path where the backup JSON file will be saved.</param>
//    /// <returns>A task that represents the asynchronous backup operation.</returns>
//    public async Task BackupAllDataAsync(string backupFilePath)
//    {
//        try
//        {
//            var backupData = new Dictionary<string, object>();

//            // Use reflection to find all DbSet properties on the context
//            var dbSetProperties = _context.GetType().GetProperties()
//                .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));

//            foreach (var dbSetProperty in dbSetProperties)
//            {
//                // Get the entity type from the DbSet
//                var entityType = dbSetProperty.PropertyType.GetGenericArguments()[0];

//                // Get the table name
//                var tableName = _context.Model.FindEntityType(entityType)?.GetTableName();

//                if (tableName != null)
//                {
//                    // Get the DbSet instance from the context
//                    var dbSet = (IQueryable<object>)dbSetProperty.GetValue(_context);

//                    // Read all data from the table
//                    var tableData = await dbSet.ToListAsync();

//                    backupData[tableName] = tableData;
//                }
//            }

//            var jsonOptions = new JsonSerializerOptions
//            {
//                WriteIndented = true,
//                // Preserve references to handle object graphs and potential circular references during serialization.
//                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
//            };

//            string json = JsonSerializer.Serialize(backupData, jsonOptions);

//            await File.WriteAllTextAsync(backupFilePath, json);
//        }
//        catch (Exception ex)
//        {
//            // Handle or log the exception as needed
//            Console.WriteLine($"An error occurred during backup: {ex.Message}");
//            throw;
//        }
//    }

//    /// <summary>
//    /// Restores data from a JSON backup file.
//    /// NOTE: This will clear all existing data in the tables before inserting the backup data.
//    /// </summary>
//    /// <param name="backupFilePath">The path to the backup JSON file.</param>
//    /// <returns>A task that represents the asynchronous restore operation.</returns>
//    public async Task RestoreDataAsync(string backupFilePath)
//    {
//        if (!File.Exists(backupFilePath))
//        {
//            Console.WriteLine("Backup file not found.");
//            return;
//        }

//        try
//        {
//            string json = await File.ReadAllTextAsync(backupFilePath);
//            var jsonOptions = new JsonSerializerOptions
//            {
//                // Use the same reference handler for deserialization.
//                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
//            };
//            var backupData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json, jsonOptions);

//            if (backupData == null)
//            {
//                Console.WriteLine("Failed to deserialize backup data.");
//                return;
//            }

//            // Topologically sort entities to respect foreign key constraints.
//            // Principal entities (like 'Companies') will come before dependent entities (like 'Employees').
//            var sortedEntityTypes = GetSortedEntityTypes();

//            using (var transaction = await _context.Database.BeginTransactionAsync())
//            {
//                // It's safest to disable and re-enable foreign key checks during this operation.
//                // The specific command depends on the database provider.
//                // For SQLite: await _context.Database.ExecuteSqlRawAsync("PRAGMA foreign_keys = OFF;");
//                // For SQL Server: await _context.Database.ExecuteSqlRawAsync("EXEC sp_msforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT all'");

//                // Delete data in reverse topological order to avoid FK violations.
//                // Dependent entities are deleted before their principal entities.
//                foreach (var entityType in sortedEntityTypes.Reverse<IEntityType>())
//                {
//                    var tableName = entityType.GetTableName();
//                    if (!string.IsNullOrEmpty(tableName))
//                    {
//                        await _context.Database.ExecuteSqlRawAsync($"DELETE FROM [{tableName}]");
//                    }
//                }

//                // Insert data in topological order.
//                foreach (var entityType in sortedEntityTypes)
//                {
//                    var tableName = entityType.GetTableName();
//                    if (tableName != null && backupData.TryGetValue(tableName, out var tableDataElement))
//                    {
//                        // Deserialize the JSON for the current table into a list of entities.
//                        var entityList = (System.Collections.IList)JsonSerializer.Deserialize(
//                            tableDataElement.GetRawText(),
//                            typeof(List<>).MakeGenericType(entityType.ClrType),
//                            jsonOptions);

//                        if (entityList != null && entityList.Count > 0)
//                        {
//                            // Add all entities from the list to the context.
//                            // EF Core will track them, but not send them to the DB yet.
//                            _context.AddRange(entityList);
//                        }
//                    }
//                }

//                // Save all changes to the database in a single batch.
//                // EF Core is smart enough to handle the insert order correctly now that all
//                // related entities are tracked.
//                await _context.SaveChangesAsync();

//                // Re-enable foreign key constraints.
//                // For SQLite: await _context.Database.ExecuteSqlRawAsync("PRAGMA foreign_keys = ON;");
//                // For SQL Server: await _context.Database.ExecuteSqlRawAsync("EXEC sp_msforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all'");

//                await transaction.CommitAsync();
//            }
//        }
//        catch (Exception ex)
//        {
//            // Handle or log the exception
//            Console.WriteLine($"An error occurred during restore: {ex.Message}");
//            throw;
//        }
//    }

//    /// <summary>
//    /// Sorts entity types based on their dependencies (foreign keys).
//    /// </summary>
//    /// <returns>A list of entity types sorted in an order safe for insertion.</returns>
//    private List<IEntityType> GetSortedEntityTypes()
//    {
//        var entityTypes = _context.Model.GetEntityTypes().ToList();
//        var dependencyGraph = new Dictionary<IEntityType, List<IEntityType>>();
//        var inDegree = new Dictionary<IEntityType, int>();

//        // Initialize graph and in-degree map
//        foreach (var et in entityTypes)
//        {
//            dependencyGraph[et] = new List<IEntityType>();
//            inDegree[et] = 0;
//        }

//        // Build the graph and in-degrees based on foreign keys
//        foreach (var et in entityTypes)
//        {
//            foreach (var fk in et.GetForeignKeys())
//            {
//                var principalEntityType = fk.PrincipalEntityType;
//                if (principalEntityType != et) // Avoid self-referencing cycles in this simple sort
//                {
//                    // An edge from principal to dependent means principal must be created first.
//                    dependencyGraph[principalEntityType].Add(et);
//                    inDegree[et]++;
//                }
//            }
//        }

//        // Kahn's algorithm for topological sorting
//        var queue = new Queue<IEntityType>(entityTypes.Where(et => inDegree[et] == 0));
//        var sortedList = new List<IEntityType>();

//        while (queue.Count > 0)
//        {
//            var current = queue.Dequeue();
//            sortedList.Add(current);

//            foreach (var dependent in dependencyGraph[current])
//            {
//                inDegree[dependent]--;
//                if (inDegree[dependent] == 0)
//                {
//                    queue.Enqueue(dependent);
//                }
//            }
//        }

//        if (sortedList.Count != entityTypes.Count)
//        {
//            // This indicates a cycle in the dependencies which this simple sort doesn't handle.
//            // Disabling FK constraints during the transaction is a workaround for this.
//            Console.WriteLine("Warning: A circular dependency may exist in the entity model.");
//        }

//        return sortedList;
//    }
//}
//public class JsonDatabaseBackupService
//{
//    private readonly DbContext _context;
//    private readonly IServiceProvider _serviceProvider;
//    private readonly string _backupDirectory = Path.Combine(FileSystem.AppDataDirectory, "Backups");

//    /// <summary>
//    /// Initializes a new instance of the DatabaseBackupService.
//    /// </summary>
//    /// <param name="serviceProvider">The service provider to resolve DbContext instances.</param>
//    public JsonDatabaseBackupService(IServiceProvider serviceProvider)
//    {
//        _serviceProvider = serviceProvider;
//        // Resolve a new DbContext instance to avoid threading issues.
//        _context = _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<DatabaseContext>();
//    }

//    /// <summary>
//    /// Backs up all data from all tables in the DbContext to a single JSON file.
//    /// </summary>
//    /// <param name="backupFilePath">The path where the backup JSON file will be saved.</param>
//    /// <returns>A task that represents the asynchronous backup operation.</returns>
//    public async Task BackupAllDataAsync(string backupFilePath)
//    {
//        try
//        {
//            var backupData = new Dictionary<string, object>();

//            // Use reflection to find all DbSet properties on the context
//            var dbSetProperties = _context.GetType().GetProperties()
//                .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));

//            foreach (var dbSetProperty in dbSetProperties)
//            {
//                // Get the entity type from the DbSet
//                var entityType = dbSetProperty.PropertyType.GetGenericArguments()[0];

//                // Get the table name
//                var tableName = _context.Model.FindEntityType(entityType)?.GetTableName();

//                if (tableName != null)
//                {
//                    // Get the DbSet instance from the context
//                    var dbSet = (IQueryable<object>)dbSetProperty.GetValue(_context);

//                    // Read all data from the table
//                    var tableData = await dbSet.ToListAsync();

//                    backupData[tableName] = tableData;
//                }
//            }

//            var jsonOptions = new JsonSerializerOptions
//            {
//                WriteIndented = true,
//                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
//            };

//            string json = JsonSerializer.Serialize(backupData, jsonOptions);

//            await File.WriteAllTextAsync(backupFilePath, json);
//        }
//        catch (Exception ex)
//        {
//            // Handle or log the exception as needed
//            Console.WriteLine($"An error occurred during backup: {ex.Message}");
//            throw;
//        }
//    }

//    /// <summary>
//    /// Restores data from a JSON backup file.
//    /// NOTE: This will clear all existing data in the tables before inserting the backup data.
//    /// </summary>
//    /// <param name="backupFilePath">The path to the backup JSON file.</param>
//    /// <returns>A task that represents the asynchronous restore operation.</returns>
//    public async Task RestoreDataAsync(string backupFilePath)
//    {
//        if (!File.Exists(backupFilePath))
//        {
//            Console.WriteLine("Backup file not found.");
//            return;
//        }

//        try
//        {
//            string json = await File.ReadAllTextAsync(backupFilePath);
//            var jsonOptions = new JsonSerializerOptions
//            {
//                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
//            };
//            var backupData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json, jsonOptions);

//            if (backupData == null)
//            {
//                Console.WriteLine("Failed to deserialize backup data.");
//                return;
//            }

//            using (var transaction = await _context.Database.BeginTransactionAsync())
//            {
//                // Get all table names from the model
//                var tableNames = _context.Model.GetEntityTypes().Select(e => e.GetTableName()).Distinct().ToList();

//                // It's often necessary to disable foreign key checks or delete data in a specific order.
//                // For simplicity, we'll just delete from all tables. For complex relationships,
//                // you might need a more sophisticated approach to order the deletions and insertions.

//                // Temporarily disable foreign key constraints (example for SQL Server)
//                // await _context.Database.ExecuteSqlRawAsync("EXEC sp_msforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT all'");

//                foreach (var tableName in tableNames)
//                {
//                    if (!string.IsNullOrEmpty(tableName))
//                    {
//                        await _context.Database.ExecuteSqlRawAsync($"DELETE FROM [{tableName}]");
//                    }
//                }

//                var dbSetProperties = _context.GetType().GetProperties()
//                    .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));

//                foreach (var dbSetProperty in dbSetProperties)
//                {
//                    var entityType = dbSetProperty.PropertyType.GetGenericArguments()[0];
//                    var tableName = _context.Model.FindEntityType(entityType)?.GetTableName();

//                    if (tableName != null && backupData.TryGetValue(tableName, out var tableDataElement))
//                    {
//                        var entityList = (System.Collections.IList)JsonSerializer.Deserialize(tableDataElement.GetRawText(), typeof(List<>).MakeGenericType(entityType), jsonOptions);

//                        if (entityList != null)
//                        {
//                            await _context.AddRangeAsync(entityList.Cast<object>());
//                        }
//                    }
//                }

//               int count= await _context.SaveChangesAsync();
//                Debug.WriteLine($"Number of records restored: {count}");
//                // Re-enable foreign key constraints (example for SQL Server)
//                // await _context.Database.ExecuteSqlRawAsync("EXEC sp_msforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all'");

//                await transaction.CommitAsync();
//            }
//        }
//        catch (Exception ex)
//        {
//            // Handle or log the exception
//            Console.WriteLine($"An error occurred during restore: {ex.Message}");
//            throw;
//        }
//    }

//}

////Implementation 
//// In your ViewModel or Page code-behind

//private readonly DatabaseBackupService _backupService;

//    public SettingsViewModel(DatabaseBackupService backupService)
//    {
//        _backupService = backupService;
//    }

//    private async void OnBackupClicked()
//    {
//        try
//        {
//            string backupFileName = $"backup_{DateTime.Now:yyyyMMdd_HHmmss}.json";
//            string backupFilePath = Path.Combine(FileSystem.AppDataDirectory, backupFileName);

//            await _backupService.BackupAllDataAsync(backupFilePath);

//            // Notify user of success
//            await App.Current.MainPage.DisplayAlert("Success", $"Database backed up to {backupFilePath}", "OK");
//        }
//        catch (Exception ex)
//        {
//            await App.Current.MainPage.DisplayAlert("Error", $"Backup failed: {ex.Message}", "OK");
//        }
//    }

//    private async void OnRestoreClicked()
//    {
//        try
//        {
//            // You would typically use a file picker to let the user select the backup file.
//            // For this example, we'll assume a fixed path.
//            string backupFilePath = Path.Combine(FileSystem.AppDataDirectory, "backup.json"); // Or use a file picker result

//            bool confirm = await App.Current.MainPage.DisplayAlert("Confirm Restore", "This will delete all current data and restore from the backup. Are you sure?", "Yes", "No");

//            if (confirm)
//            {
//                await _backupService.RestoreDataAsync(backupFilePath);
//                await App.Current.MainPage.DisplayAlert("Success", "Database restored successfully.", "OK");
//            }
//        }
//        catch (Exception ex)
//        {
//            await App.Current.MainPage.DisplayAlert("Error", $"Restore failed: {ex.Message}", "OK");
//        }
//    }