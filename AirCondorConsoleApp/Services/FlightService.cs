using System;
using MySql.Data.MySqlClient;

namespace AirCondorConsoleApp.Services
{
    /// <summary>
    /// Provides read operations for flights in the airline database.
    /// </summary>
    public static class FlightService
    {
        /// <summary>
        /// Displays all flights with airline brand and airport information.
        /// </summary>
        public static void ViewAllFlights()
        {
            const string query = @"
                SELECT 
                    f.FlightID,
                    f.FlightNumber,
                    b.Name AS BrandName,
                    f.DepartureAirportCode,
                    dep.Name AS DepartureAirportName,
                    f.ArrivalAirportCode,
                    arr.Name AS ArrivalAirportName,
                    f.DepartureDateTime,
                    f.ArrivalDateTime,
                    f.BasePrice
                FROM Flight f
                JOIN AirlineBrand b ON f.BrandID = b.BrandID
                JOIN Airport dep ON f.DepartureAirportCode = dep.AirportCode
                JOIN Airport arr ON f.ArrivalAirportCode = arr.AirportCode
                ORDER BY f.DepartureDateTime;";

            try
            {
                using (var conn = Database.GetConnection())
                using (var cmd = new MySqlCommand(query, conn))
                {
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("\n--- Flight List ---");

                        while (reader.Read())
                        {
                            Console.WriteLine(
                                $"ID: {reader["FlightID"]} | " +
                                $"No: {reader["FlightNumber"]} | " +
                                $"Brand: {reader["BrandName"]} | " +
                                $"{reader["DepartureAirportCode"]} ({reader["DepartureAirportName"]}) " +
                                $"-> {reader["ArrivalAirportCode"]} ({reader["ArrivalAirportName"]}) | " +
                                $"Dep: {reader["DepartureDateTime"]} | " +
                                $"Arr: {reader["ArrivalDateTime"]} | " +
                                $"Base: {reader["BasePrice"]} CAD");
                        }

                        Console.WriteLine("-------------------\n");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading flights:");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
