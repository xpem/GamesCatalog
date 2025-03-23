using Models.DTOs;

namespace Repo
{
    public interface IGameRepo
    {
        Task CreateAsync(GameDTO game);
        Task<GameDTO?> GetByIGDBIdAsync(int igdbId);
        Task UpdateStatusAsync(int id, DateTime updatedAt, GameStatus status, int? rate);
    }
}