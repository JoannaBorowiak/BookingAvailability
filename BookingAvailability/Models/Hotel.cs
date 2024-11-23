namespace BookingAvailability.Models
{
    public class Hotel
    {
        public string id {  get; set; }
        public string name { get; set; }
        public List<RoomType> roomTypes { get; set; }
        public List<Room> rooms { get; set; }
    }
}
