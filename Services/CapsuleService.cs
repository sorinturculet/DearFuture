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

        // ✅ Get locked capsules with filtering and sorting
        public async Task<List<Capsule>> GetLockedCapsulesAsync(string category = null, string sortOption = "")
        {
            var capsules = await _capsuleRepository.GetCapsulesAsync();
            capsules = capsules.Where(c => !c.IsOpened).ToList();

            // 🔥 Apply filtering
            if (!string.IsNullOrEmpty(category) && category != "All")
            {
                capsules = capsules.Where(c => c.Category == category).ToList();
            }

            // 🔥 Apply sorting
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

        // ✅ Get all archived (opened) capsules
        public async Task<List<Capsule>> GetArchivedCapsulesAsync()
        {
            var capsules = await _capsuleRepository.GetCapsulesAsync();
            return capsules.Where(c => c.IsOpened).ToList();
        }

        // ✅ Open a capsule and return its message
        public async Task<string> OpenCapsuleAsync(int id)
        {
            var capsule = await _capsuleRepository.GetCapsuleByIdAsync(id);
            if (capsule == null || !capsule.IsUnlocked)
                return "This capsule is still locked!";

            capsule.IsOpened = true;
            await _capsuleRepository.UpdateCapsuleAsync(capsule);
            return capsule.GetMessage(); // Return the message after unlocking
        }

        public Task<Capsule> GetCapsuleByIdAsync(int id)
        {
            return _capsuleRepository.GetCapsuleByIdAsync(id);
        }

        public async Task<bool> AddCapsuleAsync(Capsule capsule)
        {
            if (string.IsNullOrWhiteSpace(capsule.Title) || capsule.UnlockDate <= DateTime.Now)
            {
                return false; // Validation failed
            }

            await _capsuleRepository.AddCapsuleAsync(capsule);
            return true;
        }

        public Task<int> UpdateCapsuleAsync(Capsule capsule)
        {
            return _capsuleRepository.UpdateCapsuleAsync(capsule);
        }

        public Task<int> DeleteCapsuleAsync(int id)
        {
            return _capsuleRepository.DeleteCapsuleAsync(id);
        }
    }
}
