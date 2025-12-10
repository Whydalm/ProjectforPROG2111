using System;
using MySql.Data.MySqlClient;

namespace AirCondorConsoleApp.Services
{
    /// <summary>
    /// Provides operations for linking bookings and passengers
    /// via the BookingPassenger table.
    /// </summary>
    public static class BookingPassengerService
    {
        /// <summary>
        /// Links an existing passenger to an existing booking.
        /// </summary>
        public static void AddPassengerToBooking()
        {
            Console.WriteLine("\n--- Add Passenger To Booking ---");

            // 1. Show all bookings so user can choose
            Console.WriteLine("\nAvailable bookings:");
            const string bookingQuery = @"
                SELECT b.BookingID, c.FirstName, c.LastName, f.FlightNumber, b.BookingDate
                FROM Booking b
                JOIN Customer c ON b.CustomerID = c.CustomerID
                JOIN Flight f ON b.FlightID = f.FlightID
                ORDER BY b.BookingID;";

            try
            {
                using (var conn = Database.GetConnection())
                using (var cmd = new MySqlCommand(bookingQuery, conn))
                {
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine(
                                $"BookingID: {reader["BookingID"]} | " +
                                $"Customer: {reader["FirstName"]} {reader["LastName"]} | " +
                                $"Flight: {reader["FlightNumber"]} | " +
                                $"Date: {reader["BookingDate"]}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading bookings:");
                Console.WriteLine(ex.Message);
                return;
            }

            Console.Write("\nEnter BookingID: ");
            if (!int.TryParse(Console.ReadLine(), out int bookingId))
            {
                Console.WriteLine("Invalid BookingID.");
                return;
            }

            // 2. Show all passengers so user can choose
            Console.WriteLine("\nAvailable passengers:");
            const string passengerQuery = @"
                SELECT PassengerID, FirstName, LastName
                FROM Passenger
                ORDER BY LastName, FirstName;";

            try
            {
                using (var conn = Database.GetConnection())
                using (var cmd = new MySqlCommand(passengerQuery, conn))
                {
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine(
                                $"PassengerID: {reader["PassengerID"]} | " +
                                $"Name: {reader["FirstName"]} {reader["LastName"]}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading passengers:");
                Console.WriteLine(ex.Message);
                return;
            }

            Console.Write("\nEnter PassengerID: ");
            if (!int.TryParse(Console.ReadLine(), out int passengerId))
            {
                Console.WriteLine("Invalid PassengerID.");
                return;
            }

            // 3. Insert into BookingPassenger
            const string insertQuery = @"
                INSERT INTO BookingPassenger (BookingID, PassengerID)
                VALUES (@bookingId, @passengerId);";

            try
            {
                using (var conn = Database.GetConnection())
                using (var cmd = new MySqlCommand(insertQuery, conn))
                {
                    conn.Open();

                    cmd.Parameters.AddWithValue("@bookingId", bookingId);
                    cmd.Parameters.AddWithValue("@passengerId", passengerId);

                    int rows = cmd.ExecuteNonQuery();

                    Console.WriteLine(rows > 0
                        ? "Passenger successfully linked to booking!"
                        : "Passenger was not linked to booking.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error linking passenger to booking:");
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Shows all passengers for a selected booking.
        /// </summary>
        public static void ViewPassengersForBooking()
        {
            Console.WriteLine("\n--- View Passengers For Booking ---");

            Console.Write("Enter BookingID: ");
            if (!int.TryParse(Console.ReadLine(), out int bookingId))
            {
                Console.WriteLine("Invalid BookingID.");
                return;
            }

            const string query = @"
                SELECT bp.BookingPassengerID,
                       p.PassengerID,
                       p.FirstName,
                       p.LastName
                FROM BookingPassenger bp
                JOIN Passenger p ON bp.PassengerID = p.PassengerID
                WHERE bp.BookingID = @bookingId
                ORDER BY p.LastName, p.FirstName;";

            try
            {
                using (var conn = Database.GetConnection())
                using (var cmd = new MySqlCommand(query, conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@bookingId", bookingId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine($"\nPassengers in booking {bookingId}:");

                        bool any = false;

                        while (reader.Read())
                        {
                            any = true;
                            Console.WriteLine(
                                $"BookingPassengerID: {reader["BookingPassengerID"]} | " +
                                $"PassengerID: {reader["PassengerID"]} | " +
                                $"Name: {reader["FirstName"]} {reader["LastName"]}");
                        }

                        if (!any)
                        {
                            Console.WriteLine("No passengers found for this booking.");
                        }

                        Console.WriteLine("-----------------------\n");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading passengers for booking:");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
