using System.Windows;
using System.IO;
using System.IO.Compression;
using Microsoft.Win32;
using System.Net.Http;
using BeShare.App.Method;
using System.Net.Http.Headers;
using SharpCompress.Common;

namespace BeShare.App
{   
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        // new Auth().ReadToken();
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private async Task UploadFileAsync(string filePath)
        {
            string url = "https://localhost:7147/Home/api/upload";
            string token = new Auth().ReadToken();

            if (string.IsNullOrWhiteSpace(token))
            {
                MessageBox.Show("Токен порожній або відсутній.");
                return;
            }

            using (var httpClient = new HttpClient())
            {
                url += "?token=" + token;

                byte[] fileBytes = File.ReadAllBytes(filePath);

                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new ByteArrayContent(fileBytes), "file", Path.GetFileName(filePath));
                    var response = await httpClient.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Файл успішно завантажено!");
                    }
                    else
                    {
                        MessageBox.Show("Помилка під час завантаження файлу: " + response.ReasonPhrase);
                    }
                }
            }
        }

        private async void SelectFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Виберіть папку",
                Filter = "Folder|*.this.file.does.not.exist"
            };

            var dialogResult = dialog.ShowDialog();
            if (dialogResult.HasValue && dialogResult.Value)
            {
                string selectedFolderPath = Path.GetDirectoryName(dialog.FileName);
                await CreateArchive(selectedFolderPath);
            }
        }
        private async Task CreateArchive(string selectedFolderPath)
        {
            var filesInFolder = Directory.GetFiles(selectedFolderPath);

            if (filesInFolder.Length == 0)
            {
                MessageBox.Show("Оберіть папку, яка містить файли для створення архіву.");
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
                    string zipFilePath = saveFileDialog.FileName;
                    using (var archiveStream = new FileStream(zipFilePath, FileMode.Create))
                    using (var archive = new ZipArchive(archiveStream, ZipArchiveMode.Create))
                    {
                        foreach (var file in filesInFolder)
                        {
                            var entry = archive.CreateEntry(Path.GetFileName(file));
                            using (var entryStream = entry.Open())
                            using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
                            {
                                fileStream.CopyTo(entryStream);
                            }
                        }
                    }

                    MessageBox.Show("Архів створено успішно!");
                    await UploadFileAsync(zipFilePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка: {ex.Message}");
                }
            }
        }
        private async void CreateArchive_Click(object sender, RoutedEventArgs e)
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
                    string zipFilePath = saveFileDialog.FileName;
                    using (var archiveStream = new FileStream(zipFilePath, FileMode.Create))
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

                    MessageBox.Show("Архів створено успішно!");
                    await UploadFileAsync(zipFilePath);
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

    }
}