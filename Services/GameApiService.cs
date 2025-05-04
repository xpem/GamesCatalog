using ApiRepo;
using Models.DTOs;
using Models.Resps;
using Models.Resps.Api;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Services
{
    public interface IGameApiService
    {
        Task<ServiceResp> CreateAsync(GameDTO game);
        Task<List<GameStatusApiResp>?> GetByLastUpdateAsync(DateTime lastUpdate, int page);
        Task<ServiceResp> InactivateAsync(int gameId);
        Task<ServiceResp> UpdateAsync(int externalId, GameStatus status, int? rate);
    }

    public class GameApiService(IGameApiRepo gameApiRepo) : IGameApiService
    {
        public async Task<ServiceResp> CreateAsync(GameDTO game)
        {
            ApiResp apiResp = await gameApiRepo.CreateAsync(game);

            return BuildGameStatusIdResp(apiResp);
        }

        public async Task<ServiceResp> UpdateAsync(int externalId, GameStatus status, int? rate)
        {
            ApiResp apiResp = await gameApiRepo.UpdateAsync(externalId, status, rate);
            return BuildGameStatusIdResp(apiResp);
        }

        public async Task<ServiceResp> InactivateAsync(int gameId)
        {
            ApiResp apiResp = await gameApiRepo.InactivateAsync(gameId);
            return BuildGameStatusIdResp(apiResp);
        }

        private static ServiceResp BuildGameStatusIdResp(ApiResp apiResp)
        {
            if (apiResp is not null)
            {
                if (apiResp.Success && apiResp.Content is not null)
                {
                    JsonNode? jResp = JsonNode.Parse(apiResp.Content);
                    if (jResp is not null)
                    {
                        int? id = null;

                        if (jResp != null)
                            id = jResp["gameStatusId"]?.GetValue<int>();

                        return new ServiceResp(apiResp.Success, id);
                    }
                    else return new ServiceResp(false, apiResp.Content);
                }
                else
                {
                    if (apiResp.Content is not null)
                    {
                        JsonNode? jResp = JsonNode.Parse(apiResp.Content);
                        if (jResp is not null)
                        {
                            string? error = jResp["error"]?.GetValue<string>();
                            return new ServiceResp(false, error);
                        }
                    }
                }
            }
            return new ServiceResp(false, null);
        }

        public async Task<List<GameStatusApiResp>?> GetByLastUpdateAsync(DateTime lastUpdate, int page)
        {
            ApiResp apiResp = await gameApiRepo.GetByLastUpdateAsync(lastUpdate, page);

            if (apiResp is not null && apiResp.Success && apiResp.Content is not null)
            {
                return JsonSerializer.Deserialize<List<GameStatusApiResp>?>(apiResp.Content);
            }
            else
            {
                if (apiResp?.Content is not null and string)
                    throw new Exception($"Error getting data from API: {apiResp.Content}");
                else
                    throw new Exception("Error getting data from API");
            }
        }
    }
}
