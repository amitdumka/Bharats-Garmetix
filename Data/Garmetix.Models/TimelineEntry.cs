namespace Garmetix.Models
{
    public class TimelineEntry
    {
        // Unique identifier for the entry. Useful for updates.
        public Guid Id { get; set; }

        // The date and time of the timeline entry.
        public DateTime EntryDate { get; set; }

        // A short title for the entry.
        public string Title { get; set; }

        // A more detailed description of the entry.
        public string Description { get; set; }

        // Constructor to easily create new entries
        public TimelineEntry()
        {
            Id = Guid.NewGuid(); // Generate a new GUID for each new entry
            EntryDate = DateTime.Now; // Default to current date/time
            Title = string.Empty;
            Description = string.Empty;
        }

        // You can add a method to check if two entries are the same based on ID
        public override bool Equals(object obj)
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
