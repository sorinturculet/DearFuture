using DearFuture.ViewModels;
using DearFuture.Models;
using Microsoft.Extensions.DependencyInjection;
using ViewModels;
namespace DearFuture.Views
{
    public partial class MainPage : ContentPage
    {
        private readonly MainViewModel _viewModel;
        private readonly IServiceProvider _serviceProvider;

        public MainPage(MainViewModel viewModel, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            BindingContext = viewModel;
            _viewModel = viewModel; // Store reference to ViewModel
            _serviceProvider = serviceProvider;
        }

        // Finds the corresponding Frame UI element for a given capsule ID
        private Frame FindCapsuleFrame(int capsuleId)
        {
            foreach (var item in CapsuleListView.ItemsSource)
            {
                if (item is Capsule capsule && capsule.Id == capsuleId)
                {
                    return (Frame)CapsuleListView.ItemTemplate.CreateContent();
                }
            }
            return null;
        }

        // Navigates to the Create Capsule page when the add button is clicked
        private async void OnAddCapsuleClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(CreateCapsulePage));
        }

        // Handles tapping on a capsule to attempt unlocking it
        private async void OnCapsuleTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is CapsulePreview capsule)
            {
                string message = await _viewModel.OpenCapsule(capsule.Id);
                await DisplayAlert($"Capsule: {capsule.Title}", message, "OK");
            }
        }

        // Deletes the selected capsule when the delete button is clicked
        private async void OnMoveToTrashClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int capsuleId)
            {
                await _viewModel.MoveToTrash(capsuleId);
            }
        }

        // Navigates to the Archived Capsules page when the archived button is clicked
        private async void OnViewArchivedCapsulesClicked(object sender, EventArgs e)
        {
            var archivedViewModel = _serviceProvider.GetService<ArchivedCapsulesViewModel>();
            await Navigation.PushAsync(new ArchivedCapsulesPage(archivedViewModel));
        }

        // Navigates to the Trash Capsules page when the trash button is clicked
        private async void OnViewTrashCapsulesClicked(object sender, EventArgs e)
        {
            var trashViewModel = _serviceProvider.GetService<TrashCapsulesViewModel>();
            await Navigation.PushAsync(new TrashCapsulesPage(trashViewModel));
        }
    }
}
