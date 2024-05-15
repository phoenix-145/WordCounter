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
        public static async Task GetMovieName()
        {
            while(true)
            {
                Console.Write("Enter the movie name you want to search up : ");
                var input = Console.ReadLine();
                if(!string.IsNullOrEmpty(input))
                {
                    await SearchMovieinTMDB(input!);
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid movie name.");
                } 
            }
        }
        private static async Task SearchMovieinTMDB(string movieName)
        {
            int page_no = 1;

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

            using(var response = await httpClient.SendAsync(request))
            {
                Console.WriteLine("hi");
                if(response.IsSuccessStatusCode)
                {
                    var response_body = await response.Content.ReadAsStringAsync();
                    if(!string.IsNullOrEmpty(response_body))
                    {
                        Tmdb_Response tmdb_Response = JsonSerializer.Deserialize<Tmdb_Response>(response_body)!;
                        DisplaySearchedMovies(tmdb_Response);
                    }
                    else
                    {
                        Console.WriteLine($"Error, the server returned a bad response.");
                    }
                }
                else
                {
                    SlowPrintingText.SlowPrintText($"Error, something went wrong while searching for movie. Status : " + response.StatusCode);
                }
            }
        }
        private static string GenerateURI(string movieName, int page_no)
        {
            string QueryParameters = $"?query={movieName}&include_adult=true&language=en-US&page={page_no}";
            string fullURI = TmdbURL + QueryParameters;
            return fullURI;
        }
        private static void DisplaySearchedMovies(Tmdb_Response tmdb_Response)
        {
            List<string> MovieTitles = new();
            foreach(var movies in tmdb_Response.results!)
            {
                MovieTitles.Add(movies.title!);
            }

            string[] MovieTitlesArray = MovieTitles.ToArray();
            string prompt = "Choose your movie";
            Menu menu = new Menu(prompt, MovieTitlesArray);
            int input = menu.Run();

            Console.Clear();
            Console.WriteLine($"Movie Title : " + tmdb_Response.results[input].title);
            Console.WriteLine($"Original Title : " + tmdb_Response.results[input].original_title);
            Console.WriteLine($"Overview : " + tmdb_Response.results[input].overview);       
            Console.WriteLine($"Release Date : " + tmdb_Response.results[input].release_date);
            Console.WriteLine($"Poster Path : " + tmdb_Response.results[input].poster_path);                                                                         
        }
    }
}