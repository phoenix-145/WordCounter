using System.Configuration;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using static WordCounter.Program;

namespace WordCounter
{
    public class TmdbAPI
    {
        internal static readonly HttpClient httpClient = HttpClientFactory.httpClient;
        private class CheckTmdb_AuthToken
        {
            private readonly static string _tmdbAuthToken = ConfigurationManager.AppSettings.Get("TmdbAPI_AuthToken")!;
            public static string Tmdb_AuthToken
            {
                get
                {
                    if (_tmdbAuthToken == "YourTMDBAuthToken" || string.IsNullOrWhiteSpace(_tmdbAuthToken))
                    {
                        SlowPrintingText.SlowPrintText("Your TMDB authentication token is empty. Add it in app.config file.");
                        return null!;
                    }
                    return _tmdbAuthToken;
                }
            }
        }
        internal protected static readonly string TmdbURL = "https://api.themoviedb.org/3/search/movie";
        
        internal static async Task<Tmdb_Response> SearchMovieinTMDB(string movieName, int page_no)
        {
            string Tmdb_AuthToken = CheckTmdb_AuthToken.Tmdb_AuthToken;
            if(Tmdb_AuthToken == null)
            {return null!;}

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
            try
            {
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
            catch (TaskCanceledException)
            {SlowPrintingText.SlowPrintText("API call timed out. API service may be down."); return null!; }
            catch (HttpRequestException)
            {SlowPrintingText.SlowPrintText("Error : no internet connection."); return null!; }
        }
        private static string GenerateURI(string movieName, int page_no)
        {
            string QueryParameters = $"?query={movieName}&include_adult=true&language=en-US&page={page_no}";
            string fullURI = TmdbURL + QueryParameters;
            return fullURI;
        }
    }
}