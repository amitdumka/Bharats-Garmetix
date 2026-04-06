using Garmetix.Models.Enums;
using SQLite;

namespace Garmetix.Models
{

}
namespace Garmetix.Models.Queues
{
    /// <summary>
    /// Sync Queue for local database 
    /// </summary>
    public class SyncQueue
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string DataJson { get; set; }
        public string EntityType { get; set; }  // "Product" or "Invoice"
        public DateTime SyncedAt { get; set; } = DateTime.UtcNow; // Timestamp of when the item was synced
        public bool Synced { get; set; } = false;
    }
    public class RemoteSyncQueue
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string DataJson { get; set; }
        public string EntityType { get; set; }  // "Product" or "Invoice"
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Timestamp of when the item was created
        public DateTime SyncedAt { get; set; } = DateTime.UtcNow; // Timestamp of when the item was synced
        public bool Synced { get; set; } = false;
    }
}


namespace Garmetix.Models.Auth
{
    // All the code in this file is included in all platforms.
    public class AppUser
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public LoginRole Role { get; set; }
        public UserType UserType { get; set; }
        public Guid? RemoteUserId { get; set; }
        public Guid? EmployeeId { get; set; }
        public Guid? CompanyId { get; set; }
        public Guid? StoreGroupId { get; set; }
        public Guid? StoreId { get; set; }
        public bool Admin { get; set; } = false;
        public AppOperation AppOperation { get; set; } = AppOperation.Store;
    }
}
