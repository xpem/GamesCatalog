using ApiRepo;
using Newtonsoft.Json;

namespace Services
{
    public static class IGDBGamesApiService
    {
        public async static Task<List<Models.Resps.IGDBGame>> Get(string search, int startIndex)
        {
            Models.Resps.ApiResp resp = await IGDBGamesAPIRepo.Get(search, startIndex);

            if (resp is not null && resp.Success && resp.Content is not null)
            {
                return JsonConvert.DeserializeObject<List<Models.Resps.IGDBGame>>(resp.Content)
                    ?? throw new Exception("Error getting games");
            }
            else throw new Exception("Error getting games" + resp.Error);
        }

        public async static Task SaveImageAsync(string imageUrl, string fileName)
        {
            byte[] imageBytes = await IGDBGamesAPIRepo.GetGameImageAsByteArrayAsync(imageUrl);

            string filePath = Path.Combine(GameService.ImagesPath, fileName);

            if (File.Exists(filePath))
                return;

            await File.WriteAllBytesAsync(filePath, imageBytes);
        }
    }
}
