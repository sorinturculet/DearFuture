using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DearFuture.Models;
using DearFuture.Repositories;
using Microsoft.EntityFrameworkCore;

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
        public async Task<List<CapsulePreview>> GetLockedCapsulesAsync(string category = null, string sortOption = "")
        {
            var capsules = await _capsuleRepository.GetActiveCapsulesAsync();

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
        public Task<List<CapsulePreview>> GetArchivedCapsulesAsync()
        {
            return _capsuleRepository.GetArchivedCapsulesAsync();
        }

        // Attempts to open a capsule if it is unlocked and meets the location requirement
        public async Task<(bool success, string message)> TryOpenCapsuleAsync(int id, IGeolocation geolocation)
        {
            var capsule = await _capsuleRepository.GetCapsuleAsync(id);
            if (capsule == null)
                return (false, "Capsule not found.");

            // Check if capsule requires location validation
            if (capsule.HasLocation)
            {
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
                            return (false, "Unable to get your location.");
                    }

                    // Calculate distance between user and capsule location
                    var distance = location.CalculateDistance(
                        capsule.Latitude.Value, 
                        capsule.Longitude.Value, 
                        DistanceUnits.Kilometers);

                    // Capsule can only be opened if user is within 1 km
                    if (distance > 1)
                        return (false, "You must be within 1km of the capsule location to open it.");
                }
                catch (Exception)
                {
                    return (false, "Location services are not available.");
                }
            }

            // If we get here, location check passed or wasn't required
            var message = await _capsuleRepository.GetMessageAsync(id);
            if (string.IsNullOrEmpty(message))
                return (false, "Unable to retrieve capsule message.");

            await _capsuleRepository.MarkAsOpenedAsync(id);
            return (true, message);
        }

        // Adds a new capsule if it passes validation
        public async Task<bool> AddCapsuleAsync(Capsule capsule)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(capsule.Title) || 
                    string.IsNullOrWhiteSpace(capsule.Message) || 
                    capsule.UnlockDate <= DateTime.UtcNow)
                {
                    return false; // Validation failed
                }

                await _capsuleRepository.CreateAsync(capsule);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Moves a capsule to Trash
        public Task MoveCapsuleToTrashAsync(int id)
        {
            return _capsuleRepository.MoveToTrashAsync(id);
        }

        // Retrieves all trashed capsules
        public Task<List<CapsulePreview>> GetTrashedCapsulesAsync()
        {
            return _capsuleRepository.GetTrashedCapsulesAsync();
        }

        // Restores a capsule from Trash
        public Task RestoreCapsuleAsync(int id)
        {
            return _capsuleRepository.RestoreFromTrashAsync(id);
        }

        // Permanently deletes a capsule
        public Task DeletePermanentlyAsync(int id)
        {
            return _capsuleRepository.PermanentlyDeleteAsync(id);
        }

        // Cleanup old trashed capsules
        public Task<int> CleanupOldTrashedCapsulesAsync()
        {
            return _capsuleRepository.CleanupOldTrashedCapsulesAsync();
        }

        // Calculates the remaining time before a capsule can be unlocked
        public string GetTimeRemaining(CapsulePreview capsule)
        {
            // Convert UTC unlock date to local time for display
            var localUnlockDate = capsule.UnlockDate.ToLocalTime();
            TimeSpan remaining = localUnlockDate - DateTime.Now;
            
            return remaining.TotalSeconds > 0
                ? $"{remaining.Days:D2}d:{remaining.Hours:D2}h:{remaining.Minutes:D2}m:{remaining.Seconds:D2}s"
                : "Unlocked!";
        }

        public async Task<string> GetMessageAsync(int capsuleId)
        {
            return await _capsuleRepository.GetMessageAsync(capsuleId);
        }
    }
}
