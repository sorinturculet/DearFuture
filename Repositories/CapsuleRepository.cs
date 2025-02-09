using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SQLite;
using DearFuture.Models;

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

        // Fetch only active (non-deleted) capsules
        public Task<List<Capsule>> GetCapsulesAsync()
        {
            return _database.Table<Capsule>().Where(c => !c.IsDeleted).ToListAsync();
        }

        public Task<Capsule> GetCapsuleByIdAsync(int id)
        {
            return _database.Table<Capsule>().Where(c => c.Id == id && !c.IsDeleted).FirstOrDefaultAsync();
        }

        public Task<int> AddCapsuleAsync(Capsule capsule)
        {
            return _database.InsertAsync(capsule);
        }

        public Task<int> UpdateCapsuleAsync(Capsule capsule)
        {
            return _database.UpdateAsync(capsule);
        }

        // Soft delete: move capsule to Trash instead of deleting it
        public async Task<int> DeleteCapsuleAsync(int id)
        {
            var capsule = await GetCapsuleByIdAsync(id);
            if (capsule != null)
            {
                capsule.IsDeleted = true;
                capsule.DeletedAt = DateTime.UtcNow;
                return await _database.UpdateAsync(capsule); // Update instead of Delete
            }
            return 0;
        }
        public Task<int> PermanentlyDeleteCapsuleAsync(int id)
        {
            return _database.DeleteAsync<Capsule>(id);
        }

        // Restore a capsule from Trash
        public async Task<int> RestoreCapsuleAsync(int id)
        {
            var capsule = await _database.Table<Capsule>().Where(c => c.Id == id && c.IsDeleted).FirstOrDefaultAsync();
            if (capsule != null)
            {
                capsule.IsDeleted = false;
                capsule.DeletedAt = null;
                return await _database.UpdateAsync(capsule);
            }
            return 0;
        }

        // Get capsules currently in the Trash
        public Task<List<Capsule>> GetDeletedCapsulesAsync()
        {
            return _database.Table<Capsule>().Where(c => c.IsDeleted).ToListAsync();
        }

        // Permanently delete capsules in Trash for more than 15 days
        public async Task<int> CleanupOldDeletedCapsulesAsync()
        {
            var thresholdDate = DateTime.UtcNow.AddDays(-15);
            var expiredCapsules = await _database.Table<Capsule>()
                .Where(c => c.IsDeleted && c.DeletedAt < thresholdDate)
                .ToListAsync();

            int count = 0;
            foreach (var capsule in expiredCapsules)
            {
                await _database.DeleteAsync<Capsule>(capsule.Id);
                count++;
            }
            return count;
        }
    }
}
