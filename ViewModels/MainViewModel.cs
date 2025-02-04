using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DearFuture.Models;
using DearFuture.Repositories;
using DearFuture.Services;
using DearFuture.Observers;
namespace DearFuture.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

public class MainViewModel : INotifyPropertyChanged, ICapsuleObserver
{
    private readonly CapsuleService _capsuleService;
    private ObservableCollection<Capsule> _capsules;
    private string _selectedSortOption = "Date Created (Newest)";
    private string _selectedCategory = "All";

    public ObservableCollection<Capsule> Capsules
    {
        get => _capsules;
        set
        {
            _capsules = value;
            OnPropertyChanged(nameof(Capsules));
        }
    }

    public string SelectedSortOption
    {
        get => _selectedSortOption;
        set
        {
            _selectedSortOption = value;
            LoadCapsules(); // Refresh list when sorting changes
        }
    }

    public string SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            _selectedCategory = value;
            LoadCapsules(); // Refresh list when category changes
        }
    }

    public MainViewModel(CapsuleService capsuleService)
    {
        _capsuleService = capsuleService;
        Capsules = new ObservableCollection<Capsule>();

        // ✅ Register as an observer
        CapsuleObservable.AddObserver(this);

        LoadCapsules();
    }

    public async void LoadCapsules()
    {
        var capsulesList = await _capsuleService.GetCapsulesAsync();
        Capsules = new ObservableCollection<Capsule>(capsulesList);
    }

    // ✅ Observer method implementation
    public void UpdateCapsules()
    {
        LoadCapsules(); // Reload capsules when notified
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
