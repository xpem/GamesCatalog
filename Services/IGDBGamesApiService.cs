using ApiRepo;
using Models.Resps;
using Newtonsoft.Json;

namespace Services
{
    public interface IIGDBGamesApiService
    {
        Task<List<IGDBGame>> GetAsync(string search, int startIndex);
    }

    public class IGDBGamesApiService(IIGDBGamesAPIRepo iGDBGamesAPIRepo) : IIGDBGamesApiService
    {
        public async Task<List<Models.Resps.IGDBGame>> GetAsync(string search, int startIndex)
        {
            Models.Resps.ApiResp resp = await iGDBGamesAPIRepo.GetAsync(search, startIndex);

            if (resp is not null && resp.Success && resp.Content is not null)
            {
                return JsonConvert.DeserializeObject<List<Models.Resps.IGDBGame>>(resp.Content)
                    ?? throw new Exception("Error getting games");
            }
            else throw new Exception("Error getting games" + resp.Error);
        }

        public async static Task SaveImageAsync(string imageUrl, string fileName)
        {
            try
            {
                byte[] imageBytes = await IGDBGamesAPIRepo.GetGameImageAsync(imageUrl);

                string filePath = Path.Combine(GameService.ImagesPath, fileName);

                if (File.Exists(filePath))
                    return;

                await File.WriteAllBytesAsync(filePath, imageBytes);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
