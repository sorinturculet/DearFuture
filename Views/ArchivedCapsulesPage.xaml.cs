using DearFuture.ViewModels;
using DearFuture.Models;
using DearFuture.ViewModels;
using ViewModels;

namespace DearFuture.Views
{
    public partial class ArchivedCapsulesPage : ContentPage
    {
        private readonly ArchivedCapsulesViewModel _viewModel;

        public ArchivedCapsulesPage(ArchivedCapsulesViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        private async void OnArchivedCapsuleTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is CapsulePreview capsule)
            {
                string message = await _viewModel.GetCapsuleMessage(capsule.Id);
                await DisplayAlert($"Capsule: {capsule.Title}", message, "OK");
            }
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync(); // Navigate back to MainPage
        }

        private async void OnMoveToTrashClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int capsuleId)
            {
                await _viewModel.MoveToTrash(capsuleId);
            }
        }
    }
}
