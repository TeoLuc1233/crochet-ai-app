using CrochetAI.Api.Data;
using CrochetAI.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CrochetAI.Api.Repositories;

public class AIGenerationRepository : Repository<AIGeneration>, IAIGenerationRepository
{
    public AIGenerationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<AIGeneration?> GetByImageHashAsync(string imageHash)
    {
        return await _dbSet
            .FirstOrDefaultAsync(a => a.ImageHash == imageHash);
    }

    public async Task<IEnumerable<AIGeneration>> GetByUserIdAsync(string userId)
    {
        return await _dbSet
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> GetMonthlyGenerationCountAsync(string userId, DateTime month)
    {
        var startDate = new DateTime(month.Year, month.Month, 1);
        var endDate = startDate.AddMonths(1);

        return await _dbSet
            .CountAsync(a => a.UserId == userId && 
                           a.CreatedAt >= startDate && 
                           a.CreatedAt < endDate);
    }
}
