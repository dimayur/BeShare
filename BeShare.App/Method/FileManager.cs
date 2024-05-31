using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace BeShare.App.Method
{
    public class FileData
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public DateTime UploadDate { get; set; }
        public string Size { get; set; }
    }
    internal class FileManager
    {
        public async void FileReadMake()
        {
            string token = new Auth().ReadToken();

            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7147");

            try
            {
                HttpResponseMessage response = await client.PostAsync($"/Home/api/getfiles?token={token}", null);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(responseBody);
                    SaveFile(json);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Помилка {ex.Message}");
            }
        }
        public List<FileData> ReadFile()
        {
            try
            {
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "beshare.app", "files.txt");
                string json = File.ReadAllText(filePath, Encoding.UTF8);
                return JsonConvert.DeserializeObject<List<FileData>>(json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Помилка читання файлу: {ex.Message}");
            }
        }
        public void SaveFile(dynamic files)
        {
            try
            {
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "beshare.app", "files.txt");
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                string json = JsonConvert.SerializeObject(files);

                File.WriteAllText(filePath, json, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new Exception($"Помилка збереження файлу: {ex.Message}");
            }
        }


    }
}
