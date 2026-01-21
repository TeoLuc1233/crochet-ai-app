using CrochetAI.Api.Models;

namespace CrochetAI.Api.Repositories;

public interface IAIGenerationRepository : IRepository<AIGeneration>
{
    Task<AIGeneration?> GetByImageHashAsync(string imageHash);
    Task<IEnumerable<AIGeneration>> GetByUserIdAsync(string userId);
    Task<int> GetMonthlyGenerationCountAsync(string userId, DateTime month);
}
