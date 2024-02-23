using System.Configuration;
using System.Data;
using System.Windows;

namespace BeShare.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            AppArgs.Args = e.Args;
        }
    }

}
