using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace BeShare.App.Method
{
    class Auth
    {
        public string userName { get; set; }
        public async void AuthMake()
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7147");
            string token = ReadToken();
            try
            {
                HttpResponseMessage response = await client.PostAsync("/Home/api/user/validuser?token=" + token, null);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(responseBody);
                    string userName = json.userName;
                    SaveData(userName);
                    SaveToken(token);
                    new Panel().Show();
                    new StartWindow().CloseWindow(); 
                }
                else
                {
                    new StartWindow().AuthStart();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Помилка {ex.Message}");
            }
        }
        public string ReadData()
        {
            try
            {
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "beshare.app", "data.txt");
                return File.ReadAllText(filePath, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new Exception($"Помилка читання даних: {ex.Message}");
            }
        }
        public void SaveData(string name)
        {
            try
            {
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "beshare.app", "data.txt");
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                File.WriteAllText(filePath, name, Encoding.UTF8);
            }
            catch(Exception ex)
            {
                throw new Exception($"Помилка читання даних: {ex.Message}");
            }
        }
        public string ReadToken()
        {
            try
            {
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "beshare.app", "token.txt");
                return File.ReadAllText(filePath, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new Exception($"Помилка читання токена: {ex.Message}");
            }
        }
        public void SaveToken(string token)
        {
            try
            {
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "beshare.app", "token.txt");
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                File.WriteAllText(filePath, token, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new Exception($"Помилка збереження токена: {ex.Message}");
            }
        }
    }
}
