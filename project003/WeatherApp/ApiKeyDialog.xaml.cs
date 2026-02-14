using System.Windows;

namespace WeatherApp
{
    public partial class ApiKeyDialog : Window
    {
        public string ApiKey { get; private set; } = "";

        public ApiKeyDialog(string currentApiKey)
        {
            InitializeComponent();
            ApiKeyTextBox.Text = currentApiKey;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var apiKey = ApiKeyTextBox.Text.Trim();
            
            if (string.IsNullOrEmpty(apiKey))
            {
                ErrorText.Text = "API 키를 입력하세요.";
                return;
            }

            ApiKey = apiKey;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
