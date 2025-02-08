using DearFuture.Models;
using DearFuture.ViewModels;

namespace DearFuture.Views
{
    public partial class CreateCapsulePage : ContentPage
    {
        private readonly CreateCapsuleViewModel _viewModel;

        public CreateCapsulePage(CreateCapsuleViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }
        private async void OnSaveCapsuleClicked(object sender, EventArgs e)
        {
            await _viewModel.SaveCapsuleAsync(); // Call ViewModel Save method
        }
    }
}
