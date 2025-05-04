using Microsoft.EntityFrameworkCore;
using Models.DTOs;

namespace Repo
{
    public interface IUserRepo
    {
        Task<int> CreateAsync(UserDTO user);
        Task<UserDTO?> GetAsync();
        Task UpdateAsync(UserDTO user);
        Task UpdateLastUpdateAsync(DateTime lastUpdate, int uid);
    }

    public class UserRepo(IDbContextFactory<DbCtx> dbCtx) : IUserRepo
    {
        public async Task<UserDTO?> GetAsync()
        {
            using var context = dbCtx.CreateDbContext();
            return await context.Users.FirstOrDefaultAsync();
        }

        public async Task<int> CreateAsync(UserDTO user)
        {
            using var context = dbCtx.CreateDbContext();
            await context.Users.AddAsync(user);
            return await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(UserDTO user)
        {
            using var context = dbCtx.CreateDbContext();
            context.Users.Update(user);
            await context.SaveChangesAsync();
        }

        public async Task UpdateLastUpdateAsync(DateTime lastUpdate, int uid)
        {
            using var context = dbCtx.CreateDbContext();
           await context.Users.Where(x => x.Id == uid).ExecuteUpdateAsync(y => y.SetProperty(z => z.LastUpdate, lastUpdate));
        }
    }
}
