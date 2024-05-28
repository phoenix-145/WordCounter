using System.Text;
using static System.Console;
public class Menu
{
    private int SelectedIndex;
    private string[] Options;
    private string Prompt;
    private Subtitles? Subtitles;
    private string? OptionDesc;
    public Menu(string prompt, string[] options)
    {
        Prompt = prompt;
        Options = options;
        SelectedIndex = 0;
    }
    public Menu(string prompt, string[] options, Subtitles subtitles)
    {
        Prompt = prompt;
        Options = options;
        SelectedIndex = 0;
        Subtitles = subtitles;
    }
    private void DisplayForOptions()
    {
        WriteLine(Prompt);
        for(int i = 0; i < Options.Length; i++)
        {
            string prefix;
            
            if(i == SelectedIndex)
            {
                prefix = ">>";
                ForegroundColor = ConsoleColor.Black;
                BackgroundColor = ConsoleColor.White;
            }
            else{prefix = "  ";ForegroundColor = ConsoleColor.White; BackgroundColor = ConsoleColor.Black;}
            WriteLine($"{prefix} " + $"{Options[i]}");
        }
        if(Subtitles != null)
        {
            ResetColor();
            int Cursorposition = GetCursorPosition().Top;
            if(OptionDesc != null)
            {
                SetCursorPosition(0, GetCursorPosition().Top + 1);
                Write(new string(' ', WindowWidth));
                SetCursorPosition(0, GetCursorPosition().Top + 1);
                Write(new string(' ', WindowWidth));
                SetCursorPosition(0, Cursorposition);
            }
            WriteLine(CreateOptionsDesc());  
        }
        ResetColor();
    }
    public dynamic Run()
    {
        ConsoleKey consoleKey;
        int offset;
        do
        {
            CursorVisible = false;
            DisplayForOptions();
            ConsoleKeyInfo keyInfo = ReadKey(false);
            consoleKey = keyInfo.Key;
            offset = Subtitles == null ? Options.Length + 1 : Options.Length + 4;
            SetCursorPosition(0, GetCursorPosition().Top - offset);
            if (consoleKey == ConsoleKey.UpArrow)
            {
                SelectedIndex--;
                if (SelectedIndex == -1){SelectedIndex = Options.Length -1 ;}
            }
            else if(consoleKey == ConsoleKey.DownArrow)
            {
                SelectedIndex++;
                if (SelectedIndex == Options.Length){SelectedIndex = 0;}
            }   
        }
        while(consoleKey != ConsoleKey.Enter && consoleKey != ConsoleKey.Spacebar);
        SetCursorPosition(0, GetCursorPosition().Top + offset);
        CursorVisible = true;
        if(consoleKey == ConsoleKey.Spacebar)
        {return NavigationActions.Exit;}
        return SelectedIndex;
    }
    private string CreateOptionsDesc()
    {
        var sb = new StringBuilder();
        sb.Append($"\nAuthor : {Subtitles!.data![SelectedIndex].attributes!.uploader!.name} | " +
                    $"Author's rank : {Subtitles!.data![SelectedIndex].attributes!.uploader!.rank} | " + 
                    $"language : {Subtitles!.data![SelectedIndex].attributes!.language} | " + 
                    $"Download Count : {Subtitles!.data![SelectedIndex].attributes!.download_count.ToString()} | " +
                    
                    $"\nTrusted : {Subtitles!.data![SelectedIndex].attributes!.from_trusted.ToString()} | " +
                    $"Upload date : {Subtitles!.data![SelectedIndex].attributes!.upload_date} | "  +
                    $"AI translated : {Subtitles!.data![SelectedIndex].attributes!.ai_translated.ToString()} | " +
                    $"Machine translated : {Subtitles!.data![SelectedIndex].attributes!.machine_translated.ToString()} | ");
        
        OptionDesc = sb.ToString();
        return sb.ToString();   
    }
}