using Models.DTOs;
using Repo;

namespace Services
{
    public interface IUserService
    {
        Task<UserDTO?> GetUserAsync();
    }

    public class UserService(IUserRepo userRepo) : IUserService
    {
        public async Task<UserDTO?> GetUserAsync() => await userRepo.GetAsync();
    }
}
