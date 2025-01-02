using System.Text.Json.Serialization;

namespace BookingAvailability.Models
{
    public class Booking
    {
        [JsonPropertyName("hotelId")]
        public required string HotelId { get; set; }
        [JsonPropertyName("arrival")]
        public required string Arrival { get; set; }
        [JsonPropertyName("departure")]
        public required string Departure { get; set; }
        [JsonPropertyName("roomType")]
        public required string RoomType { get; set; }
        [JsonPropertyName("roomRate")]
        public required string RoomRate { get; set; }
    }
}