using Microsoft.EntityFrameworkCore;
using Models.DTOs;

namespace Repo
{
    public interface IApiOperationRepo
    {
        Task<bool> CheckIfHasPendingOperation();
        Task<bool> CheckIfHasPendingOperationWithObjectId(string objectId);
        Task<List<ApiOperationDTO>> GetPendingOperationsByStatusAsync(ApiOperationStatus operationStatus);
        Task InsertOperationInQueueAsync(ApiOperationDTO apiOperation);
        Task UpdateOperationStatusAsync(ApiOperationStatus operationStatus, int operationId);
    }

    public class ApiOperationRepo(IDbContextFactory<DbCtx> dbCtx) : IApiOperationRepo
    {
        public async Task UpdateOperationStatusAsync(ApiOperationStatus operationStatus, int operationId)
        {
            using var context = dbCtx.CreateDbContext();

            await context.ApiOperations.Where(x => x.Id == operationId)
                .ExecuteUpdateAsync(y => y
                .SetProperty(z => z.Status, operationStatus)
                .SetProperty(z => z.UpdatedAt, DateTime.Now));
        }

        public async Task<List<ApiOperationDTO>> GetPendingOperationsByStatusAsync(ApiOperationStatus operationStatus)
        {
            using var context = dbCtx.CreateDbContext();
            return await context.ApiOperations.Where(x => x.Status == operationStatus).OrderBy(x => x.CreatedAt).ToListAsync();
        }

        public async Task InsertOperationInQueueAsync(ApiOperationDTO apiOperation)
        {
            using var context = dbCtx.CreateDbContext();
            context.ApiOperations.Add(apiOperation);

            await context.SaveChangesAsync();
        }

        public async Task<bool> CheckIfHasPendingOperationWithObjectId(string objectId)
        {
            using var context = dbCtx.CreateDbContext();
            return await context.ApiOperations.AnyAsync(x => x.ObjectId == objectId && x.Status == ApiOperationStatus.Pending);
        }

        public async Task<bool> CheckIfHasPendingOperation()
        {
            using var context = dbCtx.CreateDbContext();
            return await context.ApiOperations.AnyAsync(x => x.Status == ApiOperationStatus.Pending);
        }
    }
}
