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

        private string[] texts = { "Перевірка сервера", "Проводим підключення", "Успішно!" };
        private int currentIndex = 0;
        private DispatcherTimer timer;

        public StartWindow()
        {
            InitializeComponent();
            LoadGif();
            CenterWindows();
            wpf.RegisterProgram("beshare.app", applicationPath);
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.ResizeMode = ResizeMode.NoResize;
            if (AppArgs.Args != null && AppArgs.Args.Length > 0)
            {
                string url = AppArgs.Args[0];
                ProcessUrl(url);
            }
            
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += Timer_Tick;
            timer.Start();

        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            // Зміна тексту наступним за списком
            textBlock.Text = texts[currentIndex];
            currentIndex = (currentIndex + 1) % texts.Length;
        }
        public void CloseWindow()
        {
            Application.Current.MainWindow.Close();
        }
        private void CenterWindows()
        {
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;
            double windowWidth = this.Width;
            double windowHeight = this.Height;
            this.Left = (screenWidth - windowWidth) / 2;
            this.Top = (screenHeight - windowHeight) / 2;
        }
        private void LoadGif() 
        {
            string imagePath = "/BeShare.App;component/Content/spin.gif";

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(imagePath, UriKind.Relative);
            bitmap.EndInit();

            gifImage.Source = bitmap;
            ImageBehavior.SetAnimatedSource(gifImage, bitmap);
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
            auth.AuthMake();
        }
    }
}
