using Models.ApiOperation;
using Models.DTOs;
using Models.Resps;
using Repo;
using Services.Interfaces;
using System.Text.Json;

namespace Services
{
    public static class GameSyncService
    {
        private const int PAGEMAX = 50;

        public static async Task ApiToLocalAsync(IGameApiService apiService, IGameService service, int uid, DateTime lastUpdate)
        {
            int page = 1;

            try
            {
                while (true)
                {
                    var apiList = await apiService.GetByLastUpdateAsync(lastUpdate, page);

                    if (apiList is null) break;

                    foreach (var apiRespObj in apiList)
                    {
                        if (apiRespObj is null) throw new ArgumentNullException(nameof(apiRespObj));

                        GameDTO apiGame = new()
                        {
                            Name = apiRespObj.Game.Name,
                            IGDBId = Convert.ToInt32(apiRespObj.Game.IGDBId),
                            ReleaseDate = apiRespObj.Game.ReleaseDate,
                            Platforms = apiRespObj.Game.Platforms,
                            Summary = apiRespObj.Game.Summary,
                            Rate = apiRespObj.Rate,
                            CreatedAt = apiRespObj.CreatedAt,
                            UpdatedAt = apiRespObj.UpdatedAt,
                            Inactive = apiRespObj.Inactive,
                            ExternalId = apiRespObj.Game.Id,
                            Status = apiRespObj.Status,
                            CoverUrl = apiRespObj.Game.CoverUrl,
                            UserId = uid,
                        };

                        var localGame = await service.GetByIGDBIdAsync(apiGame.IGDBId.Value, uid);

                        apiGame.Id = localGame?.Id ?? 0;

                        if (localGame == null && !apiGame.Inactive)
                        {
                            await IGDBGamesApiService.SaveImageAsync(apiRespObj.Game.CoverUrl, $"{apiRespObj.Game.IGDBId}.jpg");

                            await service.CreateAsync(apiGame, true);
                        }
                        else if (apiGame.UpdatedAt > localGame?.UpdatedAt)
                            await service.UpdateStatusAsync(localGame.Id, apiGame.Status, apiGame.Rate, true);
                    }

                    if (apiList.Count < PAGEMAX)
                        break;

                    page++;

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static async Task LocalToApiSync(IApiOperationService apiOperationService, IGameApiService apiService, IGameService gameService, int uid, DateTime lastUpdate)
        {
            List<ApiOperationDTO> pendingOperations = await apiOperationService.GetByStatusAsync(ApiOperationStatus.Pending);

            foreach (var pendingOperation in pendingOperations)
            {
                await apiOperationService.UpdateOperationStatusAsync(ApiOperationStatus.Processing, pendingOperation.Id);
                GameDTO? localGame;
                if (pendingOperation.ObjectType == ObjectType.Game)
                {

                    ServiceResp? apiServiceResp = null;

                    switch (pendingOperation.ExecutionType)
                    {
                        case ExecutionType.Insert:

                            localGame = JsonSerializer.Deserialize<GameDTO>(pendingOperation.Content);

                            apiServiceResp = await apiService.CreateAsync(localGame);

                            if (apiServiceResp.Success)
                            {
                                await gameService.UpdateExternalIdAsync(Convert.ToInt32(apiServiceResp.Content), uid);
                            }
                            else throw new Exception($"Is not possible sync the object: {pendingOperation.ObjectId}, res: {apiServiceResp.Content}");
                            break;
                        case ExecutionType.Update:

                            ApiOpGameStatus localGameStatus = JsonSerializer.Deserialize<ApiOpGameStatus>(pendingOperation.Content);

                            localGame = await gameService.GetByIdAsync(localGameStatus.Id, uid);

                            if (localGame.ExternalId is null)
                            {

                                GameDTO apiGame = new()
                                {
                                    Name = localGame.Name,
                                    IGDBId = Convert.ToInt32(localGame.IGDBId),
                                    ReleaseDate = localGame.ReleaseDate,
                                    Platforms = localGame.Platforms,
                                    Summary = localGame.Summary,
                                    Rate = localGame.Rate,
                                    CreatedAt = localGame.CreatedAt,
                                    UpdatedAt = localGame.UpdatedAt,
                                    Inactive = localGame.Inactive,
                                    ExternalId = localGame.IGDBId,
                                    Status = localGame.Status,
                                    CoverUrl = localGame.CoverUrl,
                                    UserId = uid,
                                };

                                var serviceResp = await apiService.CreateAsync(apiGame);

                                if (!serviceResp.Success) throw new Exception($"Is not possible sync the object: {pendingOperation.ObjectId}, res: {apiServiceResp.Content}");
                            }
                            else
                            {
                                apiServiceResp = await apiService.UpdateAsync(localGame.ExternalId.Value, localGameStatus.Status.Value, localGameStatus.Rate);

                                if (!apiServiceResp.Success) throw new Exception($"Is not possible sync the object: {pendingOperation.ObjectId}, res: {apiServiceResp.Content}");
                            }

                            break;
                        case ExecutionType.Delete:

                            int id = JsonSerializer.Deserialize<int>(pendingOperation.Content);

                            localGame = await gameService.GetByIdAsync(id, uid);

                            await apiService.InactivateAsync(localGame.ExternalId.Value);

                            break;
                    }
                }
                else throw new ArgumentException("Invalid ObjecType, Op Id: " + pendingOperation.Id);

                await apiOperationService.UpdateOperationStatusAsync(ApiOperationStatus.Success, pendingOperation.Id);
            }
        }
    }
}
