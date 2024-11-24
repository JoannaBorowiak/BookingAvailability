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
                    ""name"": ""Hotel Test"",
                    ""roomTypes"": [
                        { ""code"": ""SGL"", ""description"": ""Single Room"" }
                    ],
                    ""rooms"": [
                        { ""roomType"": ""SGL"", ""roomId"": ""101"" }
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
                { ""hotelId"": ""H1"", ""arrival"": ""20230901"", ""departure"": ""20230903"", ""roomType"": ""SGL"" }
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
                new Hotel { id = "H1", name = "Test Hotel" }
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
                new Hotel { id = "H1", name = "Test Hotel" }
            };

        // Act
        var result = Program.CheckHotelPresent(hotels, "H2");

        // Assert
        Assert.Null(result);
    }

    // Test for ParseInput
    [Fact]
    public void ParseInput_ValidInput_ReturnsParsedData()
    {
        // Arrange
        string input = "Availability(H1, 20230901-20230903, SGL)";

        // Act
        var (hotelId, roomType, arrivalDate, departureDate) = Program.ParseInput(input);

        // Assert
        Assert.Equal("H1", hotelId);
        Assert.Equal("SGL", roomType);
        Assert.Equal("20230901", arrivalDate);
        Assert.Equal("20230903", departureDate);
    }

    [Fact]
    public void ParseInput_InvalidFormat_ThrowsArgumentException()
    {
        // Arrange
        string input = "InvalidInput";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Program.ParseInput(input));
    }

    [Fact]
    public void ParseInput_DepartureBeforeArrival_ThrowsArgumentException()
    {
        // Arrange
        string input = "Availability(H1, 20230903-20230901, SGL)";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Program.ParseInput(input));
    }

    // Test for CountingsRooms
    [Fact]
    public void CountingsRoomsOfTypeInHotel_ValidRoomType_ReturnsCount()
    {
        // Arrange
        var hotel = new Hotel
        {
            rooms = new List<Room>
                {
                    new Room { roomType = "SGL", roomId = "101" },
                    new Room { roomType = "SGL", roomId = "102" },
                    new Room { roomType = "DBL", roomId = "201" }
                }
        };

        // Act
        int count = Program.CountingsRoomsOfTypeInHotel("SGL", hotel);

        // Assert
        Assert.Equal(2, count);
    }

    [Fact]
    public void CountingsRoomsOfTypeInHotel_NonExistingRoomType_ReturnsZero()
    {
        // Arrange
        var hotel = new Hotel
        {
            rooms = new List<Room>
                {
                    new Room { roomType = "SGL", roomId = "101" }
                }
        };

        // Act
        int count = Program.CountingsRoomsOfTypeInHotel("DBL", hotel);

        // Assert
        Assert.Equal(0, count);
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
                    new RoomType { code = "SGL", description = "Single Room" }
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
                        new Room { roomType = "SGL", roomId = "101" },
                        new Room { roomType = "SGL", roomId = "102" }
                    },
                    roomTypes = new List<RoomType>
                    {
                        new RoomType { code = "SGL" }
                    }
                }
            };

        var bookings = new List<Booking>
            {
                new Booking { hotelId = "H1", arrival = "20230901", departure = "20230903", roomType = "SGL" }
            };

        // Act
        using (var writer = new StringWriter())
        {
            Console.SetOut(writer);
            Program.CheckAvailability(hotels, bookings, "H1", "SGL", "20230904", "20230905");
            string output = writer.ToString();

            // Assert
            Assert.Contains("Number of available rooms: 2", output);
        }
    }
    [Fact]
    public void CheckAvailability_NoRoomsAvailable_ReturnsNoRoomsMessage()
    {
        // Arrange
        var hotels = new List<Hotel>
    {
        new Hotel
        {
            id = "H1",
            rooms = new List<Room>
            {
                new Room { roomType = "SGL", roomId = "101" },
                new Room { roomType = "SGL", roomId = "102" }
            },
            roomTypes = new List<RoomType>
            {
                new RoomType { code = "SGL" }
            }
        }
    };

        var bookings = new List<Booking>
    {
        new Booking { hotelId = "H1", arrival = "20230901", departure = "20230903", roomType = "SGL" },
        new Booking { hotelId = "H1", arrival = "20230902", departure = "20230905", roomType = "SGL" }
    };

        // Act
        using (var writer = new StringWriter())
        {
            Console.SetOut(writer);
            Program.CheckAvailability(hotels, bookings, "H1", "SGL", "20230902", "20230903");
            string output = writer.ToString();

            // Assert
            Assert.Contains("No rooms of type SGL available in hotel H1", output);
        }
    }
    [Fact]
    public void CheckAvailability_Overbooked_ReturnsOverbookedMessage()
    {
        // Arrange
        var hotels = new List<Hotel>
    {
        new Hotel
        {
            id = "H1",
            rooms = new List<Room>
            {
                new Room { roomType = "SGL", roomId = "101" }
            },
            roomTypes = new List<RoomType>
            {
                new RoomType { code = "SGL" }
            }
        }
    };

        var bookings = new List<Booking>
    {
        new Booking { hotelId = "H1", arrival = "20230901", departure = "20230903", roomType = "SGL" },
        new Booking { hotelId = "H1", arrival = "20230902", departure = "20230904", roomType = "SGL" }
    };

        // Act
        using (var writer = new StringWriter())
        {
            Console.SetOut(writer);
            Program.CheckAvailability(hotels, bookings, "H1", "SGL", "20230902", "20230903");
            string output = writer.ToString();

            // Assert
            Assert.Contains("SGL rooms are currently overbooked", output);
        }
    }
}