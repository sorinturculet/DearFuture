using System.Collections.Generic;
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

        public Task<List<Capsule>> GetCapsulesAsync()
        {
            return _database.Table<Capsule>().ToListAsync();
        }

        public Task<Capsule> GetCapsuleByIdAsync(int id)
        {
            return _database.Table<Capsule>().Where(c => c.Id == id).FirstOrDefaultAsync();
        }

        public Task<int> AddCapsuleAsync(Capsule capsule)
        {
            return _database.InsertAsync(capsule);
        }

        public Task<int> UpdateCapsuleAsync(Capsule capsule)
        {
            return _database.UpdateAsync(capsule);
        }

        public Task<int> DeleteCapsuleAsync(int id)
        {
            return _database.DeleteAsync<Capsule>(id);
        }
    }
}
