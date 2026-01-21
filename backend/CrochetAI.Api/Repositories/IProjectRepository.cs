using CrochetAI.Api.Models;

namespace CrochetAI.Api.Repositories;

public interface IProjectRepository : IRepository<Project>
{
    Task<IEnumerable<Project>> GetByUserIdAsync(string userId);
    Task<IEnumerable<Project>> GetByUserIdAndStatusAsync(string userId, string status);
    Task<Project?> GetByIdAndUserIdAsync(int id, string userId);
}
