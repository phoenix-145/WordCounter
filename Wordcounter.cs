using System.Text.RegularExpressions;
using System.IO;
using System.Reflection.Metadata;

namespace WordCounter
{
    internal class Wordcounter
    {
        public static Dictionary<string, int> WordsWithCount = new();
        public static void GetFileLocation()
        {
            string input;
            bool isfilepathvalid;
            
            do
            {
                isfilepathvalid = true;

                Console.Clear();
                Console.WriteLine(AsciiArt.LocalwordcounterArt);
                Console.Write("\nEnter the file path (The file must be a text file.) or enter 0 to exit : ");
                input = Console.ReadLine()!;

                if (input == null || input == "")
                {
                    SlowPrintingText.SlowPrintText($"Invalid input.\n");
                    isfilepathvalid = false;
                }
                else if(input == "0")
                {
                    return;
                }
                else if(!File.Exists(input))
                {
                    SlowPrintingText.SlowPrintText("File not found.\n");
                    isfilepathvalid = false;
                }
                else if(GetFileType(input) == false.ToString() || GetFileType(input) != ".txt")
                {
                    SlowPrintingText.SlowPrintText("File is not a text file.");
                    isfilepathvalid = false;
                }

            }while(isfilepathvalid == false);

            ReadFromFile(input!);
            WriteToFile(); 
        }
        internal static void ReadFromFile(string path)
        {
            using(StreamReader reader = new StreamReader(path))
            {
                string rawtext = reader.ReadToEnd().ToLower();
                string filteredtext = FilterScript(rawtext);
                string[] split_text = filteredtext.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                foreach(string word in split_text)
                {
                    if(WordsWithCount.ContainsKey(word))
                    {
                        WordsWithCount[word] += 1;
                    }
                    else
                    {
                        WordsWithCount.Add(word, 1);
                    }
                }
            }
        }
        internal static void WriteToFile()
        {
            string FileToWriteOutputTo;

            Console.Write("Enter your output file path(default : Output.txt) : ");
            FileToWriteOutputTo = Console.ReadLine()!;

            if(FileToWriteOutputTo == "")
            {
                FileToWriteOutputTo = @"TextFiles\Output.txt";
            }
            if(!File.Exists(FileToWriteOutputTo))
            {
                string prompt = @"The stated file does not exist. Would you like to enter the path again or use default path (TextFiles\Output.txt) ? ";
                string[] options = {"Enter Again", "Use default path", "Create a new text file at the specified path"};
                Menu menu = new Menu(prompt, options);
                int input = menu.Run();

                switch(input)
                {
                    case 0:
                        WriteToFile();
                        return;
                    case 1:
                        FileToWriteOutputTo = @"TextFiles\Output.txt";
                        break;
                    case 2:
                        FileToWriteOutputTo += ".txt";
                        break;
                }
            }

            try
            {
                using(StreamWriter writer = new StreamWriter(FileToWriteOutputTo))
                {
                    var OrderedWordsWithCount = WordsWithCount.OrderByDescending(x => x.Value); 
                    foreach(var word in OrderedWordsWithCount)
                    {
                        writer.WriteLine($"{word.Key} : {word.Value}");
                    } 
                }
                WordsWithCount.Clear();
            }
            catch(Exception ex)
            {
                Console.WriteLine("\nSomething wrong happened : " + ex.Message);
                WriteToFile();
                return;
            }
                
            SlowPrintingText.SlowPrintText($"The output has been successfully written in {FileToWriteOutputTo}."); 
        }
        private static string GetFileType(string input)
        {
            string filetype;
            try
            {
                FileInfo fileInfo= new FileInfo(input);
                filetype = fileInfo.Extension;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false.ToString();
            }
            return filetype;
        }
        private static string FilterScript(string rawtext)
        {
            string filteredtext;

            string Removetimestamps = Regex.Replace(rawtext, @"\d{2}:\d{2}:\d{2},\d{3} --> \d{2}:\d{2}:\d{2},\d{3}", "");
            string Removetags = Regex.Replace(Removetimestamps, @"<.*?>", "");
            string filtertext = Regex.Replace(Removetags, @"[^a-zA-Z\s']", "");
            string Removewhitespace = Regex.Replace(filtertext, @"\s+", " ").Trim();

            filteredtext = Removewhitespace;
            return filteredtext;
        }
    }
}