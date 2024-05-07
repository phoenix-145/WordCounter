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
    public int Run()
    {
        ConsoleKey consoleKey;
        do
        {
            CursorVisible = false;
            DisplayForOptions();
            SetCursorPosition(0, GetCursorPosition().Top - (Options.Length + 1));
            ConsoleKeyInfo keyInfo = ReadKey(false);
            consoleKey = keyInfo.Key;
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
        while(consoleKey != ConsoleKey.Enter);
        SetCursorPosition(0, GetCursorPosition().Top + (Options.Length + 1));
        CursorVisible = true;
        return SelectedIndex;
    }
}