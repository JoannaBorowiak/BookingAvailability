using System;
using BookingAvailability.Models;

namespace BookingAvailability
{
    class Program
    {
        static void Main(string[] args)
        {
            string hotelsPath = args.Length > 0 ? args[0] : @"hotels.json";
            string bookingsPath = args.Length > 1 ? args[1] : @"bookings.json";

            var hotelManager = new HotelManager(hotelsPath);
            var bookingManager = new BookingManager(bookingsPath);

            Console.WriteLine("The system is ready. Write what you want to do?");
            Console.WriteLine("Available commands: Availability");

            while (true)
            {
                string? input = Console.ReadLine();
                if (string.IsNullOrEmpty(input)) break;

                try
                {
                    (string hotelId, string roomType, DateTime arrival, DateTime departure) = InputParser.ParseInput(input);

                    var hotel = hotelManager.GetHotelById(hotelId);
                    if (hotel == null)
                    {
                        Console.WriteLine($"Hotel {hotelId} doesn't exist");
                        continue;
                    }

                    if (!hotelManager.IsRoomTypeInHotel(hotel, roomType))
                    {
                        Console.WriteLine($"Hotel {hotelId} doesn't have rooms of type {roomType}");
                        continue;
                    }

                    int roomCount = hotelManager.GetRoomCountByType(hotel, roomType);
                    bool isAvailable = bookingManager.IsRoomAvailable(hotelId, roomType, arrival, departure);

                    Console.WriteLine(isAvailable
                        ? $"Number of available rooms: {roomCount}"
                        : $"No rooms of type {roomType} available in hotel {hotelId}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error: {e.Message}");
                }
            }
        }
    }

}
