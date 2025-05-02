using Models;
using Models.ApiOperation;
using Models.DTOs;
using Models.Resps;
using Repo;
using Services.Interfaces;
using System.Text.Json;

namespace Services
{
    public class GameService(IGameRepo GameRepo, IGameApiService gameApiService, IApiOperationService apiOperationService) : IGameService
    {
        public static readonly string ImagesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Inventory");

        public async Task<ServiceResp> CreateAsync(GameDTO game, bool isON)
        {
            game.CreatedAt = game.UpdatedAt = DateTime.Now;

            await GameRepo.CreateAsync(game);

            if (isON)
            {
                var apiResp = await gameApiService.CreateAsync(game);

                if (apiResp is not null && apiResp.Success && apiResp.Content is not null)
                {
                    int externalId = (apiResp.Content as int?).Value;
                    await GameRepo.UpdateExternalIdAsync(game.Id, externalId);
                }
                else
                {
                    if (apiResp?.Content is not null and string)
                        return new ServiceResp(false, apiResp.Content);
                    else
                        return new ServiceResp(false, null);
                }
            }
            else
                await apiOperationService.InsertOperationAsync(JsonSerializer.Serialize(game), game.Id.ToString(), ExecutionType.Insert, ObjectType.Game);

            return new ServiceResp(true);
        }

        public async Task<GameDTO?> GetByIGDBIdAsync(int igdbId, int uid) =>
             await GameRepo.GetByIGDBIdAsync(igdbId, uid);

        public async Task UpdateStatusAsync(int id, GameStatus gameStatus, int? rate, bool isON)
        {
            await GameRepo.UpdateStatusAsync(id, DateTime.Now, gameStatus, rate);

            if (isON)
            {
                _ = gameApiService.UpdateAsync(id, gameStatus, rate);
            }
            else await apiOperationService.InsertOperationAsync(JsonSerializer.Serialize(new ApiOpGameStatus(id, gameStatus, rate)),
                id.ToString(), ExecutionType.Update, ObjectType.Game);

        }

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

        public async Task InactivateAsync(int uid, int id, bool isON)
        {
            await GameRepo.InactivateAsync(uid, id, DateTime.Now);

            if (isON)
            {
                _ = gameApiService.InactivateAsync(id);
            }
            else await apiOperationService.InsertOperationAsync(null, id.ToString(), ExecutionType.Delete, ObjectType.Game);

        }
    }
}
