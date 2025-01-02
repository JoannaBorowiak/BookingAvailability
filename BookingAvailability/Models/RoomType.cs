using System.Text.Json.Serialization;

namespace BookingAvailability.Models
{
    public class RoomType
    {
        [JsonPropertyName("code")]
        public required string Code {  get; set; }
        [JsonPropertyName("description")]
        public required string Description {  get; set; }
        [JsonPropertyName("amenities")]
        public required List<string> Amenities { get; set; }
        [JsonPropertyName("features")]
        public required List<string> Features { get; set; }
    }
}