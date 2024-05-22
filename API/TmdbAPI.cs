using System.Configuration;
using System.Text.Json;
using static WordCounter.Program;

namespace WordCounter
{
    public class TmdbAPI
    {
        internal protected static readonly string Tmdb_AuthToken = ConfigurationManager.AppSettings.Get("TmdbAPI_AuthToken")!;
        internal static readonly HttpClient httpClient = HttpClientFactory.httpClient;
        internal protected static readonly string TmdbURL = "https://api.themoviedb.org/3/search/movie";
        internal static async Task<Tmdb_Response> SearchMovieinTMDB(string movieName, int page_no)
        {
            string movieName_UrlEncoded = Uri.EscapeDataString(movieName);
            string fullURI = GenerateURI(movieName_UrlEncoded, page_no);

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(fullURI),
                Headers = 
                {
                    { "accept", "application/json" },
                    { "Authorization", $"Bearer {Tmdb_AuthToken}" },
                }
            };

            using var response = await httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var response_body = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(response_body))
                {
                    Tmdb_Response tmdb_Response = JsonSerializer.Deserialize<Tmdb_Response>(response_body)!;
                    return tmdb_Response;
                }
                else
                {
                    Console.WriteLine($"Error. the server returned a bad response.");
                    return null!;
                }
            }
            else
            {
                SlowPrintingText.SlowPrintText($"Error. Something went wrong while searching for movie. Status : " + response.StatusCode);
                return null!;
            }
        }
        private static string GenerateURI(string movieName, int page_no)
        {
            string QueryParameters = $"?query={movieName}&include_adult=true&language=en-US&page={page_no}";
            string fullURI = TmdbURL + QueryParameters;
            return fullURI;
        }
    }
}