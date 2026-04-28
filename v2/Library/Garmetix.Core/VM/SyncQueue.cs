/*
 * Garmetix
 * Author: Amit Kumar
 * https://garmetix.com/
 * Copyright (c) 2026. All rights reserved.
 * Version: 6.0.0
 * License: https://garmetix.com/license
 * Website: https://garmetix.com/
*/
/* * SyncQueue.cs
 * 
 * This file defines the SyncQueue and RemoteSyncQueue classes, which are used to manage synchronization of data between the local database and a remote server. 
 * Each class contains properties to store the JSON representation of the data, the type of entity being synchronized (e.g., "Product" or "Invoice"), timestamps for when the data was created and synced, and a flag to indicate whether the data has been successfully synced.
 * 
 * The SyncQueue class is intended for local synchronization, while the RemoteSyncQueue class is designed
*/


using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Garmetix.Core.VM
{
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
