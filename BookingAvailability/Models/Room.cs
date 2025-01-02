using System.Text.Json.Serialization;

namespace BookingAvailability.Models
{
    public class Room
    {
        [JsonPropertyName("roomType")]
        public required string RoomType { get; set; }
        [JsonPropertyName("roomId")]
        public required string RoomId {  get; set; }
    }
}
