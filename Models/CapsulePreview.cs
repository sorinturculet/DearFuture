using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;

public class CapsulePreview : ObservableObject  // Inherit from ObservableObject for MVVM
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Color { get; set; } = "#FFFFFF";
    public DateTime UnlockDate { get; set; }
    public CapsuleStatus Status { get; set; }
    public bool IsOpened { get; set; }
    public DateTime DateCreated { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime? StatusChangedAt { get; set; }
    public bool HasLocation { get; set; }
    
    private string _timeRemaining = string.Empty;
    public string TimeRemaining
    {
        get => _timeRemaining;
        set => SetProperty(ref _timeRemaining, value);
    }
    
    public bool IsUnlocked => DateTime.Now >= UnlockDate;
} 