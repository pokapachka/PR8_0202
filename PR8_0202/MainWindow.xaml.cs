using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using PR8_0202.Classes;

namespace PR8_0202
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private const string ApiKey = "83cb011eeb954d7b7dbbe847f49e51cb";
        private const string ApiUrl = "https://api.openweathermap.org/data/2.5/forecast?q={0}&appid={1}&units=metric&lang=ru";
        public MainWindow()
        {
            InitializeComponent();
            WeatherCashe.InitializeDatabase();
            WeatherCashe.LoadRequestTimes();

        }

        private void SearchCity(object sender, MouseButtonEventArgs e)
        {
            Weather.Visibility = Visibility.Hidden;
            Search.Visibility = Visibility.Hidden;
            CityTextbox.Visibility = Visibility.Visible;
        }

        private async void KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string TextBox = CityTextbox.Text.Trim();
                Weather.Visibility = Visibility.Visible;
                if (string.IsNullOrWhiteSpace(TextBox))
                {
                    Weather.Content = "Погода";
                    MessageBox.Show("Введите название города");
                }
                else
                {
                    Weather.Content = CityTextbox.Text;
                }
                Search.Visibility = Visibility.Visible;
                CityTextbox.Visibility = Visibility.Hidden;
                await UpdateWeather(TextBox);
                UpdateRequestCount();
            }
        }
        private async void LoadedWin(object sender, RoutedEventArgs e)
        {
            string DC = "Пермь";
            await UpdateWeather(DC);
            UpdateRequestCount();
        }
        private async Task<List<Weather>> FetchWeatherData(string city)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = string.Format(ApiUrl, city, ApiKey);
                HttpResponseMessage response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Ошибка {response.StatusCode}: {response.ReasonPhrase}");
                }
                string responseBody = await response.Content.ReadAsStringAsync();
                var json = JsonConvert.DeserializeObject<dynamic>(responseBody);

                var weatherList = new List<Weather>();

                foreach (var item in json.list)
                {
                    weatherList.Add(new Weather
                    {
                        DateTime = Convert.ToDateTime(item.dt_txt).ToString("dd.MM.yyyy HH:mm"),
                        Temperature = $"{item.main.temp} °C",
                        Pressure = $"{item.main.pressure} мм рт.ст.",
                        Humidity = $"{item.main.humidity} %",
                        WindSpeed = $"{item.wind.speed} м/с",
                        FeelsLike = $"{item.main.feels_like} °C",
                        WeatherDescription = item.weather[0].description.ToString(),
                    });
                }
                return weatherList;
            }
        }
        private void UpdateRequestCount()
        {
            int requestCount = WeatherCashe.GetRequestCountTwoHours();
            RequestCountLabel.Content = $"Количество запросов сегодня: {requestCount}/5";
        }

        private async Task UpdateWeather(string city)
        {
            try
            {
                int requestCount = WeatherCashe.GetRequestCountTwoHours();
                if (requestCount >= 5)
                {
                    var cachedData = WeatherCashe.GetWeather(city);
                    if (cachedData.Count > 0)
                    {
                        WeatherDataGrid.ItemsSource = cachedData;
                        MessageBox.Show("Данные для города получены из кэша");
                    }
                    else
                    {
                        MessageBox.Show("Лимит запросов на 2 часа превышен");
                    }
                }
                else
                {
                    var weatherData = await FetchWeatherData(city);
                    WeatherDataGrid.ItemsSource = weatherData;
                    foreach (var data in weatherData)
                    {
                        WeatherCashe.SaveWeatherData(city, data.DateTime, data.Temperature, data.Pressure, data.Humidity, 
                            data.WindSpeed, data.FeelsLike, data.WeatherDescription);
                    }
                    WeatherCashe.IncrementRecuestCount();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }
    }
}
