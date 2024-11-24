# Booking Availability Checker

This program allows you to check room availability in a hotel based on provided data about hotels and existing bookings. It uses JSON files as input and provides a command-line interface to interact with the data.

## Features

- **Room Availability Check**: Verify the availability of specific room types in given hotel for a given date range.
- **Data Input via JSON Files**: Loads data from `hotels.json` and `bookings.json`.
- **Interactive CLI**: Simple text-based input to perform operations.

## How to Use

### Prerequisites

Ensure you have:
- **.NET 6.0 SDK or later** installed on your system.
- Valid JSON files: `hotels.json` and `bookings.json`.

### Running the Program

1. Clone the repository or download the project files to a local directory.

2. Open a terminal in the project directory.

3. Run the application using:
   ```bash
   dotnet run hotels.json bookings.json
    ```
    Alternatively, if your JSON files are located in custom paths, specify their paths as arguments:
    ```bash
    dotnet run "path/to/hotels.json" "path/to/bookings.json"
    ```
4. The program will start and display the following prompt:
    ```scss
    The system is ready. Write what you want to do?
    Available commands are: Availability
    ```
5. Use the Availability command to check room availability. The syntax is:
    ```scss
    Availability(hotelId, YYYYMMDD-YYYYMMDD, roomType)
    ```
    - hotelId: The unique identifier of the hotel (e.g., H1).
    - YYYYMMDD-YYYYMMDD: The date range in the format YYYYMMDD (arrival and departure dates). If the arrival and departure are the same, use YYYYMMDD.
    - roomType: The type of room to check (e.g., SGL, DBL).
    
    Example: 
    ```scss
    Availability(H1, 20240101-20240103, DBL)
    ```
## Running Tests
You can test the program interactively by providing different JSON files and running commands in the CLI. For automated tests, implement unit tests for key methods like CheckAvailability, ParseInput, and LoadHotels using a test framework such as xUnit or NUnit. Run automated tests with:

```bash
dotnet test
```
## Error Handling
- **Invalid File Paths**: The program will notify you if the hotels.json or bookings.json file cannot be found.
- **Invalid Input Format**: Commands that do not match the required format (e.g., incorrect date format or missing arguments) will produce an error message.
- **Unavailable Features**: The program will notify you if an unsupported feature is requested.
- **Date Range Issues**: If the departure date is earlier than the arrival date, an error will be displayed.
- **Hotel Not Found**: The specified hotelId does not exist in the provided hotel database. Please verify the hotelId and try again.
- **Room Type Not Available**: The requested roomType does not exist for the specified hotel. Check the hotel's available room types and provide a valid roomType.
Ensure input files and commands are formatted correctly to avoid errors.
    