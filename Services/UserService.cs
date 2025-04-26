using ApiRepo;
using Models.DTOs;
using Models.Resps;
using Repo;
using System.Text.Json.Nodes;

namespace Services
{
    public interface IUserService
    {
        Task<UserDTO?> GetUserAsync();
        Task<ApiResp> RecoverPasswordAsync(string email);
        Task<ServiceResp> SignUpAsync(string name, string email, string password);
    }

    public class UserService(IUserRepo userRepo, IUserApiRepo userApiRepo) : IUserService
    {
        public async Task<UserDTO?> GetUserAsync() => await userRepo.GetAsync();

        public async Task<ServiceResp> SignUpAsync(string name, string email, string password)
        {
            email = email.ToLower();
            ApiResp? resp = await userApiRepo.SignUpAsync(name, email, password);

            if (resp is not null && resp.Success && resp.Content is not null)
            {
                JsonNode? jResp = JsonNode.Parse(resp.Content);
                if (jResp is not null)
                {
                    Models.DTOs.UserDTO user = new()
                    {
                        Id = jResp["id"]?.GetValue<int>() ?? 0,
                        Name = jResp["name"]?.GetValue<string>(),
                        Email = jResp["email"]?.GetValue<string>()
                    };

                    if (user.Id is not 0)
                        return new ServiceResp(resp.Success);
                }
            }

            return new ServiceResp(false);
        }

        public async Task<ApiResp> RecoverPasswordAsync(string email)
        {
            ApiResp resp = await userApiRepo.RecoverPasswordAsync(email);

            return new() { Success = resp.Success, Content = resp.Content, Error = (ErrorTypes?)resp.Error };
        }
    }
}
