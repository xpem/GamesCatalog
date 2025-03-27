using ApiRepo;
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
            game.CreatedAt = DateTime.Now;

            //User id fixed to 1
            game.UserId = 1;

            await GameRepo.CreateAsync(game);

            return new ServiceResp(true);
        }

        public async Task<GameDTO?> GetByIGDBIdAsync(int igdbId) =>
             await GameRepo.GetByIGDBIdAsync(igdbId);

        public async Task UpdateStatusAsync(int id, GameStatus gameStatus, int? rate) =>
            await GameRepo.UpdateStatusAsync(id, DateTime.Now, gameStatus, rate);

      
  
    }
}
