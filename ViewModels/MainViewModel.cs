using DearFuture.Models;
using DearFuture.Services;
using DearFuture.Observers;
namespace DearFuture.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Maui.Devices.Sensors;

public class MainViewModel : INotifyPropertyChanged, ICapsuleObserver
{
    private readonly CapsuleService _capsuleService;
    public IGeolocation geolocation;
    private ObservableCollection<Capsule> _capsules;
    private ObservableCollection<Capsule> _archivedCapsules;
    private string _selectedSortOption = "Default";
    private string _selectedCategory = "All";

    public MainViewModel(CapsuleService capsuleService, IGeolocation geolocation)
    {
        _capsuleService = capsuleService;
        Capsules = new ObservableCollection<Capsule>();
        ArchivedCapsules = new ObservableCollection<Capsule>();

        // Register as an observer to receive capsule updates
        CapsuleObservable.AddObserver(this);

        // Start a background timer to update countdowns for locked capsules
        StartTimer();

        this.geolocation = geolocation;
    }

    // Collection of locked capsules
    public ObservableCollection<Capsule> Capsules
    {
        get => _capsules;
        set
        {
            _capsules = value;
            OnPropertyChanged(nameof(Capsules));
        }
    }

    // Collection of archived (opened) capsules
    public ObservableCollection<Capsule> ArchivedCapsules
    {
        get => _archivedCapsules;
        set
        {
            _archivedCapsules = value;
            OnPropertyChanged(nameof(ArchivedCapsules));
        }
    }

    // Sorting options available for capsules
    public List<string> SortOptions { get; } = new()
    {
        "Default",
        "Date Created (Newest)",
        "Date Created (Oldest)",
        "Name (A-Z)",
        "Name (Z-A)",
        "Unlock Date (Soonest)",
        "Unlock Date (Latest)"
    };

    // Category options available for filtering
    public List<string> CategoryOptions { get; } = new()
    {
        "All",
        "Event",
        "Reminder",
        "Reflection"
    };

    // Currently selected sorting option
    public string SelectedSortOption
    {
        get => _selectedSortOption;
        set
        {
            _selectedSortOption = value;
            LoadCapsules(); // Refresh capsules when sorting changes
        }
    }

    // Currently selected category filter
    public string SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            _selectedCategory = value;
            LoadCapsules(); // Refresh capsules when category changes
        }
    }

    // Starts a background timer that updates the countdown timers every second
    private async void StartTimer()
    {
        while (true)
        {
            UpdateCapsuleTimers();
            await Task.Delay(1000);
        }
    }

    // Updates the remaining time for each locked capsule
    private void UpdateCapsuleTimers()
    {
        foreach (var capsule in Capsules)
        {
            TimeSpan remaining = capsule.UnlockDate - DateTime.Now;
            capsule.TimeRemaining = remaining.TotalSeconds > 0
                ? $"{remaining.Days:D2}d:{remaining.Hours:D2}h:{remaining.Minutes:D2}m:{remaining.Seconds:D2}s"
                : "Unlocked!";
        }
    }

    // Loads both locked and archived capsules
    public async void LoadCapsules()
    {
        var capsulesList = await _capsuleService.GetLockedCapsulesAsync(SelectedCategory, SelectedSortOption);
        Capsules = new ObservableCollection<Capsule>(capsulesList);

        var archivedCapsulesList = await _capsuleService.GetArchivedCapsulesAsync();
        ArchivedCapsules = new ObservableCollection<Capsule>(archivedCapsulesList);
    }

    // Observer method - Reloads capsules when notified of updates
    public void UpdateCapsules()
    {
        LoadCapsules();
    }

    // Attempts to open a capsule if it is unlocked and meets the location requirement
    public async Task<string> OpenCapsuleAsync(int id)
    {
        var capsule = Capsules.FirstOrDefault(c => c.Id == id);
        if (capsule == null)
            return "Capsule not found.";

        // Check if capsule requires location validation before opening
        if (capsule.HasLocation)
        {
            try
            {
                var location = await geolocation.GetLastKnownLocationAsync();
                if (location == null)
                {
                    location = await geolocation.GetLocationAsync(
                        new GeolocationRequest
                        {
                            DesiredAccuracy = GeolocationAccuracy.Medium,
                            Timeout = TimeSpan.FromSeconds(30),
                        });

                    if (location == null)
                    {
                        return "Error getting location.";
                    }
                }

                // Calculate distance between user and capsule location
                var distance = location.CalculateDistance(capsule.Latitude.Value, capsule.Longitude.Value, DistanceUnits.Kilometers);

                // Capsule can only be opened if the user is within 1 km
                if (distance > 1)
                {
                    return "You are too far from the capsule.";
                }
            }
            catch (Exception ex)
            {
                return "Error getting location.";
            }
        }

        // Attempt to unlock the capsule through the service
        string message = await _capsuleService.OpenCapsuleAsync(id);

        // If the capsule remains locked, return without updating UI
        if (!capsule.IsUnlocked)
            return message;

        // Mark the capsule as opened
        capsule.IsOpened = true;

        // Move the capsule from locked to archived list
        Capsules.Remove(capsule);
        ArchivedCapsules.Add(capsule);

        // Notify UI of updates
        OnPropertyChanged(nameof(Capsules));
        OnPropertyChanged(nameof(ArchivedCapsules));

        return message;
    }

    // Deletes a capsule by its ID
    public async Task DeleteCapsuleAsync(int id)
    {
        await _capsuleService.DeleteCapsuleAsync(id);
        Capsules.Remove(Capsules.FirstOrDefault(c => c.Id == id));
    }

    public event PropertyChangedEventHandler PropertyChanged;

    // Notifies the UI when a property value changes
    protected virtual void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
