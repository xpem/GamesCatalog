using Models.Handlers;
using Models.Resps;
using System.Text.Json;

namespace ApiRepo
{
    public interface IIGDBGamesAPIRepo
    {
        Task<ApiResp> GetAsync(string search, int startIndex);
    }

    public class IGDBGamesAPIRepo(IUserApiRepo userApiRepo) : IIGDBGamesAPIRepo
    {
        public async Task<ApiResp> GetAsync(string search, int startIndex)
        {
            try
            {
                var jsonSearch = JsonSerializer.Serialize(new { Search = search, StartIndex = startIndex });

                return await userApiRepo.AuthRequestAsync(Models.RequestsTypes.Post, DeviceHandler.Url + "/Game/IGDB", jsonSearch);
            }
            catch (Exception ex) { throw; }
        }

        public static async Task<byte[]> GetGameImageAsync(string imageUrl)
        {
            try
            {
                HttpClient httpClient = new();

                var response = await httpClient.GetAsync(imageUrl);
                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Exception downloading image url:{imageUrl}");

                return await response.Content.ReadAsByteArrayAsync();
            }
            catch
            {
                throw;
            }
        }
    }
}
