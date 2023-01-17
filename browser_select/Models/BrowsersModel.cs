using browser_select.Helpers;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace browser_select.Models
{
    public class BrowsersModel : INotifyPropertyChanged
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
        ImageSource? _parentProcessIcon;
        public ImageSource? ParentProcessIcon
        {
            get
            {
                return _parentProcessIcon;
            }
            set
            {
                _parentProcessIcon = value;
                OnPropertyChanged("ParentProcessIcon");
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
        private SaveOption _saveOption;

        public SaveOption SaveOption
        {
            get => _saveOption;
            set
            {
                _saveOption = value;
                OnPropertyChanged("SaveOption");
            }
        }

        private bool _isSaveOptionEnabled;
        public bool IsSaveOptionEnabled
        {
            get => _isSaveOptionEnabled;
            set
            {
                _isSaveOptionEnabled = value;
                OnPropertyChanged("IsSaveOptionEnabled");
            }
        }

        public IEnumerable<KeyValuePair<string, string>> SaveOptionList
        {
            get
            {
                return Helpers.EnumHelper.GetAllValuesAndDescriptions<SaveOption>();
            }
        }
        public static BrowsersModel Create()
        {
            var model = new Models.BrowsersModel();
            model.ParentProcessName = CallerHelper.FileName;
            var iconHelperCaller = new IconHelper(CallerHelper.FullPath);
            model.ParentProcessIcon = iconHelperCaller.GetIconBitmapImage(0);
            model.SiteName = RegestryHelper.GetSiteName();
            model.Browsers = new ObservableCollection<Browser>();
            foreach (var browser in BrowserHelper.GetBrowsers())
            {
                if (browser == "browser_select") continue;
                var browserName = BrowserHelper.GetBrowserGoodName(browser);
                var executable = BrowserHelper.GetBrowserExecutable(browser);
                ImageSource icon = null;
                try
                {
                    var exec = executable.Replace("\"", "");
                    var iconHelper = new Helpers.IconHelper(exec);
                    icon = iconHelper.GetIconBitmapImage(0);
                }
                catch (Exception ex)
                {
                }
                if (!string.IsNullOrEmpty(browserName) && !string.IsNullOrEmpty(executable))
                {
                    var browserItem = new Browser() { Name = browserName, Executable = executable };
                    if (icon != null)
                    {
                        browserItem.Icon = icon;
                    }
                    model.Browsers.Add(browserItem);
                }
            }
            return model;
        }
        public void LaunchBrowser(Browser browser)
        {
            var args = Environment.GetCommandLineArgs();

            if (IsSaveOptionEnabled == true && !(string.IsNullOrEmpty(SiteName) && SaveOption == SaveOption.Site))
            {
                //load settings
                var settingsFile = FileHelper.GetSettingsFile();
                if (settingsFile == null)
                {
                    settingsFile = new SettingsFile();
                }
                if (settingsFile.AutoAppsAndSites == null)
                {
                    settingsFile.AutoAppsAndSites = new Dictionary<string, Dictionary<string, List<string>>>();
                }
                string site = string.Empty;
                string caller = string.Empty;

                switch (SaveOption)
                {
                    case SaveOption.Caller:
                        site = "*";
                        caller = ParentProcessName;
                        break;
                    case SaveOption.Site:
                        site = SiteName;
                        caller = "*";
                        break;
                    case SaveOption.Both:
                        site = SiteName;
                        caller = ParentProcessName;
                        break;
                    default:
                        break;
                }
                Dictionary<string, List<string>>? browserSetting = null;
                if (settingsFile.AutoAppsAndSites.TryGetValue(browser.Name, out browserSetting) == false)
                {
                    browserSetting = new Dictionary<string, List<string>>();
                }
                List<string>? sites = null;
                if (browserSetting.TryGetValue(caller, out sites) == false)
                {
                    sites = new List<string>();
                }
                if (sites.Contains(site) == false && sites.Contains("*") == false)
                {
                    sites.Add(site);
                }
                browserSetting[caller] = sites;
                settingsFile.AutoAppsAndSites[browser.Name] = browserSetting;
                FileHelper.SaveSettingsFile(settingsFile);
            }

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
        public ObservableCollection<Browser> Browsers { get; set; }
    }
    public enum SaveOption
    {
        [Description("Always open for this caller")]
        Caller,
        [Description("Always open for this site")]
        Site,
        [Description("Always open for this caller and site")]
        Both
    }
   
}
