﻿using System.Collections.Generic;
using System.Threading.Tasks;
using DearFuture.Models;

namespace DearFuture.Repositories
{
    public interface ICapsuleRepository
    {
        Task<List<Capsule>> GetCapsulesAsync();
        Task<Capsule> GetCapsuleByIdAsync(int id);
        Task<int> AddCapsuleAsync(Capsule capsule);
        Task<int> UpdateCapsuleAsync(Capsule capsule);
        Task<int> DeleteCapsuleAsync(int id);
    }
}
