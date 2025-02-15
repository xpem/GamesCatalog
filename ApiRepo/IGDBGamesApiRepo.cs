using Models.Resps;
using System.Net;
using System.Text;

namespace ApiRepo
{
    public static class IGDBGamesAPIRepo
    {
        public async static Task<ApiResp> Get(string search, int startIndex)
        {
            try
            {
                HttpClient httpClient = new();

                httpClient.DefaultRequestHeaders.Add("Client-ID", ApiKeys.CLIENTID);
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKeys.TOKENTEMP}");

                var bodyContent = new StringContent($"fields cover,cover.url,cover.image_id,first_release_date,name,platforms.abbreviation;search \"{search}\"; limit 20; offset {startIndex};", Encoding.UTF8, "application/json");

                HttpResponseMessage httpResponse = await httpClient.PostAsync("https://api.igdb.com/v4/games", bodyContent);

                return new ApiResp()
                {
                    Success = httpResponse.IsSuccessStatusCode,
                    Error = httpResponse.StatusCode == HttpStatusCode.Unauthorized ? ErrorTypes.Unauthorized : null,
                    Content = await httpResponse.Content.ReadAsStringAsync()
                };
            }
            catch { throw; }
        }
    }
}
