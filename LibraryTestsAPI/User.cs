using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace LibraryTestsAPI
{
    public class User
    {
        [JsonPropertyName("id")]
        public int id { get; set; }
        [JsonPropertyName("name")]
        public string name { get; set; }
        [JsonPropertyName("booksTaken")]
        public List<BooksTaken> booksTaken { get; set; }
    }
}