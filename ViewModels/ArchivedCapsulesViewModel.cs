using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DearFuture.Services;
using DearFuture.Models;
using DearFuture.ViewModels;
using System.Linq;

namespace ViewModels
{
    public partial class ArchivedCapsulesViewModel : BaseViewModel
    {
        private readonly CapsuleService _capsuleService;
        private readonly MainViewModel _mainViewModel;

        [ObservableProperty]
        private ObservableCollection<CapsulePreview> archivedCapsules;

        public ArchivedCapsulesViewModel(CapsuleService capsuleService, MainViewModel mainViewModel)
        {
            _capsuleService = capsuleService;
            _mainViewModel = mainViewModel;
            Title = "Archived Capsules";
            ArchivedCapsules = new ObservableCollection<CapsulePreview>();
            LoadArchivedCapsulesCommand.Execute(null);
        }

        [RelayCommand]
        private async Task LoadArchivedCapsules()
        {
            var capsules = await _capsuleService.GetArchivedCapsulesAsync();
            ArchivedCapsules = new ObservableCollection<CapsulePreview>(capsules);
        }

        [RelayCommand]
        public async Task<string> GetCapsuleMessage(int id)
        {
            return await _capsuleService.GetMessageAsync(id);
        }

        [RelayCommand]
        public async Task<bool> MoveToTrash(int capsuleId)
        {
            var capsule = ArchivedCapsules.FirstOrDefault(c => c.Id == capsuleId);
            if (capsule != null)
            {
                await _capsuleService.MoveCapsuleToTrashAsync(capsuleId);
                ArchivedCapsules.Remove(capsule);
                return true;
            }
            return false;
        }
    }
} 