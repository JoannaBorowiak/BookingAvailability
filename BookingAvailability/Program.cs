using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using BookingAvailability.Models;

class Program
{
    static void Main(string[] args)
    {
        List<Hotel> hotels = LoadHotels(@"..\..\..\hotels.json");
        List<Booking> bookings = LoadBookings(@"..\..\..\bookings.json");

        ParseInput("Availability(H1, 20240901, SGL) ");
    }
    static List<Hotel> LoadHotels(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File {filePath} not found");
        }
        string jsonString = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<List<Hotel>>(jsonString);
    }
    static List<Booking> LoadBookings(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File {filePath} not found");
        }
        string jsonString = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<List<Booking>>(jsonString);
    }
    static (string, string, string, string) ParseInput(string question)
    {
        string pattern = @"\(([^)]+)\)";
        Match match = Regex.Match(question, pattern);
        if (match.Success)
        {
            string[] data = match.Groups[1].Value.Split(",");

            string[] dates = data[1].Split("-");

            string hotelId = data[0].Trim();
            string roomType = data[2].Trim();

            string arrivalDate = dates[0].Trim();
            string departureDate = dates.Length > 1 ? dates[1].Trim() : arrivalDate;
            return (hotelId, roomType, arrivalDate, departureDate);
        }
        else
        {
            throw new ArgumentException("Wrong input format");
        }
    }
    static DateTime ConvertStringToDate(string date)
    {
        var newDate = DateTime.ParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture);
        return newDate;
    }
}
