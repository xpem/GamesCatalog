using Models.DTOs;
using Repo;

namespace Services
{
    public interface IApiOperationService
    {
        Task InsertOperationAsync(string? jsonContent, string objectId, ExecutionType executionType, ObjectType objectType);
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
    }
}
