using Models.DTOs;

namespace Repo
{
    public interface IGameRepo
    {
        Task<int> CreateAsync(GameDTO game);
    }
}