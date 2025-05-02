using Models;
using Models.DTOs;

namespace Repo
{
    public interface IGameRepo
    {
        Task CreateAsync(GameDTO game);
        Task<GameDTO?> GetByIGDBIdAsync(int igdbId, int uid);
        Task UpdateStatusAsync(int id, DateTime updatedAt, GameStatus status, int? rate);

        List<TotalGroupedByStatus>? GetTotalsGroupedByStatusAsync(int uid);
        Task<List<GameDTO>> GetByStatusAsync(int uid, GameStatus gameStatus, int page);
        Task InactivateAsync(int uid, int id, DateTime updatedAt);
        Task<List<GameDTO>> GetByStatusAsync(int uid, GameStatus gameStatus, int page, string searchText);
        Task UpdateExternalIdAsync(int id, int externalid);
    }
}