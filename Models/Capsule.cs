using System;
using System.ComponentModel;
using SQLite;

namespace DearFuture.Models
{
    public class Capsule : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Color { get; set; } = "#FFFFFF";
        public DateTime UnlockDate { get; set; }
        public string Category { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; } = DateTime.Now;

        // Determines if the capsule can be unlocked
        public bool IsUnlocked => DateTime.Now >= UnlockDate;

        // Replace IsDeleted and DeletedAt with these fields
        public CapsuleStatus Status { get; set; } = CapsuleStatus.Active;
        public DateTime? StatusChangedAt { get; set; }
        public bool IsOpened { get; set; } = false;

        // Time remaining until the capsule can be unlocked
        private string _timeRemaining;
        public string TimeRemaining
        {
            get => _timeRemaining;
            set
            {
                if (_timeRemaining != value)
                {
                    _timeRemaining = value;
                    OnPropertyChanged(nameof(TimeRemaining));
                }
            }
        }

        // Stores GPS coordinates if the capsule is location-based
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool HasLocation => Latitude.HasValue && Longitude.HasValue;

        public string GetMessage()
        {
            return IsUnlocked ? Message : $"This capsule is locked! Come back on {UnlockDate:MMMM dd, yyyy}.";
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
