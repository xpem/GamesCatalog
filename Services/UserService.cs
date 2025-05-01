using ApiRepo;
using Models.DTOs;
using Models.Handlers;
using Models.Resps;
using Repo;
using Services.Interfaces;
using System.Text.Json.Nodes;

namespace Services
{
    public interface IUserService
    {
        Task<UserDTO?> GetUserAsync();
        Task<ApiResp> RecoverPasswordAsync(string email);
        Task<ServiceResp> SignInAsync(string email, string password);
        Task<ServiceResp> SignUpAsync(string name, string email, string password);
    }

    public class UserService(IUserRepo userRepo, IUserApiRepo userApiRepo,IBuildDbService buildDbService) : IUserService
    {
        public async Task<UserDTO?> GetUserAsync() => await userRepo.GetAsync();

        public async Task<ServiceResp> SignInAsync(string email, string password)
        {
            try
            {
                email = email.ToLower();

                var apiresp = await userApiRepo.GetTokenAsync(email, password);

                if (apiresp.Success && apiresp.Content is not null and string)
                {
                    string newToken = apiresp.Content;

                    ApiResp resp = await userApiRepo.GetAsync(newToken);

                    if (resp.Success && resp.Content != null)
                    {
                        JsonNode? userResponse = JsonNode.Parse(resp.Content);
                        if (userResponse is not null)
                        {
                            UserDTO? user = new()
                            {
                                Id = userResponse["id"]?.GetValue<int>() ?? 0,
                                Name = userResponse["name"]?.GetValue<string>(),
                                Email = userResponse["email"]?.GetValue<string>(),
                                Token = newToken,
                                Password = EncryptionHandler.Encrypt(password)
                            };

                            UserDTO? actualUser = await userRepo.GetAsync();

                            //resign 
                            if (actualUser != null)
                            {
                                //with the same user
                                if (actualUser.Id == user.Id)
                                    await userRepo.UpdateAsync(user);
                                else
                                {
                                    await buildDbService.CleanLocalDatabase();
                                    await userRepo.CreateAsync(user);
                                }
                            }
                            else
                                await userRepo.CreateAsync(user);

                            return new ServiceResp(true, user);
                        }
                    }
                }
                else if (!apiresp.Success && apiresp.Content is not null && apiresp.Content is "User/Password incorrect" or "Invalid Email")
                    return new ServiceResp(false, ErrorTypes.WrongEmailOrPassword);
                else return new ServiceResp(false, ErrorTypes.ServerUnavaliable);

                return new ServiceResp(false, ErrorTypes.Unknown);
            }
            catch { throw; }
        }

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
