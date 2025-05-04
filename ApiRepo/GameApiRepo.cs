using Models;
using Models.DTOs;
using Models.Handlers;
using Models.Resps;
using System.Text.Json;

namespace ApiRepo
{
    public interface IGameApiRepo
    {
        Task<ApiResp> CreateAsync(GameDTO game);
        Task<ApiResp> GetByLastUpdateAsync(DateTime lastUpdate, int page);
        Task<ApiResp> InactivateAsync(int gameId);
        Task<ApiResp> UpdateAsync(int externalId, GameStatus status, int? rate);
    }

    public class GameApiRepo(IUserApiRepo userApiRepo) : IGameApiRepo
    {
        public async Task<ApiResp> CreateAsync(GameDTO game)
        {
            string json = JsonSerializer.Serialize(new
            {
                game.IGDBId,
                game.Name,
                game.ReleaseDate,
                game.Platforms,
                game.Summary,
                game.CoverUrl,
                game.Status,
                game.Rate
            });

            return await userApiRepo.AuthRequestAsync(RequestsTypes.Post, DeviceHandler.Url + "/game/status", jsonContent: json);
        }

        public async Task<ApiResp> UpdateAsync(int externalId, GameStatus status, int? rate)
        {
            string json = JsonSerializer.Serialize(new
            {
                Id = externalId,
                Status = status,
                Rate = rate
            });
            return await userApiRepo.AuthRequestAsync(RequestsTypes.Put, DeviceHandler.Url + "/game/status", jsonContent: json);
        }

        public async Task<ApiResp> InactivateAsync(int gameId)
        {
            return await userApiRepo.AuthRequestAsync(RequestsTypes.Delete, DeviceHandler.Url + "/game/status/" + gameId);
        }

        public async Task<ApiResp> GetByLastUpdateAsync(DateTime lastUpdate, int page)
        {
            return await userApiRepo.AuthRequestAsync(RequestsTypes.Get, DeviceHandler.Url + $"/game/status/byupdatedat/{lastUpdate:yyyy-MM-ddThh:mm:ss.fff}/{page}");
        }
    }
}
