using Models;
using Models.DTOs;
using Models.Resps;

namespace Services
{
    public interface IGameService
    {
        Task<ServiceResp> CreateAsync(GameDTO game);
        Task<GameDTO?> GetByIGDBIdAsync(int igdbId);
        List<TotalGroupedByStatus>? GetTotalsGroupedByStatus(int? uid = null);
        Task UpdateStatusAsync(int id, GameStatus gameStatus, int? rate);
    }
}