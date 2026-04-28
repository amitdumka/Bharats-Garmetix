using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Garmetix.Core.Services;
using Garmetix.Models;
using System.Collections.ObjectModel;

namespace Garmetix.CoreBase.TimeLines;

public class TimelineGroup : ObservableCollection<TimelineEntry>
{
    public DateTime Date { get; private set; }
    public string FormattedDate => Date.ToString("dddd, MMMM dd, yyyy");

    public TimelineGroup(DateTime date, IEnumerable<TimelineEntry> entries) : base(entries)
    {
        Date = date;
    }
}

public partial class TimelinePageModel : ObservableObject
{
    private readonly TimelineDataService _dataService;

    // Observable collection to hold the grouped timeline entries for display
    [ObservableProperty]
    private ObservableCollection<TimelineGroup> _timelineEntries;

    [ObservableProperty]
    private bool _isBusy; // To show a loading indicator

    // Constructor: Inject the data service
    public TimelinePageModel(TimelineDataService dataService)
    {
        _dataService = dataService;
        TimelineEntries = new ObservableCollection<TimelineGroup>();
        LoadTimelineCommand = new AsyncRelayCommand(LoadTimelineAsync);

        // You might want to load data immediately when the ViewModel is created
        // Or trigger it from the page's OnAppearing event.
        // _ = LoadTimelineAsync();
    }
    [RelayCommand]
    private async Task Appearing() => await LoadTimelineAsync();
    // Command to load the timeline data
    public IAsyncRelayCommand LoadTimelineCommand { get; }

    /// <summary>
    /// Loads and processes timeline data for the last week.
    /// </summary>
    private async Task LoadTimelineAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        try
        {
            // 1. Get all entries from the JSON file
            List<TimelineEntry> allEntries = await _dataService.GetAllEntriesAsync();

            // 2. Define the start and end of the "last week"
            // Get today's date without time component
            DateTime today = DateTime.Today;
            // Calculate the start of the week (e.g., 7 days ago from today)
            DateTime sevenDaysAgo = today.AddDays(-6); // Includes today and 6 previous days

            // 3. Filter entries to include only those within the last week
            // Ensure entries are sorted by date for proper timeline display
            var recentEntries = allEntries
                .Where(e => e.EntryDate.Date >= sevenDaysAgo && e.EntryDate.Date <= today)
                .OrderByDescending(e => e.EntryDate) // Newest first within each day
                .ToList();

            // 4. Group entries by date
            var groupedEntries = recentEntries
                .GroupBy(e => e.EntryDate.Date) // Group by date part only
                .Select(group => new TimelineGroup(group.Key, group.OrderByDescending(x => x.EntryDate))) // Sort within each group
                .OrderByDescending(group => group.Date) // Sort groups by date (newest day first)
                .ToList();

            // 5. Update the observable collection for the UI
            TimelineEntries.Clear();
            foreach (var group in groupedEntries)
            {
                TimelineEntries.Add(group);
            }

            // Optional: Add empty groups for days with no entries in the last week
            // This ensures all 7 days are represented, even if empty.
            for (int i = 0; i < 7; i++)
            {
                DateTime currentDate = today.AddDays(-i);
                if (!TimelineEntries.Any(g => g.Date.Date == currentDate.Date))
                {
                    TimelineEntries.Add(new TimelineGroup(currentDate, new List<TimelineEntry>()));
                }
            }
            // Re-sort after adding empty groups
            TimelineEntries = new ObservableCollection<TimelineGroup>(TimelineEntries.OrderByDescending(g => g.Date));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading timeline: {ex.Message}");
            // In a real app, you'd show an alert to the user
            // await Application.Current.MainPage.DisplayAlert("Error", "Failed to load timeline data.", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    // Example method to add a new entry (would be called from an Add/Edit page)
    public async Task AddOrUpdateEntry(TimelineEntry entry)
    {
        await _dataService.SaveOrUpdateEntryAsync(entry);
        await LoadTimelineAsync(); // Reload timeline after change
    }

    // Example method to delete an entry (would be called from an item context menu)
    public async Task DeleteEntry(Guid entryId)
    {
        await _dataService.DeleteEntryAsync(entryId);
        await LoadTimelineAsync(); // Reload timeline after change
    }
}