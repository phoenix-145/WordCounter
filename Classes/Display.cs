
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
    }
}