using BeShare.App.Method;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private void GenerateTable(List<FileData> files)
        {
            foreach (var file in files)
            {
                var button = new Button();
                button.Style = FindResource("ButtonStyle1") as Style;

                var grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(100) });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(140) });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(120) });

                var stackPanel = new StackPanel();
                stackPanel.Orientation = Orientation.Horizontal;
                stackPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
                stackPanel.VerticalAlignment = VerticalAlignment.Stretch;

                var image = new Image();
                image.Margin = new Thickness(5, 0, 0, 0);
                image.Width = 50;
                image.Stretch = System.Windows.Media.Stretch.Uniform;
                image.Source = new BitmapImage(new Uri("/Custom Controls/music.png", UriKind.Relative));
                image.HorizontalAlignment = HorizontalAlignment.Left;

                var textBlock1 = new TextBlock();
                textBlock1.Margin = new Thickness(5, 0, 0, 0);
                textBlock1.Text = file.FileName;
                textBlock1.VerticalAlignment = VerticalAlignment.Center;
                textBlock1.TextAlignment = TextAlignment.Left;
                textBlock1.HorizontalAlignment = HorizontalAlignment.Left;
                textBlock1.FontWeight = FontWeights.Bold;
                textBlock1.Foreground = Brushes.LightSlateGray;

                var textBlock2 = new TextBlock();
                textBlock2.Text = file.Id.ToString();
                textBlock2.VerticalAlignment = VerticalAlignment.Center;
                textBlock2.TextAlignment = TextAlignment.Center;
                textBlock2.HorizontalAlignment = HorizontalAlignment.Center;
                textBlock2.Foreground = Brushes.LightSlateGray;

                var textBlock3 = new TextBlock();
                textBlock3.Text = file.UploadDate.ToString("MMM dd, yyyy");
                textBlock3.VerticalAlignment = VerticalAlignment.Center;
                textBlock3.TextAlignment = TextAlignment.Center;
                textBlock3.HorizontalAlignment = HorizontalAlignment.Center;
                textBlock3.Foreground = Brushes.LightSlateGray;

                var textBlock4 = new TextBlock();
                textBlock4.Text = file.Size;
                textBlock4.VerticalAlignment = VerticalAlignment.Center;
                textBlock4.TextAlignment = TextAlignment.Center;
                textBlock4.HorizontalAlignment = HorizontalAlignment.Center;
                textBlock4.Foreground = Brushes.LightSlateGray;

                stackPanel.Children.Add(image);
                stackPanel.Children.Add(textBlock1);

                Grid.SetColumn(stackPanel, 0);
                Grid.SetColumn(textBlock2, 1);
                Grid.SetColumn(textBlock3, 2);
                Grid.SetColumn(textBlock4, 3);

                grid.Children.Add(stackPanel);
                grid.Children.Add(textBlock2);
                grid.Children.Add(textBlock3);
                grid.Children.Add(textBlock4);

                button.Content = grid;

                // Додати кнопку до відповідного контейнера, наприклад, StackPanel чи Grid у вашому WPF-інтерфейсі
                Deamn.Children.Add(button);
            }
        }

        public FileListControl()
        {
            InitializeComponent();
            GenerateTable(new FileManager().ReadFile());
        }
    }
}
