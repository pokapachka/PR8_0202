
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
        private const string DbPath = "Cache.db";
        
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

                string createRequestTableQuery = @"
        CREATE TABLE IF NOT EXISTS RequestLog (
            RequestTime TEXT NOT NULL
        )";
                var command2 = new SQLiteCommand(createRequestTableQuery, connection);
                command2.ExecuteNonQuery();
            }
        }
        public static void LoadRequestTimes()
        {
            requestTime.Clear();

            using (var connection = new SQLiteConnection($"Data Source={DbPath};Version=3;"))
            {
                connection.Open();
                string selectQuery = "SELECT RequestTime FROM RequestLog";

                var command = new SQLiteCommand(selectQuery, connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DateTime timestamp = DateTime.Parse(reader["RequestTime"].ToString());
                        requestTime.Add(timestamp);
                    }
                }
            }
            DateTime twoHoursAgo = DateTime.Now.AddHours(-2);
            requestTime.RemoveAll(timestamp => timestamp < twoHoursAgo);
        }
        private static List<DateTime> requestTime = new List<DateTime>();
        public static int GetRequestCountTwoHours()
        {
            DateTime twoHoursAgo = DateTime.Now.AddHours(-2);

            using (var connection = new SQLiteConnection($"Data Source={DbPath};Version=3;"))
            {
                connection.Open();
                string deleteQuery = "DELETE FROM RequestLog WHERE RequestTime < @TwoHoursAgo";
                var deleteCommand = new SQLiteCommand(deleteQuery, connection);
                deleteCommand.Parameters.AddWithValue("@TwoHoursAgo", twoHoursAgo.ToString("yyyy-MM-dd HH:mm:ss"));
                deleteCommand.ExecuteNonQuery();
            }

            requestTime.RemoveAll(timestamp => timestamp < twoHoursAgo);
            return requestTime.Count;
        }
        public static void IncrementRecuestCount()
        {
            DateTime now = DateTime.Now;
            requestTime.Add(now);

            using (var connection = new SQLiteConnection($"Data Source={DbPath};Version=3;"))
            {
                connection.Open();
                string insertQuery = @"
        INSERT INTO RequestLog (RequestTime)
        VALUES (@RequestTime)";

                var command = new SQLiteCommand(insertQuery, connection);
                command.Parameters.AddWithValue("@RequestTime", now.ToString("yyyy-MM-dd HH:mm:ss"));
                command.ExecuteNonQuery();
            }
        }
        public static void SaveWeatherData(string city, string dateTime, string temperature, string pressure, 
            string humidity, string windSpeed, string feelsLike, string weatherDescription)
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
        public static List<Weather> GetWeather(string city)
        {
            List<Weather> weatherDataList = new List<Weather>();

            using (var connection = new SQLiteConnection($"Data Source={DbPath};Version=3;"))
            {
                connection.Open();
                string selectQuery = @"
                SELECT * FROM WeatherData
                WHERE City = @City AND RequestDate = @RequestDate";

                var command = new SQLiteCommand(selectQuery, connection);
                command.Parameters.AddWithValue("@City", city);
                command.Parameters.AddWithValue("@RequestDate", DateTime.Now.ToString("yyyy-MM-dd"));

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        weatherDataList.Add(new Weather
                        {
                            DateTime = reader["DateTime"].ToString(),
                            Temperature = reader["Temperature"].ToString(),
                            Pressure = reader["Pressure"].ToString(),
                            Humidity = reader["Humidity"].ToString(),
                            WindSpeed = reader["WindSpeed"].ToString(),
                            FeelsLike = reader["FeelsLike"].ToString(),
                            WeatherDescription = reader["WeatherDescription"].ToString()
                        });
                    }
                }
            }
            return weatherDataList;
        }
    }
}
