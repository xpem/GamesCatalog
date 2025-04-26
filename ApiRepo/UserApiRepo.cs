using Models.Resps;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ApiRepo
{
    public interface IUserApiRepo
    {
        Task<ApiResp> SignUpAsync(string name, string email, string password);
        Task<(bool, string?)> GetUserSessionAsync(string email, string password);
        Task<ApiResp> RecoverPasswordAsync(string email);
    }

    public class UserApiRepo(string serverUrl) : IUserApiRepo
    {

        public async Task<ApiResp> SignUpAsync(string name, string email, string password)
        {
            try
            {
                string json = JsonSerializer.Serialize(new { name, email, password });

                return await HttpClientFunctions.Request(Models.RequestsTypes.Post, serverUrl + "/user",jsonContent: json);
            }
            catch (Exception) { throw; }
        }

        public async Task<ApiResp> RecoverPasswordAsync(string email)
        {
            try
            {
                string json = JsonSerializer.Serialize(new { email });

                return await HttpClientFunctions.Request(Models.RequestsTypes.Post, serverUrl + "/user/recoverpassword", jsonContent: json);
            }
            catch (Exception) { throw; }
        }

        public async Task<(bool, string?)> GetUserSessionAsync(string email, string password)
        {
            try
            {
                string json = JsonSerializer.Serialize(new { email, password });

                var resp = await HttpClientFunctions.Request(Models.RequestsTypes.Post, serverUrl + "/user/session", jsonContent: json);

                if (resp is not null && resp.Content is not null)
                {
                    if (resp.Success)
                    {
                        JsonNode? jResp = JsonNode.Parse(resp.Content);

                        if (resp.Success && jResp is not null && jResp?["token"]?.GetValue<string>() is not null)
                            return (true, jResp?["token"]?.GetValue<string>());
                        else if (!resp.Success && jResp is not null)
                        {
                            if (jResp?["errors"]?.GetValue<string>() is not null)
                                return (false, jResp?["errors"]?.GetValue<string>());
                            else if (jResp?["error"]?.GetValue<string>() is not null)
                                return (false, jResp?["error"]?.GetValue<string>());
                        }
                    }
                    else if (resp.Content is not null)
                    {
                        return (false, resp.Content);
                    }

                    else throw new Exception(resp.Content);
                }

                throw new ArgumentNullException(nameof(resp));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
