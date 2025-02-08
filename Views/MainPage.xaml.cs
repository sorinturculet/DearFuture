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
            _viewModel = viewModel; // Store reference to ViewModel
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
            if (e.Item is Capsule capsule)
            {
                string message = await _viewModel.OpenCapsuleAsync(capsule.Id);
                await DisplayAlert($"Capsule: {capsule.Title}", message, "OK");
            }
        }

        // Deletes the selected capsule when the delete button is clicked
        private async void OnDeleteCapsuleClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int capsuleId)
            {
                await _viewModel.DeleteCapsuleAsync(capsuleId);
            }
        }

        // Navigates to the Archived Capsules page when the archived button is clicked
        private async void OnViewArchivedCapsulesClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ArchivedCapsulesPage(_viewModel));
        }
    }
}
