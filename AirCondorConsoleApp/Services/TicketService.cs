using System;
using MySql.Data.MySqlClient;

namespace AirCondorConsoleApp.Services
{
    /// <summary>
    /// Provides operations for seat assignment and tickets.
    /// </summary>
    public static class TicketService
    {
        /// <summary>
        /// Assigns a seat to a passenger in a booking and creates a ticket.
        /// </summary>
        public static void AssignSeatAndCreateTicket()
        {
            Console.WriteLine("\n--- Assign Seat & Create Ticket ---");

            // 1. Ask for BookingID
            Console.Write("Enter BookingID: ");
            if (!int.TryParse(Console.ReadLine(), out int bookingId))
            {
                Console.WriteLine("Invalid BookingID.");
                return;
            }

            // 2. Show passengers in this booking (BookingPassenger + Passenger)
            const string passengersQuery = @"
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
                using (var cmd = new MySqlCommand(passengersQuery, conn))
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
                            Console.WriteLine("No passengers found in this booking.");
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading passengers for booking:");
                Console.WriteLine(ex.Message);
                return;
            }

            Console.Write("\nEnter BookingPassengerID to assign a seat: ");
            if (!int.TryParse(Console.ReadLine(), out int bookingPassengerId))
            {
                Console.WriteLine("Invalid BookingPassengerID.");
                return;
            }

            // 3. Find FlightID and AircraftID for this booking
            int flightId;
            int aircraftId;

            const string flightAircraftQuery = @"
                SELECT b.FlightID, f.AircraftID
                FROM Booking b
                JOIN Flight f ON b.FlightID = f.FlightID
                WHERE b.BookingID = @bookingId;";

            try
            {
                using (var conn = Database.GetConnection())
                using (var cmd = new MySqlCommand(flightAircraftQuery, conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@bookingId", bookingId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            Console.WriteLine("Booking not found or flight not linked.");
                            return;
                        }

                        flightId = Convert.ToInt32(reader["FlightID"]);
                        aircraftId = Convert.ToInt32(reader["AircraftID"]);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading flight/aircraft for booking:");
                Console.WriteLine(ex.Message);
                return;
            }

            // 4. Show available seats for this aircraft (not taken by other tickets on this flight)
            Console.WriteLine($"\nAvailable seats for FlightID {flightId}:");

            const string seatsQuery = @"
                SELECT s.SeatID, s.SeatNumber, s.SeatClass
                FROM Seat s
                WHERE s.AircraftID = @aircraftId
                  AND s.SeatID NOT IN (
                        SELECT t.SeatID
                        FROM Ticket t
                        JOIN BookingPassenger bp ON t.BookingPassengerID = bp.BookingPassengerID
                        JOIN Booking b ON bp.BookingID = b.BookingID
                        WHERE b.FlightID = @flightId
                  )
                ORDER BY s.SeatNumber;";

            try
            {
                using (var conn = Database.GetConnection())
                using (var cmd = new MySqlCommand(seatsQuery, conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@aircraftId", aircraftId);
                    cmd.Parameters.AddWithValue("@flightId", flightId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        bool any = false;

                        while (reader.Read())
                        {
                            any = true;
                            Console.WriteLine(
                                $"SeatID: {reader["SeatID"]} | " +
                                $"Seat: {reader["SeatNumber"]} | " +
                                $"Class: {reader["SeatClass"]}");
                        }

                        if (!any)
                        {
                            Console.WriteLine("No available seats for this flight.");
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading available seats:");
                Console.WriteLine(ex.Message);
                return;
            }

            Console.Write("\nEnter SeatID to assign: ");
            if (!int.TryParse(Console.ReadLine(), out int seatId))
            {
                Console.WriteLine("Invalid SeatID.");
                return;
            }

            // 5. Ask for price and generate ticket number
            Console.Write("Enter price paid (for example 350.00): ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal pricePaid))
            {
                Console.WriteLine("Invalid price.");
                return;
            }

            // Simple auto-generated ticket number
            string ticketNumber = $"TCK-{bookingId}-{bookingPassengerId}-{seatId}";

            // 6. Insert into Ticket
            const string insertTicketQuery = @"
                INSERT INTO Ticket (BookingPassengerID, SeatID, TicketNumber, PricePaid)
                VALUES (@bpId, @seatId, @ticketNumber, @pricePaid);";

            try
            {
                using (var conn = Database.GetConnection())
                using (var cmd = new MySqlCommand(insertTicketQuery, conn))
                {
                    conn.Open();

                    cmd.Parameters.AddWithValue("@bpId", bookingPassengerId);
                    cmd.Parameters.AddWithValue("@seatId", seatId);
                    cmd.Parameters.AddWithValue("@ticketNumber", ticketNumber);
                    cmd.Parameters.AddWithValue("@pricePaid", pricePaid);

                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        Console.WriteLine("\nTicket created successfully!");
                        Console.WriteLine($"TicketNumber: {ticketNumber}");
                    }
                    else
                    {
                        Console.WriteLine("Ticket was not created.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error creating ticket:");
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Shows all tickets for a specific booking.
        /// </summary>
        public static void ViewTicketsForBooking()
        {
            Console.WriteLine("\n--- View Tickets For Booking ---");

            Console.Write("Enter BookingID: ");
            if (!int.TryParse(Console.ReadLine(), out int bookingId))
            {
                Console.WriteLine("Invalid BookingID.");
                return;
            }

            const string query = @"
                SELECT t.TicketID,
                       t.TicketNumber,
                       t.PricePaid,
                       p.FirstName,
                       p.LastName,
                       s.SeatNumber,
                       s.SeatClass
                FROM Ticket t
                JOIN BookingPassenger bp ON t.BookingPassengerID = bp.BookingPassengerID
                JOIN Passenger p ON bp.PassengerID = p.PassengerID
                JOIN Seat s ON t.SeatID = s.SeatID
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
                        Console.WriteLine($"\nTickets for booking {bookingId}:");
                        bool any = false;

                        while (reader.Read())
                        {
                            any = true;
                            Console.WriteLine(
                                $"TicketID: {reader["TicketID"]} | " +
                                $"TicketNumber: {reader["TicketNumber"]} | " +
                                $"Passenger: {reader["FirstName"]} {reader["LastName"]} | " +
                                $"Seat: {reader["SeatNumber"]} ({reader["SeatClass"]}) | " +
                                $"Price: {reader["PricePaid"]}");
                        }

                        if (!any)
                        {
                            Console.WriteLine("No tickets found for this booking.");
                        }

                        Console.WriteLine("-----------------------\n");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading tickets for booking:");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
