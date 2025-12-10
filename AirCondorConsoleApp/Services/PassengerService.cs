using System;
using MySql.Data.MySqlClient;

namespace AirCondorConsoleApp.Services
{
    /// <summary>
    /// Provides basic CRUD operations for passengers.
    /// </summary>
    public static class PassengerService
    {
        /// <summary>
        /// Adds a new passenger to the database.
        /// </summary>
        public static void AddPassenger()
        {
            Console.WriteLine("\n--- Add Passenger ---");

            Console.Write("First name: ");
            string firstName = Console.ReadLine();

            Console.Write("Last name: ");
            string lastName = Console.ReadLine();

            Console.Write("Date of birth (YYYY-MM-DD, optional): ");
            string dobInput = Console.ReadLine();

            Console.Write("Passport number (optional): ");
            string passportNumber = Console.ReadLine();

            DateTime? dob = null;
            if (!string.IsNullOrWhiteSpace(dobInput))
            {
                if (DateTime.TryParse(dobInput, out DateTime parsed))
                {
                    dob = parsed;
                }
                else
                {
                    Console.WriteLine("Invalid date format. Date of birth will be stored as NULL.");
                }
            }

            const string query = @"
                INSERT INTO Passenger (FirstName, LastName, DateOfBirth, PassportNumber)
                VALUES (@first, @last, @dob, @passport);";

            try
            {
                using (var conn = Database.GetConnection())
                using (var cmd = new MySqlCommand(query, conn))
                {
                    conn.Open();

                    cmd.Parameters.AddWithValue("@first", firstName);
                    cmd.Parameters.AddWithValue("@last", lastName);

                    if (dob.HasValue)
                        cmd.Parameters.AddWithValue("@dob", dob.Value);
                    else
                        cmd.Parameters.AddWithValue("@dob", DBNull.Value);

                    if (!string.IsNullOrWhiteSpace(passportNumber))
                        cmd.Parameters.AddWithValue("@passport", passportNumber);
                    else
                        cmd.Parameters.AddWithValue("@passport", DBNull.Value);

                    int rows = cmd.ExecuteNonQuery();

                    Console.WriteLine(rows > 0
                        ? "Passenger added successfully!"
                        : "Passenger was not added.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error adding passenger:");
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Displays all passengers stored in the database.
        /// </summary>
        public static void ViewAllPassengers()
        {
            const string query = @"
                SELECT PassengerID, FirstName, LastName, DateOfBirth, PassportNumber
                FROM Passenger
                ORDER BY LastName, FirstName;";

            try
            {
                using (var conn = Database.GetConnection())
                using (var cmd = new MySqlCommand(query, conn))
                {
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("\n--- Passenger List ---");

                        while (reader.Read())
                        {
                            string dobText = reader["DateOfBirth"] == DBNull.Value
                                ? "N/A"
                                : reader["DateOfBirth"].ToString();

                            string passportText = reader["PassportNumber"] == DBNull.Value
                                ? "N/A"
                                : reader["PassportNumber"].ToString();

                            Console.WriteLine(
                                $"ID: {reader["PassengerID"]} | " +
                                $"Name: {reader["FirstName"]} {reader["LastName"]} | " +
                                $"DOB: {dobText} | " +
                                $"Passport: {passportText}");
                        }

                        Console.WriteLine("-----------------------\n");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading passengers:");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
