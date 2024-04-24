using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;

namespace BeShare.App
{
    /// <summary>
    /// Interaction logic for devpanel.xaml
    /// </summary>
    public partial class devpanel : Window
    {
        private List<string> loadingStages = new List<string> { "Завантаження...", "Перевірка даних...", "Оновлення...", "Готово!" };
        private Random random = new Random();
        public devpanel()
        {
            InitializeComponent();
            Bruh();
            SetGifImageSource();
        }
        private void SetGifImageSource()
        {
            // Путь к вашему гиф-файлу внутри проекта
            string imagePath = "/BeShare.App;component/Content/spin.gif";

            // Создаем BitmapImage из файла
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(imagePath, UriKind.Relative);
            bitmap.EndInit();

            // Устанавливаем BitmapImage в качестве источника изображения для элемента Image
            gifImage.Source = bitmap;

            // Запускаем анимацию
            ImageBehavior.SetAnimatedSource(gifImage, bitmap);
        }

        private async void Bruh()
        {
            
            await Task.Delay(15000);
        }
        private void UpdateLoadingText()
        {
            string stage = loadingStages[random.Next(loadingStages.Count)];
            loadingText.Text = stage;
            loadingText.Visibility = Visibility.Visible;
        }
    }
}
