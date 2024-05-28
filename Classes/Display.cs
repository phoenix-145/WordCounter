using System.Text;

public class SlowPrintingText
{
    public static void SlowPrintText(string text)
    {
        foreach(char c in text)
        {
            Console.Write(c);
            Thread.Sleep(20);
        }
        Thread.Sleep(100);
        Console.WriteLine("");
    }
}

public class DisplaySearchResults
{
    internal static dynamic DisplaySearchedResults(Tmdb_Response tmdb_Response, Subtitles subtitles, int page_no)
    {
        List<string> MenuItems = new();
        if(tmdb_Response != null)
        {
            foreach(var movies in tmdb_Response.results!)
            {
                MenuItems.Add(movies.title!);
            }
            if(page_no != 1)
            {
                MenuItems.Add("Previous Page");
            }
            MenuItems.Add("Next Page");
            if(MenuItems.Count < 3)
            {
                MenuItems.Remove("Next Page");
            }
            Console.WriteLine("Page number : " + page_no);
            Console.WriteLine("Total Movies Found : " + tmdb_Response.total_results);
            Console.WriteLine("Total pages : " + tmdb_Response.total_pages);
        }
        if(subtitles != null)
        {
            int MaxNumSubtitles = subtitles.data!.Length > 5 ? 6 : subtitles.data.Length;
            for(int i = 0; i < MaxNumSubtitles; i++)
            {
                MenuItems.Add(subtitles.data![i].attributes!.files![0].file_name!);
            }
        }

        Console.WriteLine("\n");

        string[] MenuItemsArray = MenuItems.ToArray();
        var prompt = new StringBuilder(tmdb_Response != null ? "Choose your movie" : "Choose the subtitles file");
        Menu menu = tmdb_Response != null ? new(prompt.ToString(), MenuItemsArray) : new(prompt.ToString(), MenuItemsArray, subtitles!);
        dynamic input = menu.Run();
        if((input is NavigationActions) && input == NavigationActions.Exit)
        {
            return NavigationActions.Exit;
        }
        if(tmdb_Response != null)
        {
            if(MenuItemsArray.Length == tmdb_Response!.results!.Length + 2)
            {
                if (input == MenuItemsArray.Length - 1)
                {
                    return NavigationActions.Next_Page;
                }
                if (input == MenuItemsArray.Length - 2)
                {
                    return NavigationActions.Prev_Page;
                }
            }
            else if(MenuItemsArray.Length == tmdb_Response.results.Length + 1 && tmdb_Response.results.Length > 0)
            {
                if (input == MenuItemsArray.Length - 1)
                {
                    return NavigationActions.Next_Page;
                }
            }
            else if(MenuItemsArray.Length == 1)
            {
                if (input == MenuItemsArray.Length - 1)
                {
                    return NavigationActions.Prev_Page;
                }
            }
        }
        return input;
    }
}

public class AsciiArt
{
    public static readonly string MainMenuArt = @"██╗   ██╗███████╗██████╗ ██████╗  █████╗ ████████╗██╗███╗   ███╗
██║   ██║██╔════╝██╔══██╗██╔══██╗██╔══██╗╚══██╔══╝██║████╗ ████║
██║   ██║█████╗  ██████╔╝██████╔╝███████║   ██║   ██║██╔████╔██║
╚██╗ ██╔╝██╔══╝  ██╔══██╗██╔══██╗██╔══██║   ██║   ██║██║╚██╔╝██║
 ╚████╔╝ ███████╗██║  ██║██████╔╝██║  ██║   ██║   ██║██║ ╚═╝ ██║
  ╚═══╝  ╚══════╝╚═╝  ╚═╝╚═════╝ ╚═╝  ╚═╝   ╚═╝   ╚═╝╚═╝     ╚═╝
";

    public static readonly string LocalwordcounterArt = @"  ______                    __                                 __                                  __       ______                    __            __     
 /      \                  |  \                               |  \                                |  \     /      \                  |  \          |  \    
|  ▓▓▓▓▓▓\_______   ______ | ▓▓__    __  _______  ______      | ▓▓       ______   _______  ______ | ▓▓    |  ▓▓▓▓▓▓\ _______  ______  \▓▓ ______  _| ▓▓_   
| ▓▓__| ▓▓       \ |      \| ▓▓  \  |  \/       \/      \     | ▓▓      /      \ /       \|      \| ▓▓    | ▓▓___\▓▓/       \/      \|  \/      \|   ▓▓ \  
| ▓▓    ▓▓ ▓▓▓▓▓▓▓\ \▓▓▓▓▓▓\ ▓▓ ▓▓  | ▓▓  ▓▓▓▓▓▓▓  ▓▓▓▓▓▓\    | ▓▓     |  ▓▓▓▓▓▓\  ▓▓▓▓▓▓▓ \▓▓▓▓▓▓\ ▓▓     \▓▓    \|  ▓▓▓▓▓▓▓  ▓▓▓▓▓▓\ ▓▓  ▓▓▓▓▓▓\\▓▓▓▓▓▓  
| ▓▓▓▓▓▓▓▓ ▓▓  | ▓▓/      ▓▓ ▓▓ ▓▓  | ▓▓\▓▓    \| ▓▓    ▓▓    | ▓▓     | ▓▓  | ▓▓ ▓▓      /      ▓▓ ▓▓     _\▓▓▓▓▓▓\ ▓▓     | ▓▓   \▓▓ ▓▓ ▓▓  | ▓▓ | ▓▓ __ 
| ▓▓  | ▓▓ ▓▓  | ▓▓  ▓▓▓▓▓▓▓ ▓▓ ▓▓__/ ▓▓_\▓▓▓▓▓▓\ ▓▓▓▓▓▓▓▓    | ▓▓_____| ▓▓__/ ▓▓ ▓▓_____|  ▓▓▓▓▓▓▓ ▓▓    |  \__| ▓▓ ▓▓_____| ▓▓     | ▓▓ ▓▓__/ ▓▓ | ▓▓|  \
| ▓▓  | ▓▓ ▓▓  | ▓▓\▓▓    ▓▓ ▓▓\▓▓    ▓▓       ▓▓\▓▓     \    | ▓▓     \\▓▓    ▓▓\▓▓     \\▓▓    ▓▓ ▓▓     \▓▓    ▓▓\▓▓     \ ▓▓     | ▓▓ ▓▓    ▓▓  \▓▓  ▓▓
 \▓▓   \▓▓\▓▓   \▓▓ \▓▓▓▓▓▓▓\▓▓_\▓▓▓▓▓▓▓\▓▓▓▓▓▓▓  \▓▓▓▓▓▓▓     \▓▓▓▓▓▓▓▓ \▓▓▓▓▓▓  \▓▓▓▓▓▓▓ \▓▓▓▓▓▓▓\▓▓      \▓▓▓▓▓▓  \▓▓▓▓▓▓▓\▓▓      \▓▓ ▓▓▓▓▓▓▓    \▓▓▓▓ 
                              |  \__| ▓▓                                                                                                | ▓▓               
                               \▓▓    ▓▓                                                                                                | ▓▓               
                                \▓▓▓▓▓▓                                                                                                  \▓▓               
";
    
    public static readonly string OnlinewordcounterArt = @" ██████╗ ███╗   ██╗██╗     ██╗███╗   ██╗███████╗    ███████╗ ██████╗██████╗ ██╗██████╗ ████████╗    ██╗  ██╗██╗   ██╗███╗   ██╗████████╗███████╗██████╗ 
██╔═══██╗████╗  ██║██║     ██║████╗  ██║██╔════╝    ██╔════╝██╔════╝██╔══██╗██║██╔══██╗╚══██╔══╝    ██║  ██║██║   ██║████╗  ██║╚══██╔══╝██╔════╝██╔══██╗
██║   ██║██╔██╗ ██║██║     ██║██╔██╗ ██║█████╗      ███████╗██║     ██████╔╝██║██████╔╝   ██║       ███████║██║   ██║██╔██╗ ██║   ██║   █████╗  ██████╔╝
██║   ██║██║╚██╗██║██║     ██║██║╚██╗██║██╔══╝      ╚════██║██║     ██╔══██╗██║██╔═══╝    ██║       ██╔══██║██║   ██║██║╚██╗██║   ██║   ██╔══╝  ██╔══██╗
╚██████╔╝██║ ╚████║███████╗██║██║ ╚████║███████╗    ███████║╚██████╗██║  ██║██║██║        ██║       ██║  ██║╚██████╔╝██║ ╚████║   ██║   ███████╗██║  ██║
 ╚═════╝ ╚═╝  ╚═══╝╚══════╝╚═╝╚═╝  ╚═══╝╚══════╝    ╚══════╝ ╚═════╝╚═╝  ╚═╝╚═╝╚═╝        ╚═╝       ╚═╝  ╚═╝ ╚═════╝ ╚═╝  ╚═══╝   ╚═╝   ╚══════╝╚═╝  ╚═╝
";

    public static readonly string OnlineSearchResultArt = @"________                   __________        
___  __ \_______________  ____  /_  /________
__  /_/ /  _ \_  ___/  / / /_  /_  __/_  ___/
_  _, _//  __/(__  )/ /_/ /_  / / /_ _(__  ) 
/_/ |_| \___//____/ \__,_/ /_/  \__/ /____/  
                                             ";
    
    public static readonly string border = @"===============================================";
}