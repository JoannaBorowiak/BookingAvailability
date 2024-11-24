using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using BookingAvailability.Models;
namespace BookingAvailability
{
    class Program
    {
        static void Main()
        {
            //Load data from json files
            List<Hotel> ?hotels = LoadHotels(@"..\..\..\hotels.json");
            List<Booking> ?bookings = LoadBookings(@"..\..\..\bookings.json");
            
            if (hotels == null || bookings == null)
            {
                Console.WriteLine("Error reading input files. The program will exit now");
            }
            else
            {
                Console.WriteLine("The system is ready. Write what you want to do?");
                Console.WriteLine("Available comands are: Availability");
                //Main program loop
                while (true)
                {
                    string? input = Console.ReadLine();
                    if (input == null) break;
                    (string hotelId, string roomType, string arrivalDate, string departureDate) = ParseInput(input);

                    switch (input.Substring(0, input.IndexOf('(')))
                    {
                        case "Availability":
                            CheckAvailability(hotels, bookings, hotelId, roomType, arrivalDate, departureDate);
                            break;
                        case "Book":
                            Console.WriteLine("This feature is unvailable yet!");
                            break;
                    }
                }
            }
        }
        
        /// <summary>
        /// Load hotels data from json file.
        /// </summary>
        /// <returns>List of hotel class instances</returns>
        static public List<Hotel>? LoadHotels(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File {filePath} not found");
                return null;
            }
            string jsonString = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<Hotel>>(jsonString);
        }
        
        /// <summary>
        /// Load bookings data from json file.
        /// </summary>
        /// <returns>List of bookings class instances</returns>
        static public List<Booking>? LoadBookings(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File {filePath} not found");
                return null;
            }
            string jsonString = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<Booking>>(jsonString);
        }
        
        /// <summary>
        /// Parses string input to string variables
        /// </summary>
        /// <returns>Tupple of four strings</returns>
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
        
        /// <summary>
        /// Converts string value to DateTime
        /// </summary>
        /// <returns>DateTime object</returns>
        static DateTime ConvertStringToDate(string date)
        {
            var newDate = DateTime.ParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture);
            return newDate;
        }
        
        /// <summary>
        /// Checks if hotelId is present in list of hotels
        /// </summary>
        /// <returns>Hotel object of hotel with hotelId</returns>
        static public Hotel? CheckHotelPresent(List<Hotel> hotels, string hotelId)
        {
            Hotel ?foundHotel = null;
            foreach (var hotel in hotels)
            {
                if (hotel.id == hotelId)
                {
                    foundHotel = hotel;
                }
            }
            return foundHotel;
        }

        /// <summary>
        /// Checks if roomType is present in hotel object
        /// </summary>
        /// <returns>Boolean value if roomType exists in hotel object</returns>
        static public bool CheckRoomTypesInHotel(string roomType, Hotel foundHotel)
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

        /// <summary>
        /// Countings rooms of roomType in hotel object
        /// </summary>
        /// <returns>Number of rooms of roomType in hotel object</returns>
        static public int CountingsRoomsOfTypeInHotel(string roomType, Hotel foundHotel)
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

        /// <summary>
        /// Checking availability of given roomType in hotel with hoteId at the time between arrivalDate and departureDate
        /// </summary>
        static public void CheckAvailability(List<Hotel> hotels, List<Booking> bookings, string hotelId, string roomType, string arrivalDate, string departureDate)
        {
            Hotel ?foundHotel = CheckHotelPresent(hotels, hotelId);
            if (foundHotel == null)
            {
                Console.WriteLine($"Hotel {hotelId} doesn't exist");
            }
            else if (!CheckRoomTypesInHotel( roomType, foundHotel))
            {
                Console.WriteLine($"Hotel {hotelId} doesn't have rooms of type {roomType}");
            }
            else
            {
                int roomCount = CountingsRoomsOfTypeInHotel(roomType, foundHotel);
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
}
