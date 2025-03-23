using Models.DTOs;
using Models.Resps;

namespace Services
{
    public interface IGameService
    {
        Task<ServiceResp> CreateAsync(GameDTO game);
    }
}