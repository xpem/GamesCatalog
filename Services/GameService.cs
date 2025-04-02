using ApiRepo;
using Models;
using Models.DTOs;
using Models.Resps;
using Repo;

namespace Services
{
    public class GameService(IGameRepo GameRepo) : IGameService
    {
        public static readonly string ImagesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Inventory");

        public async Task<ServiceResp> CreateAsync(GameDTO game)
        {
            game.CreatedAt = game.UpdatedAt = DateTime.Now;

            //User id fixed to 1
            game.UserId = 1;

            await GameRepo.CreateAsync(game);

            return new ServiceResp(true);
        }

        public async Task<GameDTO?> GetByIGDBIdAsync(int igdbId) =>
             await GameRepo.GetByIGDBIdAsync(igdbId);

        public async Task UpdateStatusAsync(int id, GameStatus gameStatus, int? rate) =>
            await GameRepo.UpdateStatusAsync(id, DateTime.Now, gameStatus, rate);

        public List<TotalGroupedByStatus>? GetTotalsGroupedByStatus(int? uid = null)
        {
            int _uid = uid ?? 1;

            return GameRepo.GetTotalsGroupedByStatusAsync(_uid);
        }

        public async Task<List<GameDTO>> GetByStatusAsync(int? uid, GameStatus gameStatus, int page, string searchText)
        {
            int _uid = uid ?? 1;

            if (string.IsNullOrEmpty(searchText))
                return await GameRepo.GetByStatusAsync(_uid, gameStatus, page);
            else
                return await GameRepo.GetByStatusAsync(_uid, gameStatus, page, searchText);
        }

        public async Task InactivateAsync(int? uid, int id)
        {
            int _uid = uid ?? 1;

            await GameRepo.InactivateAsync(_uid, id, DateTime.Now);
        }
    }
}
