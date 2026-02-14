using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WeatherApp
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private string _apiKey = "";
        private const string BaseUrl = "https://api.openweathermap.org/data/2.5";

        public MainWindow()
        {
            InitializeComponent();
            LoadApiKey();
        }

        private void LoadApiKey()
        {
            try
            {
                var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
                if (File.Exists(configPath))
                {
                    var json = File.ReadAllText(configPath);
                    using var doc = JsonDocument.Parse(json);
                    _apiKey = doc.RootElement
                        .GetProperty("OpenWeatherMap")
                        .GetProperty("ApiKey")
                        .GetString() ?? "";
                }
            }
            catch { }
        }

        private void SaveApiKey(string apiKey)
        {
            try
            {
                var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
                var config = new { OpenWeatherMap = new { ApiKey = apiKey, BaseUrl } };
                var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(configPath, json);
                _apiKey = apiKey;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"API 키 저장 실패: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                MessageBox.Show("API 키가 설정되지 않았습니다. 설정 버튼을 클릭하여 API 키를 입력하세요.", 
                    "알림", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var city = CityTextBox.Text.Trim();
            if (string.IsNullOrEmpty(city))
            {
                ErrorText.Text = "도시명을 입력하세요.";
                return;
            }

            await GetWeatherAsync(city);
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ApiKeyDialog(_apiKey);
            if (dialog.ShowDialog() == true)
            {
                SaveApiKey(dialog.ApiKey);
            }
        }

        private async Task GetWeatherAsync(string city)
        {
            LoadingText.Text = "날씨 정보를 가져오는 중...";
            ErrorText.Text = "";
            WeatherCards.Children.Clear();

            try
            {
                // 5일/3시간 예보 API 호출
                var url = $"{BaseUrl}/forecast?q={Uri.EscapeDataString(city)}&appid={_apiKey}&units=metric&lang=kr";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ErrorText.Text = $"API 오류: {response.StatusCode}\n도시명을 확인하세요.";
                    LoadingText.Text = "";
                    return;
                }

                var json = await response.Content.ReadAsStringAsync();
                var weatherData = ParseWeatherData(json);

                DisplayWeather(weatherData);
                LoadingText.Text = "";
            }
            catch (HttpRequestException)
            {
                ErrorText.Text = "네트워크 연결을 확인하세요.";
                LoadingText.Text = "";
            }
            catch (Exception ex)
            {
                ErrorText.Text = $"오류 발생: {ex.Message}";
                LoadingText.Text = "";
            }
        }

        private List<DailyWeather> ParseWeatherData(string json)
        {
            var result = new List<DailyWeather>();
            using var doc = JsonDocument.Parse(json);
            var list = doc.RootElement.GetProperty("list");

            // 같은 날짜의 데이터를 그룹화하여 평균값 계산
            var dailyGroups = new Dictionary<string, List<WeatherItem>>();

            foreach (var item in list.EnumerateArray())
            {
                var dtTxt = item.GetProperty("dt_txt").GetString() ?? "";
                var date = dtTxt.Split(' ')[0]; // YYYY-MM-DD

                if (!dailyGroups.ContainsKey(date))
                    dailyGroups[date] = new List<WeatherItem>();

                var main = item.GetProperty("main");
                var weather = item.GetProperty("weather").EnumerateArray().First();
                var wind = item.GetProperty("wind");

                dailyGroups[date].Add(new WeatherItem
                {
                    Date = DateTime.Parse(dtTxt),
                    Temp = main.GetProperty("temp").GetDouble(),
                    FeelsLike = main.GetProperty("feels_like").GetDouble(),
                    Humidity = main.GetProperty("humidity").GetInt32(),
                    Description = weather.GetProperty("description").GetString() ?? "",
                    Icon = weather.GetProperty("icon").GetString() ?? "",
                    WindSpeed = wind.GetProperty("speed").GetDouble()
                });
            }

            // 각 날짜별로 대표값 선택 (정오 데이터 우선, 없으면 첫 번째)
            foreach (var group in dailyGroups.Take(5))
            {
                var items = group.Value;
                var noonItem = items.FirstOrDefault(i => i.Date.Hour == 12) ?? items.First();

                result.Add(new DailyWeather
                {
                    Date = DateTime.Parse(group.Key),
                    Temp = noonItem.Temp,
                    FeelsLike = noonItem.FeelsLike,
                    Humidity = noonItem.Humidity,
                    Description = noonItem.Description,
                    Icon = noonItem.Icon,
                    WindSpeed = noonItem.WindSpeed,
                    MinTemp = items.Min(i => i.Temp),
                    MaxTemp = items.Max(i => i.Temp)
                });
            }

            return result;
        }

        private void DisplayWeather(List<DailyWeather> weatherList)
        {
            WeatherCards.Children.Clear();

            foreach (var weather in weatherList)
            {
                var card = CreateWeatherCard(weather);
                WeatherCards.Children.Add(card);
            }
        }

        private Border CreateWeatherCard(DailyWeather weather)
        {
            var dayOfWeek = weather.Date.ToString("ddd", new System.Globalization.CultureInfo("ko-KR"));
            var dateStr = weather.Date.ToString("MM/dd");
            var iconUrl = $"https://openweathermap.org/img/wn/{weather.Icon}@2x.png";

            var card = new Border
            {
                Width = 150,
                Height = 200,
                Margin = new Thickness(10),
                Background = new SolidColorBrush(Color.FromRgb(45, 45, 45)),
                CornerRadius = new CornerRadius(10),
                BorderBrush = new SolidColorBrush(Color.FromRgb(85, 85, 85)),
                BorderThickness = new Thickness(1)
            };

            var stack = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center };

            // 날짜
            stack.Children.Add(new TextBlock
            {
                Text = dayOfWeek,
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10, 0, 0)
            });

            stack.Children.Add(new TextBlock
            {
                Text = dateStr,
                FontSize = 12,
                Foreground = new SolidColorBrush(Color.FromRgb(170, 170, 170)),
                HorizontalAlignment = HorizontalAlignment.Center
            });

            // 아이콘
            var iconImage = new Image
            {
                Width = 60,
                Height = 60,
                Source = new BitmapImage(new Uri(iconUrl)),
                Margin = new Thickness(0, 5, 0, 0)
            };
            stack.Children.Add(iconImage);

            // 온도
            stack.Children.Add(new TextBlock
            {
                Text = $"{weather.Temp:F1}°C",
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center
            });

            // 최저/최고
            stack.Children.Add(new TextBlock
            {
                Text = $"최저 {weather.MinTemp:F0}° / 최고 {weather.MaxTemp:F0}°",
                FontSize = 11,
                Foreground = new SolidColorBrush(Color.FromRgb(170, 170, 170)),
                HorizontalAlignment = HorizontalAlignment.Center
            });

            // 설명
            stack.Children.Add(new TextBlock
            {
                Text = weather.Description,
                FontSize = 12,
                Foreground = new SolidColorBrush(Color.FromRgb(200, 200, 200)),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 5, 0, 0),
                TextTrimming = TextTrimming.CharacterEllipsis
            });

            // 습도
            stack.Children.Add(new TextBlock
            {
                Text = $"습도 {weather.Humidity}%",
                FontSize = 11,
                Foreground = new SolidColorBrush(Color.FromRgb(170, 170, 170)),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 5, 0, 10)
            });

            card.Child = stack;
            return card;
        }
    }

    public class WeatherItem
    {
        public DateTime Date { get; set; }
        public double Temp { get; set; }
        public double FeelsLike { get; set; }
        public int Humidity { get; set; }
        public string Description { get; set; } = "";
        public string Icon { get; set; } = "";
        public double WindSpeed { get; set; }
    }

    public class DailyWeather : WeatherItem
    {
        public double MinTemp { get; set; }
        public double MaxTemp { get; set; }
    }
}
