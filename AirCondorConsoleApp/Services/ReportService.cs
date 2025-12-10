using System;
using MySql.Data.MySqlClient;

namespace AirCondorConsoleApp.Services
{
    /// <summary>
    /// Provides reporting and analytics functions for the Air Condor system.
    /// </summary>
    public static class ReportService
    {
        /// <summary>
        /// Shows all tickets with joined details:
        /// customer, passenger, flight, airports, seat, airline brand.
        /// </summary>
        public static void ShowAllTicketsDetailed()
        {
            Console.WriteLine("\n--- Report: All Tickets (Detailed) ---");

            const string query = @"
                SELECT 
                    t.TicketID,
                    t.TicketNumber,
                    t.PricePaid,
                    p.FirstName AS PassengerFirstName,
                    p.LastName AS PassengerLastName,
                    c.FirstName AS CustomerFirstName,
                    c.LastName AS CustomerLastName,
                    b.BookingID,
                    b.BookingDate,
                    f.FlightNumber,
                    f.DepartureDateTime,
                    f.ArrivalDateTime,
                    dep.AirportCode AS DepCode,
                    dep.City AS DepCity,
                    arr.AirportCode AS ArrCode,
                    arr.City AS ArrCity,
                    s.SeatNumber,
                    s.SeatClass,
                    ab.Name AS BrandName
                FROM Ticket t
                JOIN BookingPassenger bp ON t.BookingPassengerID = bp.BookingPassengerID
                JOIN Booking b ON bp.BookingID = b.BookingID
                JOIN Customer c ON b.CustomerID = c.CustomerID
                JOIN Passenger p ON bp.PassengerID = p.PassengerID
                JOIN Flight f ON b.FlightID = f.FlightID
                JOIN Airport dep ON f.DepartureAirportCode = dep.AirportCode
                JOIN Airport arr ON f.ArrivalAirportCode = arr.AirportCode
                JOIN Seat s ON t.SeatID = s.SeatID
                JOIN AirlineBrand ab ON f.BrandID = ab.BrandID
                ORDER BY t.TicketID;";

            try
            {
                using (var conn = Database.GetConnection())
                using (var cmd = new MySqlCommand(query, conn))
                {
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        bool any = false;

                        while (reader.Read())
                        {
                            any = true;

                            Console.WriteLine(
                                $"TicketID: {reader["TicketID"]} | " +
                                $"TicketNumber: {reader["TicketNumber"]} | " +
                                $"Passenger: {reader["PassengerFirstName"]} {reader["PassengerLastName"]} | " +
                                $"Customer: {reader["CustomerFirstName"]} {reader["CustomerLastName"]} | " +
                                $"Flight: {reader["FlightNumber"]} ({reader["DepCode"]}->{reader["ArrCode"]}) | " +
                                $"Seat: {reader["SeatNumber"]} ({reader["SeatClass"]}) | " +
                                $"Brand: {reader["BrandName"]} | " +
                                $"Price: {reader["PricePaid"]}");
                        }

                        if (!any)
                        {
                            Console.WriteLine("No tickets found.");
                        }

                        Console.WriteLine("-----------------------\n");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading detailed tickets report:");
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Shows total revenue and number of tickets per flight.
        /// </summary>
        public static void ShowFlightRevenue()
        {
            Console.WriteLine("\n--- Report: Revenue Per Flight ---");

            const string query = @"
                SELECT 
                    f.FlightID,
                    f.FlightNumber,
                    COUNT(DISTINCT t.TicketID) AS TicketCount,
                    IFNULL(SUM(t.PricePaid), 0) AS TotalRevenue
                FROM Flight f
                LEFT JOIN Booking b ON b.FlightID = f.FlightID
                LEFT JOIN BookingPassenger bp ON bp.BookingID = b.BookingID
                LEFT JOIN Ticket t ON t.BookingPassengerID = bp.BookingPassengerID
                GROUP BY f.FlightID, f.FlightNumber
                ORDER BY TotalRevenue DESC;";

            try
            {
                using (var conn = Database.GetConnection())
                using (var cmd = new MySqlCommand(query, conn))
                {
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("FlightID | FlightNumber | Tickets | TotalRevenue");
                        Console.WriteLine("------------------------------------------------");

                        while (reader.Read())
                        {
                            Console.WriteLine(
                                $"{reader["FlightID"],-8} | " +
                                $"{reader["FlightNumber"],-12} | " +
                                $"{reader["TicketCount"],-7} | " +
                                $"{reader["TotalRevenue"]}");
                        }

                        Console.WriteLine("------------------------------------------------\n");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading revenue per flight:");
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Shows total revenue per airline brand.
        /// </summary>
        public static void ShowBrandRevenue()
        {
            Console.WriteLine("\n--- Report: Revenue Per Airline Brand ---");

            const string query = @"
                SELECT 
                    ab.BrandID,
                    ab.Name AS BrandName,
                    COUNT(DISTINCT t.TicketID) AS TicketCount,
                    IFNULL(SUM(t.PricePaid), 0) AS TotalRevenue
                FROM AirlineBrand ab
                LEFT JOIN Flight f ON f.BrandID = ab.BrandID
                LEFT JOIN Booking b ON b.FlightID = f.FlightID
                LEFT JOIN BookingPassenger bp ON bp.BookingID = b.BookingID
                LEFT JOIN Ticket t ON t.BookingPassengerID = bp.BookingPassengerID
                GROUP BY ab.BrandID, ab.Name
                ORDER BY TotalRevenue DESC;";

            try
            {
                using (var conn = Database.GetConnection())
                using (var cmd = new MySqlCommand(query, conn))
                {
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("BrandID | BrandName        | Tickets | TotalRevenue");
                        Console.WriteLine("---------------------------------------------------");

                        while (reader.Read())
                        {
                            Console.WriteLine(
                                $"{reader["BrandID"],-7} | " +
                                $"{reader["BrandName"],-16} | " +
                                $"{reader["TicketCount"],-7} | " +
                                $"{reader["TotalRevenue"]}");
                        }

                        Console.WriteLine("---------------------------------------------------\n");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading revenue per brand:");
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Shows seat map (available / taken) for a given flight.
        /// </summary>
        public static void ShowSeatMapForFlight()
        {
            Console.WriteLine("\n--- Report: Seat Map For Flight ---");

            Console.Write("Enter FlightID: ");
            if (!int.TryParse(Console.ReadLine(), out int flightId))
            {
                Console.WriteLine("Invalid FlightID.");
                return;
            }

            // 1) Get AircraftID for this flight
            int aircraftId;

            const string flightQuery = @"
                SELECT AircraftID
                FROM Flight
                WHERE FlightID = @flightId;";

            try
            {
                using (var conn = Database.GetConnection())
                using (var cmd = new MySqlCommand(flightQuery, conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@flightId", flightId);

                    object result = cmd.ExecuteScalar();
                    if (result == null)
                    {
                        Console.WriteLine("Flight not found.");
                        return;
                    }

                    aircraftId = Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading aircraft for flight:");
                Console.WriteLine(ex.Message);
                return;
            }

            // 2) Show all seats for this aircraft with taken/free status on this flight
            const string seatMapQuery = @"
                SELECT 
                    s.SeatID,
                    s.SeatNumber,
                    s.SeatClass,
                    CASE 
                        WHEN EXISTS (
                            SELECT 1 
                            FROM Ticket t
                            JOIN BookingPassenger bp ON t.BookingPassengerID = bp.BookingPassengerID
                            JOIN Booking b ON bp.BookingID = b.BookingID
                            WHERE t.SeatID = s.SeatID
                              AND b.FlightID = @flightId
                        ) THEN 1
                        ELSE 0
                    END AS IsTaken
                FROM Seat s
                WHERE s.AircraftID = @aircraftId
                ORDER BY s.SeatNumber;";

            try
            {
                using (var conn = Database.GetConnection())
                using (var cmd = new MySqlCommand(seatMapQuery, conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@aircraftId", aircraftId);
                    cmd.Parameters.AddWithValue("@flightId", flightId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine($"\nSeat map for FlightID {flightId}:");
                        Console.WriteLine("SeatID | Seat | Class    | Status");
                        Console.WriteLine("------------------------------------");

                        while (reader.Read())
                        {
                            string status = (Convert.ToInt32(reader["IsTaken"]) == 1)
                                ? "TAKEN"
                                : "FREE";

                            Console.WriteLine(
                                $"{reader["SeatID"],-6} | " +
                                $"{reader["SeatNumber"],-4} | " +
                                $"{reader["SeatClass"],-8} | " +
                                $"{status}");
                        }

                        Console.WriteLine("------------------------------------\n");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading seat map:");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
