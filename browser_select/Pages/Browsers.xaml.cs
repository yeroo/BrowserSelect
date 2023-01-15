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

namespace browser_select.Pages
{
    /// <summary>
    /// Interaction logic for Browsers.xaml
    /// </summary>
    public partial class Browsers : Page
    {
        public Browsers()
        {
            InitializeComponent();
            var model = new Models.BrowsersModel();
            model.ParentProcessName = ParentProcess.FileName;
            var iconReader = new IconReader(ParentProcess.FullPath);
            model.ParentProcessIcon = iconReader.GetIconBitmapImage(0);
            model.SiteName = GetSiteName();

            model.Browsers = new ObservableCollection<Models.Browser>();
            foreach (var browser in Models.Browser.GetBrowsers())
            {
                if (browser == "browser_select") continue;
                var browserName = Models.Browser.GetBrowserGoodName(browser);
                var executable = Models.Browser.GetBrowserExecutable(browser);
                ImageSource icon = null;
                try
                {
                    var exec = executable.Replace("\"", "");
                    var iconR = new IconReader(exec);
                    icon = iconR.GetIconBitmapImage(0);
                }
                catch (Exception ex)
                {
                }
                if (!string.IsNullOrEmpty(browserName) && !string.IsNullOrEmpty(executable))
                {
                    var browserItem = new Models.Browser() { Name = browserName, Executable = executable };
                    if (icon != null)
                    {
                        browserItem.Icon = icon;
                    }
                    model.Browsers.Add(browserItem);
                }
            }
            this.DataContext = model;
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
            var browser = button.DataContext as Models.Browser;
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
            Application.Current.Shutdown();
        }
        
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //var selected = sender as 
            if (e.AddedItems != null && e.AddedItems.Count > 0) {
                var browser = e.AddedItems[0] as Models.Browser;
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
                Application.Current.Shutdown();
            }
        }
    }
}
