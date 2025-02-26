using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DearFuture.Models;
using DearFuture.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices.Sensors;

namespace DearFuture.ViewModels
{
    public class CreateCapsuleViewModel : INotifyPropertyChanged
    {
        private readonly CapsuleService _capsuleService;
        public IGeolocation geolocation;

        private string _title;
        private string _message;
        private DateTime _unlockDate = DateTime.Now.AddDays(1);
        private TimeSpan _unlockTime = DateTime.Now.TimeOfDay;
        private string _selectedColor;
        private string _selectedCategory;
        private bool _useLocation;
        private double? _latitude;
        private double? _longitude;
        private string _locationText = "Location not set";

        public event PropertyChangedEventHandler PropertyChanged;

        // Title of the capsule
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        // Message stored in the capsule
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        // Unlock date of the capsule
        public DateTime UnlockDate
        {
            get => _unlockDate;
            set => SetProperty(ref _unlockDate, value);
        }

        // Unlock time of the capsule
        public TimeSpan UnlockTime
        {
            get => _unlockTime;
            set => SetProperty(ref _unlockTime, value);
        }

        // Selected color for the capsule
        public string SelectedColor
        {
            get => _selectedColor;
            set => SetProperty(ref _selectedColor, value);
        }

        // Selected category of the capsule
        public string SelectedCategory
        {
            get => _selectedCategory;
            set => SetProperty(ref _selectedCategory, value);
        }

        // Available colors for selection
        public List<string> AvailableColors { get; } = new()
        {
            "#e74c3c", // Red
            "#3498db", // Blue
            "#2ecc71", // Green
            "#FFBD00", // Yellow
            "#CCCCFF", // Purple
            "#FF5400", // Orange
            "#F72585"  // Pink
        };

        // Available categories for selection
        public List<string> AvailableCategories { get; } = new()
        {
            "Event",
            "Reminder",
            "Reflection"
        };

        // Flag indicating whether the capsule uses location feature
        public bool UseLocation
        {
            get => _useLocation;
            set => SetProperty(ref _useLocation, value);
        }

        // Location text displayed in the UI
        public string LocationText
        {
            get => _locationText;
            set => SetProperty(ref _locationText, value);
        }

        public Command SetLocationCapsuleCommand { get; }
        public Command<string> SelectColorCommand { get; }
        public Command SaveCapsuleCommand { get; }

        public CreateCapsuleViewModel(CapsuleService capsuleService, IGeolocation geolocation)
        {
            _capsuleService = capsuleService;
            this.geolocation = geolocation;

            SetLocationCapsuleCommand = new Command(async () => await GetCurrentLocation());
            SelectColorCommand = new Command<string>(color => SelectedColor = color);
            SaveCapsuleCommand = new Command(async () => await SaveCapsuleAsync());

            // Set default values
            SelectedColor = AvailableColors[0];
            SelectedCategory = AvailableCategories[0];
        }

        // Fetch and store location only if enabled by the user
        private async Task GetCurrentLocation()
        {
            LocationText = "Getting your location...";
            try
            {
                var location = await geolocation.GetLastKnownLocationAsync();
                if (location == null)
                {
                    location = await geolocation.GetLocationAsync(
                        new GeolocationRequest
                        {
                            DesiredAccuracy = GeolocationAccuracy.Medium,
                            Timeout = TimeSpan.FromSeconds(30),
                        });

                    if (location == null)
                    {
                        LocationText = "Location unavailable.";
                        return;
                    }
                }

                _latitude = location.Latitude;
                _longitude = location.Longitude;
                LocationText = $"Location Set: {_latitude}, {_longitude}";
            }
            catch (Exception ex)
            {
                LocationText = "Error getting location.";
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        // Saves the capsule to the database with the selected properties
        public async Task SaveCapsuleAsync()
        {
            if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Message))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Title and Message are required.", "OK");
                return;
            }

            // Combine the selected date and time into a local DateTime
            var localUnlockDateTime = UnlockDate.Date + UnlockTime;
            
            // Convert to UTC for storage
            var utcUnlockDateTime = localUnlockDateTime.ToUniversalTime();

            // Validate that the unlock date is in the future
            if (utcUnlockDateTime <= DateTime.UtcNow)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Unlock date must be in the future.", "OK");
                return;
            }

            var capsule = new Capsule
            {
                Title = Title,
                Message = Message,
                UnlockDate = utcUnlockDateTime,  // Store in UTC
                Color = SelectedColor,
                Category = SelectedCategory,
                DateCreated = DateTime.UtcNow,
                Latitude = UseLocation ? _latitude : null,
                Longitude = UseLocation ? _longitude : null,
                Status = CapsuleStatus.Active,
                IsOpened = false
            };

            if (await _capsuleService.AddCapsuleAsync(capsule))
            {
                // Get the MainViewModel instance and refresh its data
                var mainViewModel = Application.Current.MainPage.Handler.MauiContext.Services.GetService<MainViewModel>();
                await mainViewModel?.LoadCapsulesCommand.ExecuteAsync(null);
                
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to save capsule.", "OK");
            }
        }

        // Notifies UI components when a property value changes
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
