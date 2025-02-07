using DearFuture.ViewModels;
using DearFuture.Models;
using Microsoft.Maui.Controls;
using System;

namespace DearFuture.Views
{
    public partial class ArchivedCapsulesPage : ContentPage
    {
        private readonly MainViewModel _viewModel;

        public ArchivedCapsulesPage(MainViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        private async void OnArchivedCapsuleTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is Capsule capsule)
            {
                string message = capsule.GetMessage(); // Retrieve the stored message
                await DisplayAlert("Capsule Message", message, "OK");
            }
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync(); // Navigate back to MainPage
        }
    }
}
