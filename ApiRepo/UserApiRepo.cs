using Models;
using Models.DTOs;
using Models.Handlers;
using Models.Resps;
using Repo;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ApiRepo
{
    public interface IUserApiRepo
    {
        Task<ApiResp> SignUpAsync(string name, string email, string password);
        Task<ApiResp> RecoverPasswordAsync(string email);
        Task<ApiResp> GetTokenAsync(string email, string password);
        Task<(bool success, string? newToken)> RefreshToken();
        Task<ApiResp> GetAsync(string userToken);
        Task<ApiResp> AuthRequestAsync(RequestsTypes requestsType, string url, string? jsonContent = null);
    }

    public class UserApiRepo(IUserRepo userRepo) : IUserApiRepo
    {
        public async Task<ApiResp> SignUpAsync(string name, string email, string password)
        {
            try
            {
                string json = JsonSerializer.Serialize(new { name, email, password });

                return await HttpClientFunctions.Request(Models.RequestsTypes.Post, DeviceHandler.Url + "/user", jsonContent: json);
            }
            catch (Exception) { throw; }
        }

        public async Task<ApiResp> RecoverPasswordAsync(string email)
        {
            try
            {
                string json = JsonSerializer.Serialize(new { email });

                return await HttpClientFunctions.Request(Models.RequestsTypes.Post, DeviceHandler.Url + "/user/recoverpassword", jsonContent: json);
            }
            catch (Exception) { throw; }
        }

        public async Task<ApiResp> GetTokenAsync(string email, string password)
        {
            try
            {
                string json = JsonSerializer.Serialize(new { email, password });

                var resp = await HttpClientFunctions.Request(Models.RequestsTypes.Post, DeviceHandler.Url + "/user/session", jsonContent: json);

                if (resp is not null && resp.Content is not null)
                {
                    if (resp.Success)
                    {
                        JsonNode? jResp = JsonNode.Parse(resp.Content);

                        if (resp.Success && jResp is not null && jResp?["token"]?.GetValue<string>() is not null)
                            return new ApiResp() { Success = true, Content = jResp?["token"]?.GetValue<string>() };
                        else if (!resp.Success && jResp is not null)
                        {
                            if (jResp?["errors"]?.GetValue<string>() is not null)
                                return new ApiResp() { Success = false, Content = jResp?["errors"]?.GetValue<string>() };
                            else if (jResp?["error"]?.GetValue<string>() is not null)
                                return new ApiResp() { Success = false, Content = jResp?["error"]?.GetValue<string>() };
                        }
                    }
                    else if (resp.Content is not null)
                        return new ApiResp() { Success = false, Content = resp.Content };

                    else throw new Exception(resp.Content);
                }

                throw new ArgumentNullException(nameof(resp));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<(bool success, string? newToken)> RefreshToken()
        {
            UserDTO? user = await userRepo.GetAsync();

            if (user is not null && user.Email is not null && user.Password is not null)
            {
                string password = EncryptionHandler.Decrypt(user.Password);

                var apiresp = await GetTokenAsync(user.Email, password);

                if (apiresp.Success && apiresp.Content is not null)
                {
                    string newToken = apiresp.Content;
                    user.Token = newToken;

                    await userRepo.UpdateAsync(user);

                    return (true, newToken);
                }
                else throw new UnauthorizedAccessException("Falha ao tentar recuperar token do usuario");
            }

            return (false, null);
        }

        public async Task<ApiResp> AuthRequestAsync(RequestsTypes requestsType, string url, string? jsonContent = null)
        {
            bool retry = true;
            ApiResp? resp = null;

            while (retry)
            {
                string? userToken;

                if (resp is not null && resp.TryRefreshToken)
                {
                    retry = false;

                    (bool refreshTokenSuccess, userToken) = await RefreshToken();

                    if (!refreshTokenSuccess || userToken is null)
                        return resp;
                }
                else
                {
                    userToken = (await userRepo.GetAsync())?.Token;

                    if (userToken is null) throw new ArgumentNullException(nameof(userToken));
                }

                resp = await HttpClientFunctions.Request(requestsType, url, userToken, jsonContent);

                if (!resp.TryRefreshToken || !retry) return resp;
            }

            throw new Exception($"Erro ao tentar AuthRequest de tipo {requestsType} na url: {url}");
        }

        public async Task<ApiResp> GetAsync(string userToken) => await HttpClientFunctions.Request(Models.RequestsTypes.Get, DeviceHandler.Url + "/user", userToken: userToken);

    }
}
