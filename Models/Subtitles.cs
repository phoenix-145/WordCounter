using System.Text.Json.Serialization;

public class Subtitles
{
    public int total_pages { get; set; }    
    public int total_count { get; set; }
    public Data[]? data{ get; set; }
    public class Data
    {
        public string? type { get; set; }
        public SubtitleAttributes? attributes { get; set; }
        public class SubtitleAttributes
        {
            public string? language { get; set; }
            public int download_count { get; set; }
            public bool from_trusted { get; set; }
            public string? upload_date { get; set; }
            public bool ai_translated { get; set; }
            public bool machine_translated { get; set; }
            public string? comments { get; set; }
            public Uploader? uploader { get; set; }
            public class Uploader
            {
                public int? uploader_id { get; set; }
                public string? name { get; set; }
                public string? rank { get; set; }
            }
            public File[]? files{ get; set; }
            public class File
            {
                public string? file_name { get; set; }  
                public int file_id { get; set; }    
            }
        }
    }
}