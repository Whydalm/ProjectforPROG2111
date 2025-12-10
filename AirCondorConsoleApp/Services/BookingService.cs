using System;
using MySql.Data.MySqlClient;

namespace AirCondorConsoleApp.Services
{
    /// <summary>
    /// Provides basic CRUD operations for bookings.
    /// </summary>
    public static class BookingService
    {
        /// <summary>
        /// Creates a new booking for an existing customer and flight.
        /// </summary>
        public static void CreateBooking()
        {
            Console.WriteLine("\n--- Create Booking ---");

            Console.Write("Enter existing CustomerID: ");
            string customerInput = Console.ReadLine();

            Console.Write("Enter existing FlightID: ");
            string flightInput = Console.ReadLine();

            Console.Write("Enter payment status (e.g., Paid / Pending): ");
            string paymentStatus = Console.ReadLine();

            Console.Write("Enter total amount (e.g., 350.00): ");
            string amountInput = Console.ReadLine();

            if (!int.TryParse(customerInput, out int customerId) ||
                !int.TryParse(flightInput, out int flightId) ||
                !decimal.TryParse(amountInput, out decimal totalAmount))
            {
                Console.WriteLine("Invalid numeric input. Booking was not created.");
                return;
            }

            const string query = @"
                INSERT INTO Booking
                    (CustomerID, FlightID, BookingDate, PaymentStatus, TotalAmount)
                VALUES
                    (@cust, @flight, @date, @status, @amount);";

            try
            {
                using (var conn = Database.GetConnection())
                using (var cmd = new MySqlCommand(query, conn))
                {
                    conn.Open();

                    cmd.Parameters.AddWithValue("@cust", customerId);
                    cmd.Parameters.AddWithValue("@flight", flightId);
                    cmd.Parameters.AddWithValue("@date", DateTime.Now);
                    cmd.Parameters.AddWithValue("@status", paymentStatus);
                    cmd.Parameters.AddWithValue("@amount", totalAmount);

                    int rows = cmd.ExecuteNonQuery();

                    Console.WriteLine(rows > 0
                        ? "Booking created successfully!"
                        : "Booking creation failed.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error creating booking:");
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Displays all bookings with customer and flight information.
        /// </summary>
        public static void ViewAllBookings()
        {
            const string query = @"
                SELECT
                    b.BookingID,
                    c.FirstName AS CustomerFirstName,
                    c.LastName AS CustomerLastName,
                    f.FlightNumber,
                    f.DepartureAirportCode,
                    f.ArrivalAirportCode,
                    b.BookingDate,
                    b.PaymentStatus,
                    b.TotalAmount
                FROM Booking b
                JOIN Customer c ON b.CustomerID = c.CustomerID
                JOIN Flight f ON b.FlightID = f.FlightID
                ORDER BY b.BookingDate DESC;";

            try
            {
                using (var conn = Database.GetConnection())
                using (var cmd = new MySqlCommand(query, conn))
                {
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("\n--- Booking List ---");

                        while (reader.Read())
                        {
                            Console.WriteLine(
                                $"BookingID: {reader["BookingID"]} | " +
                                $"Customer: {reader["CustomerFirstName"]} {reader["CustomerLastName"]} | " +
                                $"Flight: {reader["FlightNumber"]} " +
                                $"({reader["DepartureAirportCode"]} -> {reader["ArrivalAirportCode"]}) | " +
                                $"Date: {reader["BookingDate"]} | " +
                                $"Status: {reader["PaymentStatus"]} | " +
                                $"Total: {reader["TotalAmount"]} CAD");
                        }

                        Console.WriteLine("---------------------\n");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading bookings:");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
