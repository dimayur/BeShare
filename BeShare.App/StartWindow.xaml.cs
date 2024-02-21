using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Win32;

namespace BeShare.App
{
    public partial class StartWindow : Window
    {
        private readonly string AuthorizationUrl = "http://localhost:3000/login";
        string applicationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BeShare.Apps.exe");

        public StartWindow()
        {
            InitializeComponent();

            RegisterUrlSchemeHandler("beshare.apps", applicationPath);
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = AuthorizationUrl,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка відкриття URL: {ex.Message}", "Помилка");
            }
        }

        private void Frame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            MessageBox.Show($"Navigating to: {e.Uri}");
            Uri uri = e.Uri;
            if (uri != null && uri.Scheme == "beshare.apps" && uri.LocalPath == "/callback")
            {
                try
                {
                    string token = HttpUtility.ParseQueryString(uri.Query).Get("token");
                    HandleAuthorizationCallback(token);
                    e.Cancel = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка обробки авторизації: {ex.Message}", "Помилка");
                }
            }
        }

        private void HandleAuthorizationCallback(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    SaveTokenLocally(token);
                    MessageBox.Show("Авторизація успішна!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка збереження токену: {ex.Message}", "Помилка");
                }
            }
            else
            {
                MessageBox.Show("Отримано невірний токен або callback URL.", "Помилка авторизації");
            }
        }

        private void SaveTokenLocally(string token)
        {
            MessageBox.Show("SaveTokenLocally");
            try
            {
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "YourApp", "token.txt");
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                File.WriteAllText(filePath, token, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new Exception($"Помилка збереження токену: {ex.Message}");
            }
        }

        private void RegisterUrlSchemeHandler(string scheme, string applicationPath)
        {
            try
            {
                RegistryKey key = Registry.ClassesRoot.CreateSubKey(scheme);
                if (key != null)
                {
                    key.SetValue(string.Empty, "URL:" + scheme);
                    key.SetValue("URL Protocol", string.Empty);

                    RegistryKey shellKey = key.CreateSubKey("shell");
                    if (shellKey != null)
                    {
                        RegistryKey openKey = shellKey.CreateSubKey("open");
                        if (openKey != null)
                        {
                            RegistryKey commandKey = openKey.CreateSubKey("command");
                            if (commandKey != null)
                            {
                                commandKey.SetValue(string.Empty, $"\"{applicationPath}\" \"%1\"");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Помилка реєстрації обробника URL-схеми: {ex.Message}");
            }
        }

    }
}
