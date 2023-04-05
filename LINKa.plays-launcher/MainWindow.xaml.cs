using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text.Json;
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
            try
            {
                CheckUpdate();
            }
            catch (Exception)
            {

            } 
        }

        private void CheckUpdate()
        {
            // URL of the JSON file
            string url = "https://linka.su/linkaplay/version.json";

            // Get the version from the app
            string appVersion = GetAppVersion();

            // Fetch the JSON data from the URL
            using (WebClient client = new WebClient())
            {
                string jsonData = client.DownloadString(url);
                JsonDocument doc = JsonDocument.Parse(jsonData);
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
            Process.Start(Path.Combine(Directory.GetCurrentDirectory(), "LINKa.plays.exe"));
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