using Models.ApiOperation;
using Models.DTOs;
using Models.Resps;
using Models.Resps.Api;
using Moq;
using Repo;
using Services;
using Services.Interfaces;
using System.Text.Json;

namespace ServicesTests
{
    [TestClass()]
    public class GameSyncServiceTests
    {
        [TestMethod()]
        public async Task ApiToLocalAsync_ShouldSyncGamesCorrectly()
        {
            // Arrange
            var mockApiService = new Mock<IGameApiService>();
            var mockGameService = new Mock<IGameService>();
            int uid = 1;
            DateTime lastUpdate = DateTime.UtcNow.AddDays(-1);

            var apiResponse = new List<GameStatusApiResp>
        {
            new() {
                Game = new GameApiResp
                {
                    Name = "Test Game",
                    IGDBId = 123,
                    ReleaseDate = "2023-01-01",
                    Platforms = "PC",
                    Summary = "Test Summary",
                    Id = 1,
                    CoverUrl = "http://example.com/cover.jpg"
                },
                Rate = 5,
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                UpdatedAt = DateTime.UtcNow.AddDays(-1),
                Inactive = false,
                Status = GameStatus.Playing
            }
        };

            mockApiService
                .Setup(api => api.GetByLastUpdateAsync(lastUpdate, It.IsAny<int>()))
                .ReturnsAsync(apiResponse);

            mockGameService
                .Setup(service => service.GetByIGDBIdAsync(123, uid))
                .ReturnsAsync((GameDTO?)null);

            // Act
            await GameSyncService.ApiToLocalAsync(mockApiService.Object, mockGameService.Object, uid, lastUpdate);

            // Assert
            mockGameService.Verify(service => service.CreateAsync(It.Is<GameDTO>(g =>
                g.Name == "Test Game" &&
                g.IGDBId == 123 &&
                g.ReleaseDate == "2023-01-01" &&
                g.Platforms == "PC" &&
                g.Summary == "Test Summary" &&
                g.CoverUrl == "http://example.com/cover.jpg" &&
                g.Rate == 5 &&
                g.Status == GameStatus.Playing &&
                g.UserId == uid
            ), true), Times.Once);

            mockApiService.Verify(api => api.GetByLastUpdateAsync(lastUpdate, It.IsAny<int>()), Times.AtLeastOnce);
        }

        [TestMethod()]
        public async Task ApiToLocalAsync_ShouldCallUpdateStatusAsync_WhenLocalGameExistsAndApiGameIsUpdated()
        {
            // Arrange
            var mockApiService = new Mock<IGameApiService>();
            var mockGameService = new Mock<IGameService>();
            int uid = 1;
            DateTime lastUpdate = DateTime.UtcNow.AddDays(-1);

            var apiGameResp = new GameStatusApiResp
            {
                Id = 1,
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                UpdatedAt = DateTime.UtcNow,
                Status = GameStatus.Played,
                Rate = 5,
                Inactive = false,
                Game = new GameApiResp
                {
                    Id = 1,
                    Name = "Test Game",
                    IGDBId = 123,
                    ReleaseDate = "2023-01-01",
                    Platforms = "PC",
                    Summary = "Test Summary",
                    CoverUrl = "http://example.com/cover.jpg"
                }
            };

            var apiList = new List<GameStatusApiResp> { apiGameResp };
            mockApiService.Setup(s => s.GetByLastUpdateAsync(lastUpdate, 1)).ReturnsAsync(apiList);

            var localGame = new GameDTO
            {
                Id = 1,
                IGDBId = 123,
                UserId = uid,
                Name = "Test Game",
                UpdatedAt = DateTime.UtcNow.AddDays(-3),
                Status = GameStatus.Want,
                Rate = 3
            };

            mockGameService.Setup(s => s.GetByIGDBIdAsync(123, uid)).ReturnsAsync(localGame);

            // Act
            await GameSyncService.ApiToLocalAsync(mockApiService.Object, mockGameService.Object, uid, lastUpdate);

            // Assert
            mockGameService.Verify(s => s.UpdateStatusAsync(localGame.Id, apiGameResp.Status, apiGameResp.Rate, true), Times.Once);
        }

        [TestMethod()]
        public async Task LocalToApiSync_ShouldHandleExecutionTypeInsert()
        {
            // Arrange
            var mockApiOperationRepo = new Mock<IApiOperationRepo>();
            var mockApiService = new Mock<IGameApiService>();
            var mockGameRepo = new Mock<IGameRepo>();
            var mockGameService = new Mock<IGameService>();

            int uid = 1;
            DateTime lastUpdate = DateTime.UtcNow;

            var pendingOperation = new ApiOperationDTO
            {
                Id = 1,
                ObjectType = ObjectType.Game,
                ExecutionType = ExecutionType.Insert,
                Content = JsonSerializer.Serialize(new GameDTO
                {
                    Name = "Test Game",
                    IGDBId = 123,
                    ReleaseDate = "2023-01-01",
                    Platforms = "PC",
                    Summary = "Test Summary",
                    Rate = 5,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Inactive = false,
                    Status = GameStatus.Playing,
                    CoverUrl = "http://example.com/cover.jpg",
                    UserId = uid
                }),
                Status = ApiOperationStatus.Pending,
                ObjectId = "1"
            };

            mockApiOperationRepo
                .Setup(repo => repo.GetByStatusAsync(ApiOperationStatus.Pending))
                .ReturnsAsync([pendingOperation]);

            mockApiOperationRepo
                .Setup(repo => repo.UpdateOperationStatusAsync(ApiOperationStatus.Processing, pendingOperation.Id))
                .Returns(Task.CompletedTask);

            mockApiOperationRepo
                .Setup(repo => repo.UpdateOperationStatusAsync(ApiOperationStatus.Success, pendingOperation.Id))
                .Returns(Task.CompletedTask);

            mockApiService
                .Setup(service => service.CreateAsync(It.IsAny<GameDTO>()))
                .ReturnsAsync(new ServiceResp(true, "12345"));

            mockGameRepo
                .Setup(repo => repo.UpdateExternalIdAsync(It.IsAny<int>(), uid))
                .Returns(Task.CompletedTask);

            // Act
            await GameSyncService.LocalToApiSync(
                mockApiOperationRepo.Object,
                mockApiService.Object,
                mockGameService.Object,
                mockGameRepo.Object,
                uid,
                lastUpdate
            );

            // Assert
            mockApiOperationRepo.Verify(repo => repo.GetByStatusAsync(ApiOperationStatus.Pending), Times.Once);
            mockApiOperationRepo.Verify(repo => repo.UpdateOperationStatusAsync(ApiOperationStatus.Processing, pendingOperation.Id), Times.Once);
            mockApiService.Verify(service => service.CreateAsync(It.IsAny<GameDTO>()), Times.Once);
            mockGameRepo.Verify(repo => repo.UpdateExternalIdAsync(It.IsAny<int>(), uid), Times.Once);
            mockApiOperationRepo.Verify(repo => repo.UpdateOperationStatusAsync(ApiOperationStatus.Success, pendingOperation.Id), Times.Once);
        }

        [TestMethod()]
        public async Task LocalToApiSync_ShouldHandleUpdate_WhenExternalIdIsNull()
        {
            // Arrange
            var apiOperationRepoMock = new Mock<IApiOperationRepo>();
            var apiServiceMock = new Mock<IGameApiService>();
            var gameServiceMock = new Mock<IGameService>();
            var gameRepoMock = new Mock<IGameRepo>();

            int uid = 1;
            DateTime lastUpdate = DateTime.UtcNow;

            var pendingOperation = new ApiOperationDTO()
            {
                Id = 1,
                ObjectId = "1",
                ObjectType = ObjectType.Game,
                ExecutionType = ExecutionType.Update,
                Content = JsonSerializer.Serialize(new ApiOpGameStatus(1, GameStatus.Playing,5)),
                Status = ApiOperationStatus.Pending
                
            };

            apiOperationRepoMock
                .Setup(repo => repo.GetByStatusAsync(ApiOperationStatus.Pending))
                .ReturnsAsync([pendingOperation]);

            apiOperationRepoMock
                .Setup(repo => repo.UpdateOperationStatusAsync(ApiOperationStatus.Processing, pendingOperation.Id))
                .Returns(Task.CompletedTask);

            var localGame = new GameDTO
            {
                Id = 1,
                Name = "Test Game",
                IGDBId = 123,
                ReleaseDate = "2023-01-01",
                Platforms = "PC",
                Summary = "Test Summary",
                Rate = 5,
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                UpdatedAt = DateTime.UtcNow.AddDays(-5),
                Inactive = false,
                Status = GameStatus.Playing,
                CoverUrl = "http://example.com/cover.jpg",
                UserId = uid
            };

            gameRepoMock
                .Setup(repo => repo.GetByIdAsync(1, uid))
                .ReturnsAsync(localGame);

            apiServiceMock
                .Setup(service => service.CreateAsync(It.IsAny<GameDTO>()))
                .ReturnsAsync(new ServiceResp(true,"12345"));

            apiOperationRepoMock
                .Setup(repo => repo.UpdateOperationStatusAsync(ApiOperationStatus.Success, pendingOperation.Id))
                .Returns(Task.CompletedTask);

            // Act
            await GameSyncService.LocalToApiSync(apiOperationRepoMock.Object, apiServiceMock.Object, gameServiceMock.Object, gameRepoMock.Object, uid, lastUpdate);

            // Assert
            apiOperationRepoMock.Verify(repo => repo.UpdateOperationStatusAsync(ApiOperationStatus.Processing, pendingOperation.Id), Times.Once);
            gameRepoMock.Verify(repo => repo.GetByIdAsync(1, uid), Times.Once);
            apiServiceMock.Verify(service => service.CreateAsync(It.Is<GameDTO>(g => g.Name == localGame.Name && g.IGDBId == localGame.IGDBId)), Times.Once);
            apiOperationRepoMock.Verify(repo => repo.UpdateOperationStatusAsync(ApiOperationStatus.Success, pendingOperation.Id), Times.Once);
        }
    }
}