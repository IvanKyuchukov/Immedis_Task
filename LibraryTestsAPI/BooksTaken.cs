namespace LibraryTestsAPI
{
    public class BooksTaken
    {
        public int id { get; set; }
        public int userId { get; set; }
        public int bookId { get; set; }
        public string dateTaken { get; set; }
        public string book { get; set; }
        public string user { get; set; }
    }
}