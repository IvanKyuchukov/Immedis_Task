using System.Text.Json.Serialization;

namespace LibraryTestsAPI
{
    public class Book
    {
        [JsonPropertyName("id")]
        public int id { get; set; }
        [JsonPropertyName("name")]
        public string name { get; set; }
        [JsonPropertyName("author")]
        public string author { get; set; }
        [JsonPropertyName("genre")]
        public string genre { get; set; }
        [JsonPropertyName("quontity")]
        public int quontity { get; set; }
        [JsonPropertyName("booksTaken")]
        public List<BooksTaken> booksTaken { get; set; }
    }
}