using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace browser_select.Models
{
    public class SettingsModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _jsonAutoAppsAndSites;
        public string JsonAutoAppsAndSites
        {
            get => _jsonAutoAppsAndSites;
            set
            {
                _jsonAutoAppsAndSites = value;
                OnPropertyChanged("JsonAutoAppsAndSites");
            }
        }
        public void LoadSettings(SettingsFile settings)
        {
            JsonAutoAppsAndSites = JsonConvert.SerializeObject(settings.AutoAppsAndSites);
        }

        public (SettingsFile, bool) StoreSettings()
        {
            (SettingsFile, bool) retVal = (new SettingsFile(), false);
            try
            {
                retVal.Item1.AutoAppsAndSites = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, List<string>>>>(JsonAutoAppsAndSites);
                retVal.Item2 = true;
            }
            catch { }
            return retVal;
        }
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    public class SettingsFile
    {
        public Dictionary<string, Dictionary<string, List<string>>> AutoAppsAndSites { get; set; }
    }
}
