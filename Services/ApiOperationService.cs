using Models.DTOs;
using Repo;

namespace Services
{
    public interface IApiOperationService
    {
        Task<List<ApiOperationDTO>> GetByStatusAsync(ApiOperationStatus operationStatus);
        Task InsertOperationAsync(string? jsonContent, string objectId, ExecutionType executionType, ObjectType objectType);

        Task UpdateOperationStatusAsync(ApiOperationStatus operationStatus, int operationId);
    }

    public class ApiOperationService(IApiOperationRepo operationQueueRepo) : IApiOperationService
    {
        public async Task InsertOperationAsync(string? jsonContent, string objectId, ExecutionType executionType, ObjectType objectType)
        {
            DateTime dateTimeNow = DateTime.Now;

            ApiOperationDTO apiOperation = new()
            {
                CreatedAt = dateTimeNow,
                ObjectType = objectType,
                Status = ApiOperationStatus.Pending,
                UpdatedAt = dateTimeNow,
                Content = jsonContent,
                ObjectId = objectId,
                ExecutionType = executionType
            };

            await operationQueueRepo.InsertOperationInQueueAsync(apiOperation);
        }

        public async Task UpdateOperationStatusAsync(ApiOperationStatus operationStatus, int operationId) =>
            await operationQueueRepo.UpdateOperationStatusAsync(operationStatus, operationId);

        public async Task<List<ApiOperationDTO>> GetByStatusAsync(ApiOperationStatus operationStatus) =>
            await operationQueueRepo.GetByStatusAsync(operationStatus);
    }
}
