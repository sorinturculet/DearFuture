using DearFuture.ViewModels;
using DearFuture.Models;
namespace DearFuture.Views
{
    public partial class MainPage : ContentPage
    {
        private readonly MainViewModel _viewModel;

        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            _viewModel = viewModel; //  Store reference to ViewModel
        }
        
        private async void OnAddCapsuleClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(CreateCapsulePage));
        }

        private async void OnCapsuleTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is Capsule capsule)
            {
                string message = await _viewModel.OpenCapsuleAsync(capsule.Id);
                await DisplayAlert($" Capsule: {capsule.Title}", message, "OK");
            }
        }

        private async void OnDeleteCapsuleClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int capsuleId)
            {
                await _viewModel.DeleteCapsuleAsync(capsuleId);
            }
        }

        private async void OnViewArchivedCapsulesClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ArchivedCapsulesPage(_viewModel));
        }

    }
}