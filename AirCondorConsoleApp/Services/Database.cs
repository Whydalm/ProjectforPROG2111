using MySql.Data.MySqlClient;

namespace AirCondorConsoleApp
{
    /// <summary>
    /// Provides a single point of access to the MySQL database connection.
    /// </summary>
    public static class Database
    {
        private const string ConnectionString =
            "server=localhost;port=3306;database=airline;user=root;password=111qqq123e;";

        /// <summary>
        /// Creates and returns a new MySqlConnection object.
        /// The caller must open/close the connection.
        /// </summary>
        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }
    }
}
