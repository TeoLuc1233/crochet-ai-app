using CrochetAI.Api.Data;
using CrochetAI.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CrochetAI.Api.Repositories;

public class PatternRepository : Repository<Pattern>, IPatternRepository
{
    public PatternRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Pattern>> GetByDifficultyAsync(string difficulty)
    {
        return await _dbSet
            .Where(p => p.Difficulty == difficulty)
            .ToListAsync();
    }

    public async Task<IEnumerable<Pattern>> GetByCategoryAsync(string category)
    {
        return await _dbSet
            .Where(p => p.Category == category)
            .ToListAsync();
    }

    public async Task<IEnumerable<Pattern>> GetPremiumPatternsAsync()
    {
        return await _dbSet
            .Where(p => p.IsPremium)
            .ToListAsync();
    }

    public async Task<IEnumerable<Pattern>> SearchAsync(string searchTerm)
    {
        var term = searchTerm.ToLower();
        return await _dbSet
            .Where(p => p.Title.ToLower().Contains(term) || 
                       (p.Description != null && p.Description.ToLower().Contains(term)))
            .ToListAsync();
    }

    public async Task IncrementViewCountAsync(int patternId)
    {
        var pattern = await _dbSet.FindAsync(patternId);
        if (pattern != null)
        {
            pattern.ViewCount++;
            await _context.SaveChangesAsync();
        }
    }
}
