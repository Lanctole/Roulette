namespace Games.Classes
{
    public class SupportedLanguage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Russian { get; set; }
        public ICollection<Game> Games { get; set; }
    }
}
