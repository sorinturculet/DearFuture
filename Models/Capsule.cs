using System;
using SQLite;

namespace DearFuture.Models
{
    public class Capsule
    {
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
                }
            }
        }

        // ✅ Directly bindable `TimeRemaining` property
        private string _timeRemaining;
        public string TimeRemaining
        {
            get
            {
                TimeSpan remaining = UnlockDate - DateTime.Now;
                return remaining.TotalSeconds > 0
                    ? $"{remaining.Days:D2}d:{remaining.Hours:D2}h:{remaining.Minutes:D2}m:{remaining.Seconds:D2}"
                    : "Unlocked!";
            }
            set
            {
                _timeRemaining = value;
            }
        }

        // Return message based on unlock state
        public string GetMessage()
        {
            return IsUnlocked ? Message : $" This capsule is locked! Come back on {UnlockDate:MMMM dd, yyyy}.";
        }
    }
}
