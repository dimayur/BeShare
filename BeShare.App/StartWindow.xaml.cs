using System.Diagnostics;
using System.IO;
using System.Web;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using BeShare.App.Method;
using WpfAnimatedGif;


namespace BeShare.App
{
    public static class AppArgs
    {
        public static string[] Args { get; set; }
    }

    public partial class StartWindow : Window
    {
        WpfHelper wpf = new WpfHelper();
        Auth auth = new Auth();
        private readonly string AuthorizationUrl = "http://localhost:3000/login";
        string applicationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BeShare.App.exe");
        public StartWindow()
        {
            InitializeComponent();
            //wpf.RegisterProgram("beshare.app", applicationPath); 
            if (AppArgs.Args != null && AppArgs.Args.Length > 0)
            {
                string url = AppArgs.Args[0];
                ProcessUrl(url);
            }
        }

        //AuthStart();

        //auth.AuthMake();

        public void CloseWindow()
        {
            Application.Current.MainWindow.Close();
        }

        public void AuthStart()
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
                    auth.SaveToken(token);
                    Panel panel = new Panel();
                    panel.Show();
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
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //AuthStart();
            //auth.AuthMake();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            auth.AuthMake();
        }
    }
}
