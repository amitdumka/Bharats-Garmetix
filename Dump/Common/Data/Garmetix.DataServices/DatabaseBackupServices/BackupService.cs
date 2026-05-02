using Garmetix.Databases;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Reflection;
using System.Text.Json;

namespace Garmetix.DataServices.DatabaseBackupServices
{
    /// <summary>
    /// A generic backup service that serializes every DbSet in your DbContext into one JSON file.
    /// It uses reflection so any new table added to AppDbContext is automatically included.
    /// </summary>
    public class BackupService
    {
        private readonly DatabaseContext _context;
        private readonly string _backupFilePath;

        internal BackupService(DatabaseContext context)
        {
            _context = context;
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _backupFilePath = Path.Combine(folder, "backup.json");
        }

        /// <summary>
        /// Backs up all data in the database into a JSON file.
        /// </summary>
        public async Task BackupAsync()
        {
            var backupData = new Dictionary<string, object>();

            // Get all public properties in AppDbContext that are of type DbSet<T>.
            var dbSetProperties = typeof(DatabaseContext).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType.IsGenericType &&
                            p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));

            foreach (var prop in dbSetProperties)
            {
                // Get the entity type (T in DbSet<T>).
                Type entityType = prop.PropertyType.GetGenericArguments()[0];

                // Obtain the DbSet<T> via the generic Set<T>() method.
                var methodSet = typeof(DbContext).GetMethod("Set", BindingFlags.Public | BindingFlags.Instance);
                var genericSetMethod = methodSet.MakeGenericMethod(entityType);
                var dbSet = genericSetMethod.Invoke(_context, null);

                // Call ToListAsync<T>(dbSet, CancellationToken.None) via reflection.
                var toListAsyncMethod = typeof(EntityFrameworkQueryableExtensions)
                    .GetMethod("ToListAsync",
                        new Type[] { typeof(IQueryable<>).MakeGenericType(entityType), typeof(CancellationToken) });
                var task = (Task)toListAsyncMethod.Invoke(null, new object[] { dbSet, CancellationToken.None });
                await task.ConfigureAwait(false);

                // Retrieve the result from the completed Task.
                var resultProperty = task.GetType().GetProperty("Result");
                var list = resultProperty.GetValue(task);

                // Use the property name (e.g. "Users") as key.
                backupData[prop.Name] = list;
            }

            // Serialize the backup dictionary to JSON with indented formatting.
            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(backupData, jsonOptions);
            await File.WriteAllTextAsync(_backupFilePath, json);
        }

        /// <summary>
        /// Restores the database from the JSON backup file.
        /// </summary>

        public async Task RestoreAsync()
        {
            if (!File.Exists(_backupFilePath))
            {
                return;
            }

            string json = await File.ReadAllTextAsync(_backupFilePath);

            // Parse the JSON into a document.
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            // Get all public properties in AppDbContext that are of type DbSet<T>.
            var dbSetProperties = typeof(DatabaseContext).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType.IsGenericType &&
                            p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));

            foreach (var prop in dbSetProperties)
            {
                if (root.TryGetProperty(prop.Name, out var jsonElement))
                {
                    // Get the entity type (T) for the current table.
                    Type entityType = prop.PropertyType.GetGenericArguments()[0];
                    // Construct the type for List<T>.
                    var listType = typeof(List<>).MakeGenericType(entityType);

                    // Deserialize the JSON array into a List<T>.
                    var deserializedList = JsonSerializer.Deserialize(jsonElement.GetRawText(), listType);

                    // Get the corresponding DbSet from the context using reflection.
                    var setMethod = typeof(DbContext).GetMethod("Set", BindingFlags.Public | BindingFlags.Instance);
                    var genericSetMethod = setMethod.MakeGenericMethod(entityType);
                    var dbSet = genericSetMethod.Invoke(_context, null);

                    // Remove all existing items from the DbSet.
                    var items = ((IEnumerable)dbSet).Cast<object>().ToList();
                    _context.RemoveRange(items);
                    await _context.SaveChangesAsync();

                    // Add the deserialized items.
                    // We use reflection to call the AddRange method.
                    var addRangeMethod = dbSet.GetType().GetMethod("AddRange");
                    addRangeMethod.Invoke(dbSet, new object[] { deserializedList });
                    await _context.SaveChangesAsync();
                }
            }
        }
        //public async Task RestoreAsync()
        //{
        //    if (!File.Exists(_backupFilePath))
        //        return;

        //    string json = await File.ReadAllTextAsync(_backupFilePath);

        //    // Parse the JSON into a document.
        //    using var doc = JsonDocument.Parse(json);
        //    var root = doc.RootElement;

        //    // Get all public properties in AppDbContext that are of type DbSet<T>.
        //    var dbSetProperties = typeof(DatabaseContext).GetProperties(BindingFlags.Public | BindingFlags.Instance)
        //        .Where(p => p.PropertyType.IsGenericType &&
        //                    p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));

        //    foreach (var prop in dbSetProperties)
        //    {
        //        if (root.TryGetProperty(prop.Name, out var jsonElement))
        //        {
        //            // Get the entity type (T) for the current table.
        //            Type entityType = prop.PropertyType.GetGenericArguments()[0];
        //            // Construct the type for List<T>.
        //            var listType = typeof(List<>).MakeGenericType(entityType);

        //            // Deserialize the JSON array into a List<T>.
        //            var deserializedList = JsonSerializer.Deserialize(jsonElement.GetRawText(), listType);

        //            // Get the corresponding DbSet from the context.
        //            var dbSet = _context.Set(entityType);

        //            // Remove all existing items from the DbSet.
        //            var items = ((IEnumerable)dbSet).Cast<object>().ToList();
        //            _context.RemoveRange(items);
        //            await _context.SaveChangesAsync();

        //            // Add the deserialized items. Using dynamic makes the AddRange call simpler.
        //            ((dynamic)dbSet).AddRange((dynamic)deserializedList);
        //            await _context.SaveChangesAsync();
        //        }
        //    }
        //}
    }
}
