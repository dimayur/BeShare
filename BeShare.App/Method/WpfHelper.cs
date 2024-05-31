using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeShare.App.Method
{
    public class WpfHelper
    {
        public void RegisterProgram(string scheme, string applicationPath)
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
