using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
namespace BookingAvailability.Models;

public class BookingManager
{
    public List<Booking> Bookings { get; private set; }

    public BookingManager(string bookingsFilePath)
    {
        Bookings = LoadBookings(bookingsFilePath);
    }

    private List<Booking> LoadBookings(string filePath)
    {
        if (!File.Exists(filePath)) throw new FileNotFoundException($"File {filePath} not found");
        string jsonString = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<List<Booking>>(jsonString) ?? new List<Booking>();
    }

    public bool IsRoomAvailable(string hotelId, string roomType, DateTime arrival, DateTime departure)
    {
        return !Bookings.Any(b =>
            b.HotelId == hotelId &&
            b.RoomType == roomType &&
            arrival < DateTime.ParseExact(b.Departure, "yyyyMMdd", null) &&
            departure > DateTime.ParseExact(b.Arrival, "yyyyMMdd", null));
    }
}
