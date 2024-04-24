using System.Windows;
using System.IO;
using System.IO.Compression;
using Microsoft.Win32;
using Wpf.Ui.Appearance;

namespace BeShare.App
{    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ApplicationThemeManager.Apply(this);
        }
        private void CreateArchive_Click(object sender, RoutedEventArgs e)
        {
            var filesToArchive = fileListBox.SelectedItems.OfType<string>().ToArray();

            if (filesToArchive.Length == 0)
            {
                MessageBox.Show("Вибери файл.");
                return;
            }

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "ZIP Archive (*.zip)|*.zip",
                Title = "Зберегти ZIP архів"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    using (var archiveStream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                    using (var archive = new ZipArchive(archiveStream, ZipArchiveMode.Create))
                    {
                        foreach (var file in filesToArchive)
                        {
                            var entry = archive.CreateEntry(Path.GetFileName(file));
                            using (var entryStream = entry.Open())
                            using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
                            {
                                fileStream.CopyTo(entryStream);
                            }
                        }
                    }

                    MessageBox.Show("Успішно!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка: {ex.Message}");
                }
            }
        }
        private void MainWindow_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (var file in files)
                {
                    if (File.Exists(file))
                    {
                        fileListBox.Items.Add(file);
                    }
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Panel me = new Panel();
            me.Show();
        }
    }
}