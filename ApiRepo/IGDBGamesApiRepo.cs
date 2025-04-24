using Models.Resps;
using System.Net;
using System.Net.Http;
using System.Text;

namespace ApiRepo
{
    public static class IGDBGamesAPIRepo
    {
        public async static Task<ApiResp> GetAsync(string search, int startIndex)
        {
            try
            {
                HttpClient httpClient = new();

                //httpClient.DefaultRequestHeaders.Add("Client-ID", ApiKeys.CLIENTID);
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1aWQiOiI1IiwiZW1haWwiOiJlbWFudWVsLnhwZUBnbWFpbC5jb20iLCJuYmYiOjE3NDQ4Mzg2NjQsImV4cCI6MTc0NTI3MDY2NCwiaWF0IjoxNzQ0ODM4NjY0fQ.x_Q_a1PcGxgeduZFJZllNc7eGOZd1cKBfU7aHH9UQSg");

                var bodyContent = new StringContent($"{{  \"Search\": \"{search}\",  \"StartIndex\": \"{startIndex}\"}}", Encoding.UTF8, "application/json");
                //http://10.0.2.2:5048
                //http://localhost:5048
                HttpResponseMessage httpResponse = await httpClient.PostAsync("http://localhost:5048/Game/IGDB", bodyContent);

                return new ApiResp()
                {
                    Success = httpResponse.IsSuccessStatusCode,
                    Error = httpResponse.StatusCode == HttpStatusCode.Unauthorized ? ErrorTypes.Unauthorized : null,
                    Content = await httpResponse.Content.ReadAsStringAsync()
                };
            }
            catch(Exception ex) { throw; }
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
