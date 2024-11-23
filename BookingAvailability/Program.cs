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

        Console.WriteLine("The system is ready. Write what you want to do?");
        Console.WriteLine("Available comands are: Availability");

        do
        {
            string input = Console.ReadLine();
            if (input == null) break;
            (string hotelId, string roomType, string arrivalDate, string departureDate) = ParseInput(input);

            switch (input.Substring(0, input.IndexOf("(")))
            {
                case "Availability":
                    CheckAvailability(hotels, bookings, hotelId, roomType, arrivalDate, departureDate);
                    break;
                case "Book":
                    Console.WriteLine("This feature is unvailable yet!");
                    break;
            }
        } while (true);
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
            string[] data = match.Groups[1].Value.ToUpper().Split(",");

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

    static Hotel CheckHotelAvailability(List<Hotel> hotels, string hotelId)
    {
        Hotel foundHotel = null;
        foreach (var hotel in hotels)
        {
            if (hotel.id == hotelId)
            {
                foundHotel = hotel;
            }
        }
        return foundHotel;
    }
    static bool CheckRoomTypesInHotel(List<Hotel> hotels, string hotelId, string roomType, Hotel foundHotel)
    {
        bool roomTypeExist = false;
        foreach (var roomTypeHotel in foundHotel.roomTypes)
        {
            if (roomTypeHotel.code == roomType)
            {
                roomTypeExist = true;
            }
        }
        if (!roomTypeExist)
        {
            Console.WriteLine($"Room type {roomType} doesn't exist in hotel {foundHotel}");
        }
        return roomTypeExist;
    }
    //Countings rooms
    static int CountingsRoomsTypeInHotel(List<Hotel> hotels, string hotelId, string roomType, Hotel foundHotel)
    {
        int roomCount = 0;
        foreach (Room room in foundHotel.rooms)
        {
            if (room.roomType == roomType)
            {
                roomCount++;
            }
        }
        return roomCount;
    }

    static void CheckAvailability(List<Hotel> hotels, List<Booking> bookings, string hotelId, string roomType, string arrivalDate, string departureDate)
    {
        Hotel foundHotel = CheckHotelAvailability(hotels, hotelId);
        if (foundHotel == null)
        {
            Console.WriteLine($"Hotel {hotelId} doesn't exist");
        }
        if (!CheckRoomTypesInHotel(hotels, hotelId, roomType, foundHotel))
        {
            Console.WriteLine($"Hotel {hotelId} doesn't have rooms of type {roomType}");
        }
        else
        {
        int roomCount = CountingsRoomsTypeInHotel(hotels, hotelId, roomType, foundHotel);
            foreach (Booking booking in bookings)
            {
                //Arrival after reservation start
                bool arrivalAfterBookingArrival = ConvertStringToDate(arrivalDate) >= ConvertStringToDate(booking.arrival);
                //Arrival before reservation end
                bool arrivalBeforeBookingDeparture = ConvertStringToDate(arrivalDate) < ConvertStringToDate(booking.departure);
                //Departure after reservation start
                bool departureAfterBookingArrival = ConvertStringToDate(departureDate) > ConvertStringToDate(booking.arrival);
                //Departure before reservation end
                bool departureBeforeBookingDeparture = ConvertStringToDate(departureDate) <= ConvertStringToDate(booking.departure);
                bool unavaible = (arrivalAfterBookingArrival && arrivalBeforeBookingDeparture) || (departureAfterBookingArrival && departureBeforeBookingDeparture);

                if (hotelId == booking.hotelId && roomType == booking.roomType && unavaible)
                {
                    roomCount--;
                }
            }
            if (roomCount > 0) Console.WriteLine($"Number of available rooms: {roomCount}");
            else if (roomCount == 0) Console.WriteLine($"No rooms of type {roomType} available in hotel {hotelId}");
            else Console.WriteLine($"{roomType} rooms are currently overbooked");
        }
    }
}