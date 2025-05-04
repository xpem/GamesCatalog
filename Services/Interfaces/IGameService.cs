using Models;
using Models.DTOs;
using Models.Resps;

namespace Services.Interfaces
{
    public interface IGameService
    {
        Task<ServiceResp> CreateAsync(GameDTO game, bool isON);
        Task<GameDTO?> GetByIdAsync(int id, int uid);
        Task<GameDTO?> GetByIGDBIdAsync(int igdbId, int uid);
        Task<List<GameDTO>> GetByStatusAsync(int uid, GameStatus gameStatus, int page, string searchText);
        List<TotalGroupedByStatus>? GetTotalsGroupedByStatus(int uid);
        Task InactivateAsync(int uid, int id, bool isON);
        Task UpdateExternalIdAsync(int id, int externalid);
        Task UpdateStatusAsync(int id, GameStatus gameStatus, int? rate, bool isON);
    }
}
