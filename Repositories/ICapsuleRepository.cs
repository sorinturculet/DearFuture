using System.Collections.Generic;
using System.Threading.Tasks;
using DearFuture.Models;

namespace DearFuture.Repositories
{
    public interface ICapsuleRepository
    {
        // List retrieval methods
        Task<List<CapsulePreview>> GetActiveCapsulesAsync();
        Task<List<CapsulePreview>> GetArchivedCapsulesAsync();
        Task<List<CapsulePreview>> GetTrashedCapsulesAsync();
        
        // Message retrieval
        Task<string> GetMessageAsync(int capsuleId);
        
        // Creation
        Task<int> CreateAsync(Capsule capsule);
        
        // Status change operations
        Task MarkAsOpenedAsync(int capsuleId);
        Task MoveToArchiveAsync(int capsuleId);
        Task MoveToTrashAsync(int capsuleId);
        Task RestoreFromTrashAsync(int capsuleId);
        
        // Cleanup operations
        Task<int> CleanupOldTrashedCapsulesAsync(int daysThreshold = 30);
        Task<int> PermanentlyDeleteAsync(int capsuleId);

        Task<Capsule> GetCapsuleAsync(int id);
    }
}
