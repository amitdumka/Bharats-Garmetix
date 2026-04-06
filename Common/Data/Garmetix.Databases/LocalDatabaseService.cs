using Garmetix.Models.Queues;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SQLite;

//TODO: uploading sync is implemented, but need to implement Remote Sync , data from other client, and download sync

namespace Garmetix.Databases
{
    /// <summary>
    /// Local Cache Database Service
    /// It handles caching of data in the local database.
    /// </summary>
    public class LocalDatabaseService
    {
        private readonly SQLiteAsyncConnection _database;

        public LocalDatabaseService()
        {
            _database = new SQLiteAsyncConnection(Constants.DatabasePath);
        }

        private void CreateDatabase()
        {
            _database.CreateTableAsync<SyncQueue>().Wait();
            _database.CreateTableAsync<RemoteSyncQueue>().Wait();
        }

        // Add or update inventory item
        public async Task SaveAsync<T>(T entity) where T : class
        {
            await _database.InsertOrReplaceAsync(entity);
        }

        // Fetch inventory from cache
        public async Task<List<T>> GetsAsync<T>() where T : class, new()
        {
            return await _database.Table<T>().ToListAsync();
        }

        public async Task<T> GetAsync<T>(Guid id) where T : class, new()
        {
            return await _database.Table<T>().FirstOrDefaultAsync(x => EF.Property<Guid>(x, "Id") == id);
        }

        public async Task<T> GetAsync<T>(int id) where T : class, new()
        {
            return await _database.Table<T>().FirstOrDefaultAsync(x => EF.Property<int>(x, "Id") == id);
        }

        public async Task DeleteAsync<T>(T entity) where T : class
        {
            await _database.DeleteAsync(entity);
        }

        public void DropDatabase()
        {
            _database.DropTableAsync<SyncQueue>().Wait();
            _database.DropTableAsync<RemoteSyncQueue>().Wait();
        }

        //Batch Sync
        //Add queue operations:
        public async Task AddToSyncQueueAsync(object entity, string entityType)
        {
            var dataJson = JsonConvert.SerializeObject(entity);
            var syncItem = new SyncQueue { DataJson = dataJson, EntityType = entityType };
            await _database.InsertAsync(syncItem);
        }

        /// <summary>
        /// Get all pending items from the sync queue for uploading
        /// </summary>
        /// <returns></returns>
        public async Task<List<SyncQueue>> GetPendingSyncItemsAsync()
        {
            return await _database.Table<SyncQueue>().Where(x => !x.Synced).ToListAsync();
        }

        /// <summary>
        /// Mark an item as synced in the sync queue
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task MarkAsSyncedAsync(int id)
        {
            var item = await _database.FindAsync<SyncQueue>(id);
            if (item != null)
            {
                item.Synced = true;
                item.SyncedAt = DateTime.UtcNow;
                await _database.UpdateAsync(item);
            }
        }

        /// <summary>
        /// Remove all stale items from the sync queue that are older than 7 days.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteStaleItemsAsync(Guid id)
        {
            var staleItems = await _database.Table<SyncQueue>().Where(x => x.Synced && x.SyncedAt < DateTime.UtcNow.AddDays(-7)).ToListAsync();
            if (staleItems != null && staleItems.Count > 0)
            {
                await _database.DeleteAsync(staleItems);
            }
        }
    }
}