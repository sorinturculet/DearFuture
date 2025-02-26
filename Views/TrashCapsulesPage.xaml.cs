using DearFuture.ViewModels;
using DearFuture.Models;
using ViewModels;
namespace DearFuture.Views
{
    public partial class TrashCapsulesPage : ContentPage
    {
        private readonly TrashCapsulesViewModel _viewModel;

        public TrashCapsulesPage(TrashCapsulesViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        private async void OnTrashCapsuleTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is CapsulePreview capsule)
            {
                string message = $"This capsule was moved to trash on {capsule.StatusChangedAt:MMMM dd, yyyy}.";
                await DisplayAlert($"Trash Capsule: {capsule.Title}", message, "OK");
            }
        }

        private async void OnRestoreCapsuleClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int capsuleId)
            {
                bool confirm = await DisplayAlert("Restore Capsule",
                    "Are you sure you want to restore this capsule?",
                    "Restore", "Cancel");

                if (confirm)
                {
                    await _viewModel.RestoreCapsuleAsync(capsuleId);
                }
            }
        }

        private async void OnDeleteCapsulePermanentlyClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int capsuleId)
            {
                bool confirm = await DisplayAlert("Delete Permanently",
                    "Are you sure you want to permanently delete this capsule? This action cannot be undone.",
                    "Delete", "Cancel");

                if (confirm)
                {
                    await _viewModel.PermanentlyDeleteCapsuleAsync(capsuleId);
                }
            }
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync(); // Navigate back to MainPage
        }
    }
}
