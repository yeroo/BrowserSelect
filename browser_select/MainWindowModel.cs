using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace browser_select
{
    public class MainWindowModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        string _parentProcessName;
        public string ParentProcessName
        {
            get
            {
                return _parentProcessName;
            }
            set
            {
                _parentProcessName = value;
                OnPropertyChanged("ParentProcessName");
            }
        }
        private string _siteName;
        public string SiteName
        {
            get { return _siteName; }
            set
            {
                _siteName = value;
                OnPropertyChanged("SiteName");
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ObservableCollection<Browser> Browsers { get; set; }
    }

    public class Browser
    {
        public string Name { get; set; }
        public string Executable { get; set; }

        public static string GetBrowserGoodName(string browser)
        {
            return _GetBrowserRegistryValue(browser, "Capabilities","ApplicationName");
        }
        private static string _GetBrowserRegistryValue(string browser, string keyName, string valueName)
        {
            var retVal = string.Empty;
            var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Clients\StartMenuInternet\" + browser + @"\" + keyName);
            if (key != null)
            {
                retVal = key.GetValue(valueName).ToString();
            }
            else
            {
                key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Clients\StartMenuInternet\" + browser + @"\" + keyName);
                if (key != null)
                {
                    retVal = key.GetValue(valueName).ToString();
                }
            }
            return retVal;
        }
        public static string GetBrowserExecutable(string browser)
        {
            return _GetBrowserRegistryValue(browser, @"shell\open\command", "");
        }
        public static List<string> GetBrowsers()
        {
            var browsers = new List<string>();
            var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Clients\StartMenuInternet");
            if (key != null)
            {
                foreach (var subKeyName in key.GetSubKeyNames())
                {
                    if (!browsers.Contains(subKeyName))
                    {
                        browsers.Add(subKeyName);
                    }
                }
            }
            key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Clients\StartMenuInternet");
            if (key != null)
            {
                foreach (var subKeyName in key.GetSubKeyNames())
                {
                    if (!browsers.Contains(subKeyName))
                    {
                        browsers.Add(subKeyName);
                    }
                }
            }
            return browsers;
        }
    }
}