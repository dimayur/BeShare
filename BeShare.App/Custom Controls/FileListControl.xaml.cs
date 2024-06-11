using BeShare.App.Method;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileManagerUI.Custom_Controls
{
    /// <summary>
    /// Interaction logic for FileListControl.xaml
    /// </summary>
    public partial class FileListControl : UserControl
    {
        FileManager filesys = new FileManager();
        public FileListControl()
        {
            InitializeComponent();
            LoadFiles();
            filesys.FileReadMake();
        }

        private async Task DeleteFileAsync(int fileId)
        {
            string url = $"https://localhost:7147/Home/api/deletefile/{fileId}";
            string token = new Auth().ReadToken();

            if (string.IsNullOrWhiteSpace(token))
            {
                MessageBox.Show("Токен порожній або відсутній.");
                return;
            }

            using (var httpClient = new HttpClient())
            {
                url += "?token=" + token;
                var response = await httpClient.DeleteAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Файл успішно видалено!");
                }
                else
                {
                    MessageBox.Show("Помилка під час видалення файлу: " + response.ReasonPhrase);
                }
            }
        }
        public async Task RefreshFiles()
        {
            Deamn.Children.Clear();
            List<FileData> files = filesys.ReadFile();

            foreach (var file in files)
            {
                Button button = new Button
                {
                    Style = (Style)FindResource("ButtonStyle1"),
                    Content = CreateButtonContent(file),
                    Tag = file
                };

                button.Click += FileButton_Click;
                Deamn.Children.Add(button);
            }
            Deamn.InvalidateVisual();
        }
        public void LoadFiles()
        {
            List<FileData> files = filesys.ReadFile();

            foreach (var file in files)
            {
                Button button = new Button
                {
                    Style = (Style)FindResource("ButtonStyle1"),
                    Content = CreateButtonContent(file),
                    Tag = file
                };

                button.Click += FileButton_Click;
                Deamn.Children.Add(button);
            }
        }

        private void FileButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            FileData fileData = clickedButton.Tag as FileData;

            if (fileData != null)
            {
                string url = $"http://localhost:3000/files/info/{fileData.Id}";
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
        }
        private UIElement CreateButtonContent(FileData file)
        {
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(140) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) });

            StackPanel stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            Image image = new Image
            {
                Margin = new Thickness(5, 0, 0, 0),
                Width = 50,
                Stretch = Stretch.Uniform,
                Source = new BitmapImage(new Uri("folder.png", UriKind.Relative)),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            stackPanel.Children.Add(image);

            TextBlock fileNameTextBlock = new TextBlock
            {
                Margin = new Thickness(5, 0, 0, 0),
                Text = file.FileName,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Left,
                HorizontalAlignment = HorizontalAlignment.Left,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Colors.LightSlateGray)
            };
            stackPanel.Children.Add(fileNameTextBlock);

            grid.Children.Add(stackPanel);
            Grid.SetColumn(stackPanel, 0);

            TextBlock idTextBlock = new TextBlock
            {
                Text = file.FileType,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = new SolidColorBrush(Colors.LightSlateGray)
            };
            grid.Children.Add(idTextBlock);
            Grid.SetColumn(idTextBlock, 1);

            TextBlock dateTextBlock = new TextBlock
            {
                Text = Convert.ToString(file.UploadDate),
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = new SolidColorBrush(Colors.LightSlateGray)
            };
            grid.Children.Add(dateTextBlock);
            Grid.SetColumn(dateTextBlock, 2);

            TextBlock sizeTextBlock = new TextBlock
            {
                Text = "1 Gb",
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = new SolidColorBrush(Colors.LightSlateGray)
            };
            grid.Children.Add(sizeTextBlock);
            Grid.SetColumn(sizeTextBlock, 3);
            Button actionButton = new Button
            {
                Content = "Видалити",
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(5),
                Width = 70,
                Height = 40,
                Background = new SolidColorBrush(Color.FromRgb(221, 221, 221)),
                Foreground = new SolidColorBrush(Colors.Black),
                BorderBrush = new SolidColorBrush(Colors.Black),
                BorderThickness = new Thickness(2),
                FontSize = 14,
            };

            var style = new Style(typeof(Button));
            var template = new ControlTemplate(typeof(Button));
            var borderFactory = new FrameworkElementFactory(typeof(Border));
            borderFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(15));
            borderFactory.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(Button.BackgroundProperty));
            borderFactory.SetValue(Border.BorderBrushProperty, new TemplateBindingExtension(Button.BorderBrushProperty));
            borderFactory.SetValue(Border.BorderThicknessProperty, new TemplateBindingExtension(Button.BorderThicknessProperty));

            var contentPresenterFactory = new FrameworkElementFactory(typeof(ContentPresenter));
            contentPresenterFactory.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            contentPresenterFactory.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);

            borderFactory.AppendChild(contentPresenterFactory);
            template.VisualTree = borderFactory;

            var mouseOverTrigger = new Trigger
            {
                Property = UIElement.IsMouseOverProperty,
                Value = true
            };
            mouseOverTrigger.Setters.Add(new Setter(Button.BackgroundProperty, new SolidColorBrush(Color.FromRgb(170, 170, 170))));
            template.Triggers.Add(mouseOverTrigger);

            var isPressedTrigger = new Trigger
            {
                Property = Button.IsPressedProperty,
                Value = true
            };
            isPressedTrigger.Setters.Add(new Setter(Button.BackgroundProperty, new SolidColorBrush(Color.FromRgb(136, 136, 136))));
            template.Triggers.Add(isPressedTrigger);

            style.Setters.Add(new Setter(Button.TemplateProperty, template));
            actionButton.Style = style;
            actionButton.Click += (sender, e) =>
            {
                e.Handled = true;
                MessageBoxResult result = MessageBox.Show(
            "Ви дійсно хочете видалити цей файл?",
            "Підтвердження видалення",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    DeleteFileAsync(file.Id);
                }
            };
            grid.Children.Add(actionButton);
            Grid.SetColumn(actionButton, 4);

            return grid;
        }
    }
}
