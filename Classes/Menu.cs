using System.Globalization;
using static System.Console;
public class Menu
{
    private int SelectedIndex;
    private string[] Options;
    private string Prompt;

    public Menu(string prompt, string[] options)
    {
        Prompt = prompt;
        Options = options;
        SelectedIndex = 0;
    }
    public void DisplayForOptions()
    {
        WriteLine(Prompt);
        for(int i = 0; i < Options.Length; i++)
        {
            if(i > Options.Length - 1)
            {
                break;
            }
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

        ResetColor();
    }
    public dynamic Run()
    {
        ConsoleKey consoleKey;
        do
        {
            CursorVisible = false;
            DisplayForOptions();
            ConsoleKeyInfo keyInfo = ReadKey(false);
            consoleKey = keyInfo.Key;
            SetCursorPosition(0, GetCursorPosition().Top - (Options.Length + 1));
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
        SetCursorPosition(0, GetCursorPosition().Top + Options.Length + 1);
        CursorVisible = true;
        if(consoleKey == ConsoleKey.Spacebar)
        {return NavigationActions.Exit;}
        return SelectedIndex;
    }
}