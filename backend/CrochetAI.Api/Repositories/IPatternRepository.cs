using CrochetAI.Api.Models;

namespace CrochetAI.Api.Repositories;

public interface IPatternRepository : IRepository<Pattern>
{
    Task<IEnumerable<Pattern>> GetByDifficultyAsync(string difficulty);
    Task<IEnumerable<Pattern>> GetByCategoryAsync(string category);
    Task<IEnumerable<Pattern>> GetPremiumPatternsAsync();
    Task<IEnumerable<Pattern>> SearchAsync(string searchTerm);
    Task IncrementViewCountAsync(int patternId);
}
