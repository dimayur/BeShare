using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web;
using System.Windows;
using Microsoft.Win32;

namespace BeShare.App
{
    public static class AppArgs
    {
        public static string[] Args { get; set; }
    }

    public partial class StartWindow : Window
    {
        private readonly string AuthorizationUrl = "http://localhost:3000/login";
        string applicationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BeShare.App.exe");

        public StartWindow()
        {
            InitializeComponent();

            RegisterUrlSchemeHandler("beshare.app", applicationPath);
            if (AppArgs.Args != null && AppArgs.Args.Length > 0)
            {
                string url = AppArgs.Args[0];
                ProcessUrl(url);
            }
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
                MessageBox.Show($"Помилка: {ex.Message}", "Помилка");
            }
        }

        private void ProcessUrl(string url)
        {
            try
            {
                Uri uri = new Uri(url);
                string token = HttpUtility.ParseQueryString(uri.Query).Get("token");
                Callback(token);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}", "Помилка");
            }
        }

        private void Callback(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    SaveToken(token);
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка: {ex.Message}", "Помилка");
                }
            }
            else
            {
                MessageBox.Show("[#]Невірний токен", "Помилка авторизації");
            }
        }

        private void SaveToken(string token)
        {
            try
            {
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "beshare.app", "token.txt");
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                File.WriteAllText(filePath, token, Encoding.UTF8);
                MessageBox.Show("[#]Токен збережено!");
            }
            catch (Exception ex)
            {
                throw new Exception($"Помилка збереження токена: {ex.Message}");
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
                throw new Exception($"Помилка: {ex.Message}");
            }
        }
    }
}
