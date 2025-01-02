using System;
namespace BookingAvailability.Models;
using System.Collections.Generic;
using System.Text.Json;

public class HotelManager
{
    public List<Hotel> Hotels { get; private set; }

    public HotelManager(string hotelsFilePath)
    {
        Hotels = LoadHotels(hotelsFilePath);
    }

    private List<Hotel> LoadHotels(string filePath)
    {
        if (!File.Exists(filePath)) throw new FileNotFoundException($"File {filePath} not found");
        string jsonString = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<List<Hotel>>(jsonString) ?? new List<Hotel>();
    }

    public Hotel? GetHotelById(string hotelId)
    {
        return Hotels.FirstOrDefault(h => h.Id == hotelId);
    }

    public bool IsRoomTypeInHotel(Hotel hotel, string roomType)
    {
        return hotel.RoomTypes.Any(rt => rt.Code == roomType);
    }

    public int GetRoomCountByType(Hotel hotel, string roomType)
    {
        return hotel.Rooms.Count(r => r.RoomType == roomType);
    }
}

