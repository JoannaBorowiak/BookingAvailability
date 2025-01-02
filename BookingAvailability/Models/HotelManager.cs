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
        return Hotels.FirstOrDefault(h => h.id == hotelId);
    }

    public bool IsRoomTypeInHotel(Hotel hotel, string roomType)
    {
        return hotel.roomTypes.Any(rt => rt.code == roomType);
    }

    public int GetRoomCountByType(Hotel hotel, string roomType)
    {
        return hotel.rooms.Count(r => r.roomType == roomType);
    }
}

