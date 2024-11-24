using BookingAvailability.Models;
using Xunit;

namespace BookingAvailability.Tests;

public class UnitTest1
{
    // Test for LoadHotels
    [Fact]
    public void LoadHotels_ValidFile_ReturnsHotels()
    {
        // Arrange
        string filePath = "hotels.json";
        File.WriteAllText(filePath, @"
    [
        {
            ""id"": ""H1"",
            ""name"": ""Hotel California"",
            ""roomTypes"": [
                { ""code"": ""SGL"", ""description"": ""Single Room"", ""amenities"": [""WiFi"", ""TV""], ""features"": [""Non-smoking""] },
                { ""code"": ""DBL"", ""description"": ""Double Room"", ""amenities"": [""WiFi"", ""TV"", ""Minibar""], ""features"": [""Non-smoking"", ""Sea View""] }
            ],
            ""rooms"": [
                { ""roomType"": ""SGL"", ""roomId"": ""101"" },
                { ""roomType"": ""DBL"", ""roomId"": ""201"" }
            ]
        }
    ]");

        // Act
        var hotels = Program.LoadHotels(filePath);

        // Assert
        Assert.NotNull(hotels);
        Assert.Single(hotels);
        Assert.Equal("H1", hotels[0].id);

        // Cleanup
        File.Delete(filePath);
    }
    // Test for LoadBookings
    [Fact]
    public void LoadBookings_ValidFile_ReturnsBookings()
    {
        // Arrange
        string filePath = "bookings.json";
        File.WriteAllText(filePath, @"
    [
        {
            ""hotelId"": ""H1"",
            ""arrival"": ""20240901"",
            ""departure"": ""20240903"",
            ""roomType"": ""DBL"",
            ""roomRate"": ""Prepaid""
        }
    ]");

        // Act
        var bookings = Program.LoadBookings(filePath);

        // Assert
        Assert.NotNull(bookings);
        Assert.Single(bookings);
        Assert.Equal("H1", bookings[0].hotelId);

        // Cleanup
        File.Delete(filePath);
    }
    // Test for CheckHotelPresent
    [Fact]
    public void CheckHotelPresent_ExistingHotel_ReturnsHotel()
    {
        // Arrange
        var hotels = new List<Hotel>
    {
        new Hotel { id = "H1", name = "Hotel California" }
    };

        // Act
        var result = Program.CheckHotelPresent(hotels, "H1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("H1", result.id);
    }
    [Fact]
    public void CheckHotelPresent_NonExistingHotel_ReturnsNull()
    {
        // Arrange
        var hotels = new List<Hotel>
    {
        new Hotel { id = "H1", name = "Hotel California" }
    };

        // Act
        var result = Program.CheckHotelPresent(hotels, "H2");

        // Assert
        Assert.Null(result);
    }

    // Test for CountingsRooms
    [Fact]
    public void CountingsRoomsOfTypeInHotel_ValidRoomType_ReturnsCorrectCount()
    {
        // Arrange
        var hotel = new Hotel
        {
            rooms = new List<Room>
        {
            new Room { roomType = "SGL", roomId = "101" },
            new Room { roomType = "DBL", roomId = "201" },
            new Room { roomType = "SGL", roomId = "102" }
        }
        };

        // Act
        int count = Program.CountingsRoomsOfTypeInHotel("SGL", hotel);

        // Assert
        Assert.Equal(2, count);
    }
    // Test for CheckRoomTypesInHotel
    [Fact]
    public void CheckRoomTypesInHotel_ExistingRoomType_ReturnsTrue()
    {
        // Arrange
        var hotel = new Hotel
        {
            roomTypes = new List<RoomType>
        {
            new RoomType { code = "SGL", description = "Single Room" },
            new RoomType { code = "DBL", description = "Double Room" }
        }
        };

        // Act
        bool result = Program.CheckRoomTypesInHotel("SGL", hotel);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CheckRoomTypesInHotel_NonExistingRoomType_ReturnsFalse()
    {
        // Arrange
        var hotel = new Hotel
        {
            roomTypes = new List<RoomType>
        {
            new RoomType { code = "SGL", description = "Single Room" }
        }
        };

        // Act
        bool result = Program.CheckRoomTypesInHotel("DBL", hotel);

        // Assert
        Assert.False(result);
    }
    //Test for CheckAvailability
    [Fact]
    public void CheckAvailability_RoomsAvailable_ReturnsCorrectCount()
    {
        // Arrange
        var hotels = new List<Hotel>
    {
        new Hotel
        {
            id = "H1",
            rooms = new List<Room>
            {
                new Room { roomType = "DBL", roomId = "201" },
                new Room { roomType = "DBL", roomId = "202" }
            },
            roomTypes = new List<RoomType>
            {
                new RoomType { code = "DBL" }
            }
        }
    };

        var bookings = new List<Booking>
    {
        new Booking { hotelId = "H1", arrival = "20240901", departure = "20240903", roomType = "DBL" }
    };

        // Act
        using (var writer = new StringWriter())
        {
            Console.SetOut(writer);
            Program.CheckAvailability(hotels, bookings, "H1", "DBL", "20240904", "20240905");
            string output = writer.ToString();

            // Assert
            Assert.Contains("Number of available rooms: 2", output);
        }
    }

    [Fact]
    public void CheckAvailability_NoRoomsAvailable_ReturnsMessage()
    {
        // Arrange
        var hotels = new List<Hotel>
    {
        new Hotel
        {
            id = "H1",
            rooms = new List<Room>
            {
                new Room { roomType = "DBL", roomId = "201" },
                new Room { roomType = "DBL", roomId = "202" }
            },
            roomTypes = new List<RoomType>
            {
                new RoomType { code = "DBL" }
            }
        }
    };

        var bookings = new List<Booking>
    {
        new Booking { hotelId = "H1", arrival = "20240901", departure = "20240903", roomType = "DBL" },
        new Booking { hotelId = "H1", arrival = "20240902", departure = "20240905", roomType = "DBL" }
    };

        // Act
        using (var writer = new StringWriter())
        {
            Console.SetOut(writer);
            Program.CheckAvailability(hotels, bookings, "H1", "DBL", "20240902", "20240903");
            string output = writer.ToString();

            // Assert
            Assert.Contains("No rooms of type DBL available in hotel H1", output);
        }
    }


}