using System;
using System.Globalization;
using System.Text.RegularExpressions;
namespace BookingAvailability.Models;

public class InputParser
{
    public static (string hotelId, string roomType, DateTime arrival, DateTime departure) ParseInput(string input)
    {
        string pattern = @"\(([^)]+)\)";
        Match match = Regex.Match(input, pattern);
        if (match.Success)
        {
            string[] data = match.Groups[1].Value.Split(",");
            if (data.Length != 3)
            {
                throw new ArgumentException("This function takes exactly 3 arguments!");
            }

            string hotelId = data[0].Trim();
            string roomType = data[2].Trim();
            string[] dates = data[1].Split("-");

            DateTime arrival = DateTime.ParseExact(dates[0].Trim(), "yyyyMMdd", CultureInfo.InvariantCulture);
            DateTime departure = dates.Length > 1
                ? DateTime.ParseExact(dates[1].Trim(), "yyyyMMdd", CultureInfo.InvariantCulture)
                : arrival;

            if (arrival > departure)
            {
                throw new ArgumentException("Departure date can't be before arrival date!");
            }

            return (hotelId, roomType, arrival, departure);
        }

        throw new ArgumentException("Invalid input format");
    }
}
