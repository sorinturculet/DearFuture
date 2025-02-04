using DearFuture.ViewModels;

namespace DearFuture.Views
{
    public partial class MainPage : ContentPage
    {
        private readonly MainViewModel _viewModel;

        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            _viewModel = viewModel; // ✅ Store reference to ViewModel
        }

        private async void OnAddCapsuleClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(CreateCapsulePage));
        }

        private async void OnDeleteCapsuleClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int capsuleId)
            {
                await _viewModel.DeleteCapsuleAsync(capsuleId);
            }
        }

    }
}
