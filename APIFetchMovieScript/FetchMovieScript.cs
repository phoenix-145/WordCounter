using System.Reflection.Metadata.Ecma335;

namespace WordCounter
{
    public class FetchMovieScript
    {
        internal static async Task GetMovieName()
        {
            while(true)
            {
                Console.Clear();
                Console.WriteLine(AsciiArt.OnlinewordcounterArt);
                Console.WriteLine("\n");
                Console.Write("Enter the movie name you want to search up or enter 0 to exit : ");
                var input = Console.ReadLine();
                if(input == "0")
                {break;}
                if(!string.IsNullOrEmpty(input))
                {
                    await SearchMovie(input);
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid movie name.");
                } 
            }
        }
        private static async Task SearchMovie(string moviename)
        {
            bool loop = true;
            int page_no = 1;
            Tmdb_Response tmdb_Response = null!;
            
            while(loop)
            {
                DisplayBanner();
                if(tmdb_Response == null || tmdb_Response.page != page_no)
                {
                    tmdb_Response = await TmdbAPI.SearchMovieinTMDB(moviename, page_no);
                }
                var UserMenuChoice = DisplaySearchResults.DisplaySearchedResults(tmdb_Response, null!, page_no);
                if((UserMenuChoice is NavigationActions) && UserMenuChoice == NavigationActions.Exit)
                {
                    break;
                }
                if ((UserMenuChoice is NavigationActions) && UserMenuChoice == NavigationActions.Next_Page)
                {
                    page_no += 1;
                }
                else if ((UserMenuChoice is NavigationActions) && UserMenuChoice == NavigationActions.Prev_Page)
                {
                    page_no -= 1;
                }
                else
                {
                    int CorrectMovie = DisplayMovieInfo(tmdb_Response.results![UserMenuChoice]);
                    if(CorrectMovie == 0)
                    {
                        DisplayBanner();
                        Subtitles subtitles = await OpensubtitlesAPI.GetSubtitlesId(tmdb_Response.results![UserMenuChoice].id, tmdb_Response.results![UserMenuChoice].title!);
                        if(subtitles != null)
                        {
                            dynamic SubtitlesChoice = DisplaySearchResults.DisplaySearchedResults(null!, subtitles, 1);
                            if(SubtitlesChoice is not NavigationActions)
                            {
                                await OpensubtitlesAPI.LoginAndDownloadSubtitles(subtitles.data![SubtitlesChoice].id!);
                                break;
                            }
                        }
                    }
                } 
            }  
        }
        private static int DisplayMovieInfo(Tmdb_Response.Result tmdb_Response)
        {
            string[] MovieDetails = {tmdb_Response.title!, tmdb_Response.original_title!, tmdb_Response.overview!, tmdb_Response.release_date!, "https://image.tmdb.org/t/p/original/" + tmdb_Response.poster_path!};
            string[] Name = {"Movie Title","Movie Original Title","Movie Overview","Movie Release Date","Movie Poster Path"};
            
            DisplayBanner();
            for(int i = 0; i < MovieDetails.Length; i++)
            {
                string _name = "**"+ Name[i] + "**";
                int CountofChar = _name.Length;
                int DiffinCount = CountofChar - MovieDetails[i].Length;
                if(i == 2)
                {
                    Console.WriteLine(_name);
                    Console.WriteLine(MovieDetails[i]);
                    if(MovieDetails[i].Length > Console.WindowWidth)
                    {
                        for(int k = 0; k < Console.WindowWidth; k++)
                        {
                            Console.Write("=");
                        }
                    }
                    else
                    {
                        foreach(char c in MovieDetails[i])
                        {
                            Console.Write("=");
                        }
                    }
                }
                else if(DiffinCount > 0)
                {
                    Console.WriteLine(_name);
                    for(int j = 0; j < (DiffinCount/2); j++)
                    {
                        Console.Write(" ");
                    }
                    Console.WriteLine(MovieDetails[i]);
                    foreach(char c in _name)
                    {
                        Console.Write("=");
                    }
                }
                else
                {
                    for(int j = 0; j < ((MovieDetails[i].Length - CountofChar)/2); j++)
                    {
                        Console.Write(" ");
                    }
                    Console.WriteLine(_name);
                    Console.WriteLine(MovieDetails[i]);
                    foreach(char c in MovieDetails[i])
                    {
                        Console.Write("=");
                    }
                }
                Console.WriteLine();
            }
            
            Console.WriteLine("\n");
            string prompt = "Is this the correct movie ?";
            string[] options = { "Yes", "No",};
            Menu menu = new Menu(prompt, options);
            int input = menu.Run();

            return input;
        }
        private static void DisplayBanner()
        {
            Console.Clear();
            Console.WriteLine(AsciiArt.OnlinewordcounterArt);
            Console.WriteLine("\n");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(AsciiArt.border);
            Console.WriteLine(AsciiArt.OnlineSearchResultArt);
            Console.WriteLine(AsciiArt.border);
            Console.ResetColor();   
            Console.WriteLine("\n");
        }
    }
}