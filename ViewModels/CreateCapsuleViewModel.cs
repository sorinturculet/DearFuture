using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DearFuture.Models;
using DearFuture.Services;
using DearFuture.Observers;
using Microsoft.Maui.Controls;

namespace DearFuture.ViewModels
{
    public class CreateCapsuleViewModel : INotifyPropertyChanged
    {
        private readonly CapsuleService _capsuleService;
        private string _title;
        private string _message;
        private DateTime _unlockDate = DateTime.Now.AddDays(1);
        private TimeSpan _unlockTime = DateTime.Now.TimeOfDay;
        private string _selectedColor;
        private string _selectedCategory;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public DateTime UnlockDate
        {
            get => _unlockDate;
            set => SetProperty(ref _unlockDate, value);
        }

        public TimeSpan UnlockTime
        {
            get => _unlockTime;
            set => SetProperty(ref _unlockTime, value);
        }

        public string SelectedColor
        {
            get => _selectedColor;
            set => SetProperty(ref _selectedColor, value);
        }

        public string SelectedCategory
        {
            get => _selectedCategory;
            set => SetProperty(ref _selectedCategory, value);
        }

        public List<string> AvailableColors { get; } = new()
        {
            "#e74c3c", // Red
            "#3498db", // Blue
            "#2ecc71", // Green
            "#FFBD00", // Yellow
            "#390099", // Purple
            "#FF5400",  // Orange
            "#F72585"  // Pink
        };

        public List<string> AvailableCategories { get; } = new()
        {
            "Event",
            "Reminder",
            "Reflection"
        };

        public Command<string> SelectColorCommand { get; }
        public Command SaveCapsuleCommand { get; }

        public CreateCapsuleViewModel(CapsuleService capsuleService)
        {
            _capsuleService = capsuleService;
            SelectColorCommand = new Command<string>(color => SelectedColor = color);
            SaveCapsuleCommand = new Command(async () => await SaveCapsuleAsync());

            // Set default values
            SelectedColor = AvailableColors[0];
            SelectedCategory = AvailableCategories[0];
        }

        public async Task SaveCapsuleAsync()
        {
            if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Message))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Title and Message are required.", "OK");
                return;
            }

            // Combine Date and Time into one DateTime value
            DateTime unlockDateTime = UnlockDate.Date + UnlockTime;

            var capsule = new Capsule
            {
                Title = Title,
                Message = Message,
                UnlockDate = unlockDateTime,
                Color = SelectedColor,
                Category = SelectedCategory,
                DateCreated = DateTime.Now // Track when the capsule was created
            };

            if (await _capsuleService.AddCapsuleAsync(capsule))
            {
                CapsuleObservable.NotifyObservers();
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to save capsule.", "OK");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
