using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace browser_select
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowModel();
            Model.ParentProcessName = ParentProcess.FileName;
            Model.SiteName = GetSiteName();
            Model.Browsers = new ObservableCollection<Browser>();
            foreach (var browser in Browser.GetBrowsers())
            {
                if (browser == "browser_select") continue;
                var browserName = Browser.GetBrowserGoodName(browser);
                var executable = Browser.GetBrowserExecutable(browser);
                if (!string.IsNullOrEmpty(browserName) && !string.IsNullOrEmpty(executable))
                {
                    Model.Browsers.Add(new Browser() { Name = browserName, Executable = executable });
                }
            }
            //Model.Browsers.Add(new Browser() { Name = "Google Chrome", Executable = Browser.GetBrowserExecutable("Google Chrome") });
            //Model.Browsers.Add(new Browser() { Name = "Microsoft Edge", Executable = Browser.GetBrowserExecutable("Microsoft Edge") });
        }

        public MainWindowModel Model
        {
            get
            {
                return (MainWindowModel)DataContext;
            }
        }

        private string GetSiteName()
        {
            var retVal = string.Empty;
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                var url = args[1];
                // get host name from url
                var uriAddress = new Uri(url);
                retVal = uriAddress.Host;

            }
            return retVal;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;
            var browser = button.DataContext as Browser;
            if (browser == null) return;
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                var url = args[1];
                Process.Start(browser.Executable, url);
            }
            else
            {
                Process.Start(browser.Executable);
            }
            this.Close();
        }
    }
}