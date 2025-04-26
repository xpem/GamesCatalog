using Models;
using Models.Resps;
using System.Net;
using System.Text;

namespace ApiRepo
{
    public static class HttpClientFunctions
    {
        public static async Task<ApiResp> Request(RequestsTypes requestsType, string url, string? userToken = null, string? jsonContent = null)
        {
            try
            {
                HttpClient httpClient = new(new HttpClientHandler
                {
                    SslProtocols = System.Security.Authentication.SslProtocols.Tls12
                });

                if (userToken is not null)
                    httpClient.DefaultRequestHeaders.Add("authorization", "bearer " + userToken);

                HttpResponseMessage httpResponse = new();

                switch (requestsType)
                {
                    case RequestsTypes.Get:
                        httpResponse = await httpClient.GetAsync(url);
                        break;
                    case RequestsTypes.Post:
                        if (jsonContent is not null)
                        {
                            StringContent bodyContent = new(jsonContent, Encoding.UTF8, "application/json");
                            httpResponse = await httpClient.PostAsync(url, bodyContent);
                        }
                        else return new ApiResp() { Success = false, Content = null, Error = ErrorTypes.BodyContentNull };
                        break;
                    case RequestsTypes.Put:
                        if (jsonContent is not null)
                        {
                            StringContent bodyContent = new(jsonContent, Encoding.UTF8, "application/json");
                            httpResponse = await httpClient.PutAsync(url, bodyContent);
                        }
                        else return new ApiResp() { Success = false, Content = null, Error = ErrorTypes.BodyContentNull };
                        break;
                    case RequestsTypes.Delete:
                        httpResponse = await httpClient.DeleteAsync(url);
                        break;
                }


                return new ApiResp()
                {
                    Success = httpResponse.IsSuccessStatusCode,
                    Error = httpResponse.StatusCode == HttpStatusCode.Unauthorized ? ErrorTypes.Unauthorized : null,
                    TryRefreshToken = httpResponse.StatusCode == HttpStatusCode.Unauthorized,
                    Content = await httpResponse.Content.ReadAsStringAsync()
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
