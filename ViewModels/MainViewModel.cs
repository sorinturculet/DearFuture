using DearFuture.Models;
using DearFuture.Services;
using DearFuture.Observers;
namespace DearFuture.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Timers;

public class MainViewModel : INotifyPropertyChanged, ICapsuleObserver
{
    private readonly CapsuleService _capsuleService;
    private ObservableCollection<Capsule> _capsules;
    private ObservableCollection<Capsule> _archivedCapsules;
    private string _selectedSortOption = "Default";
    private string _selectedCategory = "All";
    private static System.Timers.Timer _timer;

    public ObservableCollection<Capsule> Capsules
    {
        get => _capsules;
        set
        {
            _capsules = value;
            OnPropertyChanged(nameof(Capsules));
        }
    }

    public ObservableCollection<Capsule> ArchivedCapsules
    {
        get => _archivedCapsules;
        set
        {
            _archivedCapsules = value;
            OnPropertyChanged(nameof(ArchivedCapsules));
        }
    }

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

    public List<string> CategoryOptions { get; } = new()
    {
        "All",
        "Event",
        "Reminder",
        "Reflection"
    };

    public string SelectedSortOption
    {
        get => _selectedSortOption;
        set
        {
            _selectedSortOption = value;
            LoadCapsules(); // Refresh when sorting changes
        }
    }

    public string SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            _selectedCategory = value;
            LoadCapsules(); // Refresh when category changes
        }
    }

    public MainViewModel(CapsuleService capsuleService)
    {
        _capsuleService = capsuleService;
        Capsules = new ObservableCollection<Capsule>();
        ArchivedCapsules = new ObservableCollection<Capsule>();

        // ✅ Register as an observer
        CapsuleObservable.AddObserver(this);

        LoadCapsules();
    }

    public async void LoadCapsules()
    {
        var capsulesList = await _capsuleService.GetLockedCapsulesAsync(SelectedCategory, SelectedSortOption);
        Capsules = new ObservableCollection<Capsule>(capsulesList);

        var archivedCapsulesList = await _capsuleService.GetArchivedCapsulesAsync();
        ArchivedCapsules = new ObservableCollection<Capsule>(archivedCapsulesList);
    }

    // ✅ Observer method implementation
    public void UpdateCapsules()
    {
        LoadCapsules(); // Reload capsules when notified
    }

    public async Task<string> OpenCapsuleAsync(int id)
    {
        string message = await _capsuleService.OpenCapsuleAsync(id);
        LoadCapsules(); // Refresh lists after opening a capsule
        return message;
    }

    public async Task DeleteCapsuleAsync(int id)
    {
        await _capsuleService.DeleteCapsuleAsync(id); // Calls the service to delete
        Capsules.Remove(Capsules.FirstOrDefault(c => c.Id == id)); // Remove from UI
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
