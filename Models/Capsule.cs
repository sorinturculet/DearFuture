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
        public string Color { get; set; } = "#FFFFFF"; // Default : white
        public DateTime UnlockDate { get; set; }
        public string Category { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; } = DateTime.Now;

        // Quick functions to help with the capsule
        public TimeSpan TimeRemaining => UnlockDate > DateTime.Now ? UnlockDate - DateTime.Now : TimeSpan.Zero;
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

        // Return message based on unlock state
        public string GetMessage()
        {
            return IsUnlocked ? Message : $" This capsule is locked! Come back on {UnlockDate:MMMM dd, yyyy}.";
        }
    }
}
