
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
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
                SQLiteConnection.CreateFile(DbPath);
            }

            using (var connection = new SQLiteConnection($"Data Source={DbPath};Version=3;"))
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

                var command = new SQLiteCommand(createTableQuery, connection);
                command.ExecuteNonQuery();
            }
        }
        public static void SaveWeatherData(string city, string dateTime, string temperature, string pressure, string humidity, string windSpeed, string feelsLike, string weatherDescription)
        {
            using (var connection = new SQLiteConnection($"Data Source={DbPath};Version=3;"))
            {
                connection.Open();
                string insertQuery = @"
                INSERT INTO WeatherData (City, DateTime, Temperature, Pressure, Humidity, WindSpeed, FeelsLike, WeatherDescription, RequestDate)
                VALUES (@City, @DateTime, @Temperature, @Pressure, @Humidity, @WindSpeed, @FeelsLike, @WeatherDescription, @RequestDate)";

                var command = new SQLiteCommand(insertQuery, connection);
                command.Parameters.AddWithValue("@City", city);
                command.Parameters.AddWithValue("@DateTime", dateTime);
                command.Parameters.AddWithValue("@Temperature", temperature);
                command.Parameters.AddWithValue("@Pressure", pressure);
                command.Parameters.AddWithValue("@Humidity", humidity);
                command.Parameters.AddWithValue("@WindSpeed", windSpeed);
                command.Parameters.AddWithValue("@FeelsLike", feelsLike);
                command.Parameters.AddWithValue("@WeatherDescription", weatherDescription);
                command.Parameters.AddWithValue("@RequestDate", DateTime.Now.ToString("yyyy-MM-dd"));

                command.ExecuteNonQuery();
            }
        }
    }
}
