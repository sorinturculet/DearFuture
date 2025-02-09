using DearFuture.ViewModels;
using DearFuture.Models;
namespace DearFuture.Views
{
    public partial class TrashCapsulesPage : ContentPage
    {
        private readonly MainViewModel _viewModel;

        public TrashCapsulesPage(MainViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel; // Store reference to ViewModel
        }

        private async void OnTrashCapsuleTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is Capsule capsule)
            {
                string message = $"This capsule was deleted on {capsule.DeletedAt:MMMM dd, yyyy}.";
                await DisplayAlert($"Trash Capsule: {capsule.Title}", message, "OK");
            }
        }

        private async void OnRestoreCapsuleClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is Capsule capsule)
            {
                await _viewModel.RestoreCapsuleAsync(capsule.Id);
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
                    await _viewModel.PermanentlyDeleteCapsuleAsync(capsuleId); // Implement permanent deletion if needed
                }
            }
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync(); // Navigate back to MainPage
        }
    }
}
