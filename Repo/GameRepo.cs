using Microsoft.EntityFrameworkCore;
using Models.DTOs;

namespace Repo
{
    public class GameRepo(IDbContextFactory<DbCtx> DbCtx) : IGameRepo
    {
        public async Task<int> CreateAsync(GameDTO game)
        {
            using var context = DbCtx.CreateDbContext();

            await context.Games.AddAsync(game);

            return await context.SaveChangesAsync();
        }
    }
}
