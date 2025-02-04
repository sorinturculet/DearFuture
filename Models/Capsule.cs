using System;
using SQLite;
namespace DearFuture.Models
{
    public class Capsule
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Color { get; set; } = "#FFFFFF"; //Default : white
        public DateTime UnlockDate { get; set; }
        public string Category { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; } = DateTime.Now;

        public bool IsUnlocked => DateTime.Now >= UnlockDate;
    }
}
