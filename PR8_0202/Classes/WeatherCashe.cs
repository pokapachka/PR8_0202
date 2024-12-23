using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PR8_0202.Classes
{
    public class WeatherCashe
    {
        private const string DbPath = "weatherCache.db";
        
        public static void InitializeDatabase()
        {
            if (!System.IO.File.Exists(DbPath))
            {
                MySqlConnection.connect(DbPath);
            }

            using (var connection = new MySqlConnection($"Data Source={DbPath};Version=3;"))
            {
                connection.Open();
                string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS WeatherData (
                    City TEXT NOT NULL,
                    DateTime TEXT NOT NULL,
                    Temperature TEXT,
                    Pressure TEXT,
                    Humidity TEXT,
                    WindSpeed TEXT,
                    FeelsLike TEXT,
                    WeatherDescription TEXT,
                    RequestDate TEXT NOT NULL
                )";

                var command = new MySqlCommand(createTableQuery, connection);
                command.ExecuteNonQuery();
            }
        }
    }
}
