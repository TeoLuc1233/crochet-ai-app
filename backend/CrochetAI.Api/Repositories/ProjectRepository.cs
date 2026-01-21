using CrochetAI.Api.Data;
using CrochetAI.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CrochetAI.Api.Repositories;

public class ProjectRepository : Repository<Project>, IProjectRepository
{
    public ProjectRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Project>> GetByUserIdAsync(string userId)
    {
        return await _dbSet
            .Where(p => p.UserId == userId)
            .Include(p => p.Pattern)
            .ToListAsync();
    }

    public async Task<IEnumerable<Project>> GetByUserIdAndStatusAsync(string userId, string status)
    {
        return await _dbSet
            .Where(p => p.UserId == userId && p.Status == status)
            .Include(p => p.Pattern)
            .ToListAsync();
    }

    public async Task<Project?> GetByIdAndUserIdAsync(int id, string userId)
    {
        return await _dbSet
            .Include(p => p.Pattern)
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
    }
}
