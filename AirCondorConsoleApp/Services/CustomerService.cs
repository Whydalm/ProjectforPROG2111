using System;
using MySql.Data.MySqlClient;

namespace AirCondorConsoleApp.Services
{
    /// <summary>
    /// Provides CRUD operations for the Customer entity in the airline database.
    /// </summary>
    public static class CustomerService
    {
        /// <summary>
        /// Inserts a new customer into the database.
        /// </summary>
        public static void AddCustomer()
        {
            Console.WriteLine("Enter first name:");
            string firstName = Console.ReadLine();

            Console.WriteLine("Enter last name:");
            string lastName = Console.ReadLine();

            Console.WriteLine("Enter email:");
            string email = Console.ReadLine();

            Console.WriteLine("Enter phone:");
            string phone = Console.ReadLine();

            const string query = @"
                INSERT INTO Customer (FirstName, LastName, Email, Phone)
                VALUES (@fn, @ln, @em, @ph);";

            try
            {
                using (var conn = Database.GetConnection())
                using (var cmd = new MySqlCommand(query, conn))
                {
                    conn.Open();

                    cmd.Parameters.AddWithValue("@fn", firstName);
                    cmd.Parameters.AddWithValue("@ln", lastName);
                    cmd.Parameters.AddWithValue("@em", email);
                    cmd.Parameters.AddWithValue("@ph", phone);

                    int rows = cmd.ExecuteNonQuery();
                    Console.WriteLine(rows > 0
                        ? "Customer added successfully!"
                        : "Customer insertion failed.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error adding customer:");
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Displays all customers stored in the database.
        /// </summary>
        public static void ViewAllCustomers()
        {
            const string query = "SELECT CustomerID, FirstName, LastName, Email, Phone FROM Customer;";

            try
            {
                using (var conn = Database.GetConnection())
                using (var cmd = new MySqlCommand(query, conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("\n--- Customer List ---");

                        while (reader.Read())
                        {
                            Console.WriteLine(
                                $"ID: {reader["CustomerID"]} | " +
                                $"{reader["FirstName"]} {reader["LastName"]} | " +
                                $"Email: {reader["Email"]} | Phone: {reader["Phone"]}");
                        }

                        Console.WriteLine("-----------------------\n");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading customers:");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
