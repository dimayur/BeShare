using BeShare.App.Method;
using FileManagerUI.Custom_Controls;
using System.Windows;
using System.Windows.Controls;

namespace BeShare.App
{
    /// <summary>
    /// Interaction logic for Panel.xaml
    /// </summary>
    public partial class Panel : Window
    {
        public Panel()
        {
            InitializeComponent();
            Auth auth = new Auth();
            auth.userName = auth.ReadData();
            this.DataContext = auth;
        }

        private void mediumSizeButtons_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Refresh(object sender, RoutedEventArgs e)
        {
            
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow addfiles = new MainWindow();
            addfiles.Show();
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
