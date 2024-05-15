public class Tmdb_Response
{
    public int page { get; set; }
    public Result[]? results { get; set; }
    public int total_pages { get; set; }
    public int total_results { get; set; }
    public class Result
    {
        public int id { get; set; }
        public string? original_title { get; set; }
        public string? overview { get; set; }
        public string? poster_path { get; set; }
        public string? release_date { get; set; }
        public string? title { get; set; }

    }
}