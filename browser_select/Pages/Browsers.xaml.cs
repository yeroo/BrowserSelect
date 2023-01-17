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
using browser_select.Helpers;
using browser_select.Models;

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
            this.DataContext = BrowsersModel.Create();
        }

        public BrowsersModel Model
        {
            get => (BrowsersModel)DataContext;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //var selected = sender as 
            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                var browser = e.AddedItems[0] as Browser;
                if (browser == null) return;
                if (Model != null)
                {
                    Model.LaunchBrowser(browser);
                }
            }
        }
    }
}
