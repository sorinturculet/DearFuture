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
        public string Message { get; init; }
        public string Color { get; set; } = "#FFFFFF"; // Default: white
        public DateTime UnlockDate { get; set; }
        public string Category { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; } = DateTime.Now;

        public bool IsUnlocked => DateTime.Now >= UnlockDate;

        private bool _isOpened = false;
        public bool IsOpened
        {
            get => _isOpened;
            set
            {
                if (IsUnlocked)
                {
                    _isOpened = value;
                    OnPropertyChanged(nameof(IsOpened));
                }
            }
        }

        private string _timeRemaining;
        public string TimeRemaining
        {
            get => _timeRemaining;
            set
            {
                if (_timeRemaining != value)
                {
                    _timeRemaining = value;
                    OnPropertyChanged(nameof(TimeRemaining)); // ✅ Notify UI when updated
                }
            }
        }

        public string GetMessage()
        {
            return IsUnlocked ? Message : $" This capsule is locked! Come back on {UnlockDate:MMMM dd, yyyy}.";
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
