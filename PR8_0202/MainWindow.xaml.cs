using System;
using System.Collections.Generic;
using System.Linq;
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

namespace PR8_0202
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SearchCity(object sender, MouseButtonEventArgs e)
        {
            Weather.Visibility = Visibility.Hidden;
            Search.Visibility = Visibility.Hidden;
            CityTextbox.Visibility = Visibility.Visible;
        }

        private void KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var TextBox = CityTextbox.Text;
                Weather.Visibility = Visibility.Visible;
                if (string.IsNullOrWhiteSpace(TextBox))
                {
                    Weather.Content = "Погода";
                }
                else
                {
                    Weather.Content = CityTextbox.Text;
                }
                Search.Visibility = Visibility.Visible;
                CityTextbox.Visibility = Visibility.Hidden;
            }
        }
        private void LoadedWin(object sender, RoutedEventArgs e)
        {
            string DC = "Пермь";
        }
        private async Task UpdateWeather(string city)
        {
            try
            {
                int requestCount = WeatherCache.GetRequestCountForToday();

                if (requestCount >= 500)
                {
                    var cachedData = WeatherCache.GetWeatherData(city);
                    if (cachedData.Count > 0)
                    {
                        WeatherDataGrid.ItemsSource = cachedData;
                        MessageBox.Show("Данные для города получены из кэша");
                    }
                    else
                    {
                        MessageBox.Show("Лимит запросов на сегодня превышен");
                    }
                }
                else
                {
                    var weatherData = await FetchWeatherData(city);
                    WeatherDataGrid.ItemsSource = weatherData;
                    foreach (var data in weatherData)
                    {
                        WeatherCache.SaveWeatherData(city, data.DateTime, data.Temperature, data.Pressure, data.Humidity, data.WindSpeed, data.FeelsLike, data.WeatherDescription);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }
    }
}
