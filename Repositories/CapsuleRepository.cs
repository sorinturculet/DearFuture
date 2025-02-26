using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SQLite;
using DearFuture.Models;
using SQLiteNetExtensions.Extensions;

namespace DearFuture.Repositories
{
    public class CapsuleRepository : ICapsuleRepository
    {
        private readonly SQLiteAsyncConnection _database;

        public CapsuleRepository(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Capsule>().Wait();
        }

        private static CapsulePreview ToCapsulePreview(Capsule capsule)
        {
            return new CapsulePreview
            {
                Id = capsule.Id,
                Title = capsule.Title,
                Color = capsule.Color,
                UnlockDate = capsule.UnlockDate,
                Status = capsule.Status,
                IsOpened = capsule.IsOpened,
                DateCreated = capsule.DateCreated,
                Category = capsule.Category,
                StatusChangedAt = capsule.StatusChangedAt,
                HasLocation = capsule.HasLocation
            };
        }

        public async Task<List<CapsulePreview>> GetActiveCapsulesAsync()
        {
            var capsules = await _database.Table<Capsule>()
                .Where(c => c.Status == CapsuleStatus.Active)
                .ToListAsync();

            return capsules.Select(ToCapsulePreview).ToList();
        }

        public async Task<List<CapsulePreview>> GetArchivedCapsulesAsync()
        {
            var capsules = await _database.Table<Capsule>()
                .Where(c => c.Status == CapsuleStatus.Archived)
                .ToListAsync();

            return capsules.Select(ToCapsulePreview).ToList();
        }

        public async Task<List<CapsulePreview>> GetTrashedCapsulesAsync()
        {
            var capsules = await _database.Table<Capsule>()
                .Where(c => c.Status == CapsuleStatus.Trashed)
                .ToListAsync();

            return capsules.Select(ToCapsulePreview).ToList();
        }

        public async Task<string> GetMessageAsync(int capsuleId)
        {
            var capsule = await _database.Table<Capsule>()
                .Where(c => c.Id == capsuleId)
                .FirstOrDefaultAsync();
            
            return capsule?.Message ?? string.Empty;
        }

        public async Task<int> CreateAsync(Capsule capsule)
        {
            capsule.DateCreated = DateTime.UtcNow;
            capsule.Status = CapsuleStatus.Active;
            capsule.StatusChangedAt = DateTime.UtcNow;
            return await _database.InsertAsync(capsule);
        }

        public async Task MarkAsOpenedAsync(int capsuleId)
        {
            var capsule = await _database.GetAsync<Capsule>(capsuleId);
            if (capsule != null)
            {
                capsule.IsOpened = true;
                capsule.Status = CapsuleStatus.Archived;
                capsule.StatusChangedAt = DateTime.UtcNow;
                await _database.UpdateAsync(capsule);
            }
        }

        public async Task MoveToArchiveAsync(int capsuleId)
        {
            var capsule = await _database.GetAsync<Capsule>(capsuleId);
            if (capsule != null)
            {
                capsule.Status = CapsuleStatus.Archived;
                capsule.StatusChangedAt = DateTime.UtcNow;
                await _database.UpdateAsync(capsule);
            }
        }

        public async Task MoveToTrashAsync(int capsuleId)
        {
            var capsule = await _database.GetAsync<Capsule>(capsuleId);
            if (capsule != null)
            {
                capsule.Status = CapsuleStatus.Trashed;
                capsule.StatusChangedAt = DateTime.UtcNow;
                await _database.UpdateAsync(capsule);
            }
        }

        public async Task RestoreFromTrashAsync(int capsuleId)
        {
            var capsule = await _database.GetAsync<Capsule>(capsuleId);
            if (capsule != null)
            {
                capsule.Status = capsule.IsOpened ? CapsuleStatus.Archived : CapsuleStatus.Active;
                capsule.StatusChangedAt = DateTime.UtcNow;
                await _database.UpdateAsync(capsule);
            }
        }

        public async Task<int> CleanupOldTrashedCapsulesAsync(int daysThreshold = 30)
        {
            var thresholdDate = DateTime.UtcNow.AddDays(-daysThreshold);
            var expiredCapsules = await _database.Table<Capsule>()
                .Where(c => c.Status == CapsuleStatus.Trashed)
                .ToListAsync();

            // Filter in memory since SQLite doesn't support DateTime comparisons well
            var capsulesToDelete = expiredCapsules
                .Where(c => c.StatusChangedAt < thresholdDate)
                .ToList();

            int count = 0;
            foreach (var capsule in capsulesToDelete)
            {
                await _database.DeleteAsync(capsule);
                count++;
            }
            return count;
        }

        public Task<int> PermanentlyDeleteAsync(int capsuleId)
        {
            return _database.DeleteAsync<Capsule>(capsuleId);
        }

        public Task<Capsule> GetCapsuleAsync(int id)
        {
            return _database.Table<Capsule>()
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync();
        }
    }
}
