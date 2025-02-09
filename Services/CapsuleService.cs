using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DearFuture.Models;
using DearFuture.Repositories;

namespace DearFuture.Services
{
    public class CapsuleService
    {
        private readonly ICapsuleRepository _capsuleRepository;

        public CapsuleService(ICapsuleRepository capsuleRepository)
        {
            _capsuleRepository = capsuleRepository;
        }

        // Retrieves all locked capsules, applying optional filtering and sorting
        public async Task<List<Capsule>> GetLockedCapsulesAsync(string category = null, string sortOption = "")
        {
            var capsules = await _capsuleRepository.GetCapsulesAsync();
            capsules = capsules.Where(c => !c.IsOpened).ToList();

            // Apply filtering if a category is specified
            if (!string.IsNullOrEmpty(category) && category != "All")
            {
                capsules = capsules.Where(c => c.Category == category).ToList();
            }

            // Apply sorting based on the selected option
            return sortOption switch
            {
                "Date Created (Newest)" => capsules.OrderByDescending(c => c.DateCreated).ToList(),
                "Date Created (Oldest)" => capsules.OrderBy(c => c.DateCreated).ToList(),
                "Name (A-Z)" => capsules.OrderBy(c => c.Title).ToList(),
                "Name (Z-A)" => capsules.OrderByDescending(c => c.Title).ToList(),
                "Unlock Date (Soonest)" => capsules.OrderBy(c => c.UnlockDate).ToList(),
                "Unlock Date (Latest)" => capsules.OrderByDescending(c => c.UnlockDate).ToList(),
                _ => capsules
            };
        }

        // Retrieves all archived (opened) capsules
        public async Task<List<Capsule>> GetArchivedCapsulesAsync()
        {
            var capsules = await _capsuleRepository.GetCapsulesAsync();
            return capsules.Where(c => c.IsOpened).ToList();
        }

        // Opens a capsule if it is unlocked and returns its message
        public async Task<string> OpenCapsuleAsync(int id)
        {
            var capsule = await _capsuleRepository.GetCapsuleByIdAsync(id);
            if (capsule == null || !capsule.IsUnlocked)
                return "This capsule is locked!";

            capsule.IsOpened = true;
            await _capsuleRepository.UpdateCapsuleAsync(capsule);
            return capsule.GetMessage();
        }

        // Retrieves a capsule by its ID
        public Task<Capsule> GetCapsuleByIdAsync(int id)
        {
            return _capsuleRepository.GetCapsuleByIdAsync(id);
        }

        // Adds a new capsule if it passes validation
        public async Task<bool> AddCapsuleAsync(Capsule capsule)
        {
            if (string.IsNullOrWhiteSpace(capsule.Title) || capsule.UnlockDate <= DateTime.Now)
            {
                return false; // Validation failed
            }

            await _capsuleRepository.AddCapsuleAsync(capsule);
            return true;
        }

        // Updates an existing capsule in the database
        public Task<int> UpdateCapsuleAsync(Capsule capsule)
        {
            return _capsuleRepository.UpdateCapsuleAsync(capsule);
        }

        // Moves a capsule to Trash (soft delete)
        public Task<int> DeleteCapsuleAsync(int id)
        {
            return _capsuleRepository.DeleteCapsuleAsync(id);
        }
        // Permanently deletes a capsule from the database
        public Task<int> PermanentlyDeleteCapsuleAsync(int id)
        {
            return _capsuleRepository.PermanentlyDeleteCapsuleAsync(id);
        }

        // Retrieves all deleted (trashed) capsules
        public Task<List<Capsule>> GetDeletedCapsulesAsync()
        {
            return _capsuleRepository.GetDeletedCapsulesAsync();
        }


        // Restores a capsule from Trash
        public Task<int> RestoreCapsuleAsync(int id)
        {
            return _capsuleRepository.RestoreCapsuleAsync(id);
        }

        // Permanently deletes capsules that have been in Trash for more than 15 days
        public Task<int> CleanupOldDeletedCapsulesAsync()
        {
            return _capsuleRepository.CleanupOldDeletedCapsulesAsync();
        }

        // Calculates the remaining time before a capsule can be unlocked
        public string GetTimeRemaining(Capsule capsule)
        {
            TimeSpan remaining = capsule.UnlockDate - DateTime.Now;
            return remaining.TotalSeconds > 0
                ? $"{remaining.Days:D2}d:{remaining.Hours:D2}h:{remaining.Minutes:D2}m"
                : "Unlocked!";
        }
    }
}
