using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ViewModels
{
    public partial class TrashCapsulesViewModel : BaseViewModel
    {
        private readonly CapsuleService _capsuleService;
        private readonly MainViewModel _mainViewModel;

        [ObservableProperty]
        private ObservableCollection<CapsulePreview> trashCapsules;

        public TrashCapsulesViewModel(CapsuleService capsuleService, MainViewModel mainViewModel)
        {
            _capsuleService = capsuleService;
            _mainViewModel = mainViewModel;
            Title = "Trash";
            LoadTrashCapsules();
        }

        [RelayCommand]
        private async Task LoadTrashCapsules()
        {
            await _capsuleService.CleanupOldTrashedCapsulesAsync();
            var capsules = await _capsuleService.GetTrashedCapsulesAsync();
            TrashCapsules = new ObservableCollection<CapsulePreview>(capsules);
        }

        [RelayCommand]
        public async Task RestoreCapsuleAsync(int id)
        {
            await _capsuleService.RestoreCapsuleAsync(id);
            await LoadTrashCapsules();
            await _mainViewModel.LoadCapsules();
        }

        [RelayCommand]
        public async Task PermanentlyDeleteCapsuleAsync(int id)
        {
            await _capsuleService.DeletePermanentlyAsync(id);
            var capsule = TrashCapsules.FirstOrDefault(c => c.Id == id);
            if (capsule != null)
            {
                TrashCapsules.Remove(capsule);
            }
        }
    }
} 