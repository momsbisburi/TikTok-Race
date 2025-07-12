using Newtonsoft.Json;

namespace TikFinity.Models
{
    public class Event
    {
        [JsonProperty("event")]
        public string _Event { get; set; }

        [JsonProperty("data")]
        public object Data { get; set; }
    }
}
