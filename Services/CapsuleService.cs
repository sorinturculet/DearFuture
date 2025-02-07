using System.Collections.Generic;
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

        public async Task<List<Capsule>> GetCapsulesAsync(string category = null, string sortOption = "")
        {
            var capsules = await _capsuleRepository.GetCapsulesAsync();

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