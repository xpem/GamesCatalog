using Models;
using Models.DTOs;
using Models.Resps;

namespace Services.Interfaces
{
    public interface IGameService
    {
        Task<ServiceResp> CreateAsync(GameDTO game);
        Task<GameDTO?> GetByIGDBIdAsync(int igdbId);
        Task<List<GameDTO>> GetByStatusAsync(int? uid, GameStatus gameStatus, int page, string searchText);
        List<TotalGroupedByStatus>? GetTotalsGroupedByStatus(int? uid = null);
        Task InactivateAsync(int? uid, int id);
        Task UpdateStatusAsync(int id, GameStatus gameStatus, int? rate);
    }
}