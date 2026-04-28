/*
 * TimelineEntry.cs
 * 
 * TimelineEntry represents a single entry in a timeline, such as a log of events or activities. It includes properties for the date of the entry, a title, and a description. This model can be used to display a chronological list of events in the Garmetix application.
 * 
 * This file is part of Garmetix.
 *
 * Garmetix is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Garmetix is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Garmetix.  If not, see <http://www.gnu.org/licenses/>.
 */
/*
 * Garmetix
 * Author: Amit Kumar
 * https://garmetix.com/
 * Copyright (c) 2025. All rights reserved.
 * Version: 5.0.0
 * License: https://garmetix.com/license
 * Website: https://garmetix.com/
*/
using System.ComponentModel.DataAnnotations;

namespace Garmetix.Core.Models
{
    public class TimelineEntry
    {
        // Unique identifier for the entry. Useful for updates.
        [Display(Name = "ID")]public Guid Id { get; set; }

        // The date and time of the timeline entry.
        [Display(Name = "Entry Date")]public DateTime EntryDate { get; set; }

        // A short title for the entry.
        [Display(Name = "Title")]public string Title { get; set; }

        // A more detailed description of the entry.
        [Display(Name = "Description")]public string Description { get; set; } = string.Empty;

        // Constructor to easily create new entries
        public TimelineEntry()
        {
            Id = Guid.NewGuid(); // Generate a new GUID for each new entry
            EntryDate = DateTime.Now; // Default to current date/time
            Title = string.Empty;
            Description = string.Empty;
        }

        // You can add a method to check if two entries are the same based on ID
        public override bool Equals(object? obj)
        {
            return obj is TimelineEntry entry &&
                   Id.Equals(entry.Id);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }

}
