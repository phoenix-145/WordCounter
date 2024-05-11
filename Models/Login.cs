public class Login
{
    public string? token { get; set; }
    public int status { get; set; } 
    public User? user { get; set; }
    public class User
    {
        public int allowed_downloads { get; set; }
    }
}