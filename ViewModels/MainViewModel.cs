using DearFuture.Models;
using DearFuture.Services;
namespace DearFuture.ViewModels;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Maui.Devices.Sensors;
using CommunityToolkit.Mvvm.ComponentModel;

public partial class MainViewModel : BaseViewModel, IDisposable
{
    private readonly CapsuleService _capsuleService;
    private readonly IGeolocation _geolocation;

    [ObservableProperty]
    private ObservableCollection<CapsulePreview> capsules;

    [ObservableProperty]
    private string selectedSortOption = "Default";

    [ObservableProperty]
    private string selectedCategory = "All";

    public MainViewModel(CapsuleService capsuleService, IGeolocation geolocation)
    {
        Title = "Dear Future";
        _capsuleService = capsuleService;
        _geolocation = geolocation;
        Capsules = new ObservableCollection<CapsulePreview>();
        
        StartTimer();
        LoadCapsulesCommand.Execute(null);
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

    [RelayCommand]
    public async Task LoadCapsules()
    {
        var capsulesList = await _capsuleService.GetLockedCapsulesAsync(SelectedCategory, SelectedSortOption);
        Capsules = new ObservableCollection<CapsulePreview>(capsulesList);
    }

    [RelayCommand]
    public async Task<string> OpenCapsule(int id)
    {
        var capsule = Capsules.FirstOrDefault(c => c.Id == id);
        if (capsule == null) return "Capsule not found.";

        // Check if it's time to unlock
        if (!capsule.IsUnlocked)
        {
            var timeRemaining = _capsuleService.GetTimeRemaining(capsule);
            if (timeRemaining != "Unlocked!")
            {
                return $"This capsule will unlock in {timeRemaining}";
            }
        }

        // Try to open the capsule (includes location check)
        var (success, message) = await _capsuleService.TryOpenCapsuleAsync(id, _geolocation);
        if (!success)
            return message;

        // If successful, update UI
        Capsules.Remove(capsule);
        await LoadCapsules(); // Refresh all lists to ensure proper state

        return $"Message: {message}\n\nThis capsule has been moved to your archives.";
    }

    [RelayCommand]
    public async Task<bool> MoveToTrash(int capsuleId)
    {
        var capsule = Capsules.FirstOrDefault(c => c.Id == capsuleId);
        if (capsule != null)
        {
            await _capsuleService.MoveCapsuleToTrashAsync(capsuleId);
            Capsules.Remove(capsule);
            return true;
        }
        return false;
    }

    private bool _isDisposed;
    private async void StartTimer()
    {
        while (!_isDisposed)
        {
            UpdateCapsuleTimers();
            await Task.Delay(1000);
        }
    }

    private void UpdateCapsuleTimers()
    {
        foreach (var capsule in Capsules)
        {
            capsule.TimeRemaining = _capsuleService.GetTimeRemaining(capsule);
        }
    }

    partial void OnSelectedSortOptionChanged(string value)
    {
        LoadCapsulesCommand.Execute(null);
    }

    partial void OnSelectedCategoryChanged(string value)
    {
        LoadCapsulesCommand.Execute(null);
    }

    public void Dispose()
    {
        _isDisposed = true;
    }

    public async Task<string> GetCapsuleMessageAsync(int capsuleId)
    {
        return await _capsuleService.GetMessageAsync(capsuleId);
    }
}
