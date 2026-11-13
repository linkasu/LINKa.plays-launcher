using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace LINKa.plays_launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += async (_, _) =>
            {
                try
                {
                    await CheckUpdate();
                }
                catch (Exception)
                {
                    // Games remain available when the update service is unreachable.
                }
            };
        }

        private async Task CheckUpdate()
        {
            // URL of the JSON file
            string url = "https://linka.su/linkaplay/version.json";

            // Get the version from the app
            string appVersion = GetAppVersion();

            // Fetch the JSON data from the URL
            using (HttpClient client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) })
            {
                string jsonData = await client.GetStringAsync(url);
                using JsonDocument doc = JsonDocument.Parse(jsonData);
                string version = GetStringProperty(doc.RootElement, "version");

                // Compare the version with the app version
                if (Version.Parse(version) > Version.Parse(appVersion))
                {
                    UpdateButton.IsEnabled = true;
                    UpdateButton.Content = "Есть новое обновление!";
                }
            }
        }

        private string GetAppVersion()
        {

            // Read the version from the local version.json file in the app directory
            string versionFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "version.json");
            if (File.Exists(versionFile))
            {
                try
                {
                    string jsonData = File.ReadAllText(versionFile);
                    JsonDocument doc = JsonDocument.Parse(jsonData);
                    return GetStringProperty(doc.RootElement, "version");
                }
                catch (JsonException)
                {
                    Console.WriteLine("Error parsing version.json file.");
                }
            }
            else
            {
                Console.WriteLine("version.json file not found.");
            }

            // Return the assembly version if version.json file is not found or there is an error parsing it
            return "0.0.0";
        }

        private string GetStringProperty(JsonElement element, string propertyName)
        {
            if (element.TryGetProperty(propertyName, out JsonElement property))
            {
                if (property.ValueKind == JsonValueKind.String)
                {
                    return property.GetString();
                }
                else
                {
                    Console.WriteLine($"Error: Property '{propertyName}' is not a string.");
                }
            }
            else
            {
                Console.WriteLine($"Error: Property '{propertyName}' not found.");
            }

            return null;
        }

        private void OpenGames_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
                using Process process = Process.Start(new ProcessStartInfo
                {
                    FileName = Path.Combine(appDirectory, "LINKa.plays.exe"),
                    WorkingDirectory = appDirectory
                }) ?? throw new InvalidOperationException("Не удалось создать процесс игры.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this,
                    $"Не удалось запустить игры. Убедитесь, что LINKa.plays.exe находится в папке запуска приложения.\n\n{ex.Message}",
                    "Ошибка запуска", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Application.Current.Shutdown();
        }

        private void OpenMethodic_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://linka.su/plays-methodic.pdf",
                UseShellExecute = true
            });
        }
        private void OpenDownload_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://linka.su/linkaplay/linkaplaysetup.exe",
                UseShellExecute = true
            });
            Application.Current.Shutdown();

        }
    }
}
