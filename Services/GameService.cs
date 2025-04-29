using ApiRepo;
using Models;
using Models.DTOs;
using Models.Resps;
using Repo;
using Services.Interfaces;

namespace Services
{
    public class GameService(IGameRepo GameRepo) : IGameService
    {
        public static readonly string ImagesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Inventory");

        public async Task<ServiceResp> CreateAsync(GameDTO game)
        {
            game.CreatedAt = game.UpdatedAt = DateTime.Now;

            await GameRepo.CreateAsync(game);

            return new ServiceResp(true);
        }

        public async Task<GameDTO?> GetByIGDBIdAsync(int igdbId, int uid) =>
             await GameRepo.GetByIGDBIdAsync(igdbId, uid);

        public async Task UpdateStatusAsync(int id, GameStatus gameStatus, int? rate) =>
            await GameRepo.UpdateStatusAsync(id, DateTime.Now, gameStatus, rate);

        public List<TotalGroupedByStatus>? GetTotalsGroupedByStatus(int uid)
        {
            return GameRepo.GetTotalsGroupedByStatusAsync(uid);
        }

        public async Task<List<GameDTO>> GetByStatusAsync(int uid, GameStatus gameStatus, int page, string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
                return await GameRepo.GetByStatusAsync(uid, gameStatus, page);
            else
                return await GameRepo.GetByStatusAsync(uid, gameStatus, page, searchText);
        }

        public async Task InactivateAsync(int uid, int id)
        {
            await GameRepo.InactivateAsync(uid, id, DateTime.Now);
        }
    }
}
