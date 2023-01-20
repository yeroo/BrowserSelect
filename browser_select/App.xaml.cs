using browser_select.Helpers;
using browser_select.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace browser_select
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var settingsFile = FileHelper.GetSettingsFile();
            // get parent process

            var caller = CallerHelper.FileName;
            var site = RegestryHelper.GetSiteName();
            var browsers = BrowserHelper.GetBrowsers();

            var found = false;
            if (settingsFile != null && settingsFile.AutoAppsAndSites != null)
            {
                foreach (var browser in browsers)
                {
                    Dictionary<string, List<string>>? browserSetting = null;

                    if (settingsFile.AutoAppsAndSites.TryGetValue(browser, out browserSetting))
                    {
                        List<string>? sites = null;
                        if (browserSetting != null && browserSetting.TryGetValue("*", out sites))
                        {
                            if (sites != null && (sites.Contains("*") || sites.Contains(site)))
                            {
                                found = true;
                               
                            }
                        }
                        if (!found && browserSetting != null && browserSetting.TryGetValue(caller, out sites))
                        {
                            if (sites != null && (sites.Contains("*") || sites.Contains(site)))
                            {
                                found = true;
                               
                            }
                        }
                        if (found)
                        {
                            var args = Environment.GetCommandLineArgs();
                            var executable = BrowserHelper.GetBrowserExecutable(browser);
                            if (args.Length > 1)
                            {
                                var url = args[1];
                                Process.Start(executable, url);
                            }
                            else
                            {
                                Process.Start(executable);
                            }
                            break;
                        }
                    }
                }
            }
            if (found)
            {
                Application.Current.Shutdown();
            }
            else
            {                // set height in code
                // base height with one item is 406, each additional item is 77
                var browserCount = BrowserHelper.GetBrowsers().Count;
                var additionalItems = browserCount > 4 ? 3 : browserCount - 1;
                var height = 408 + additionalItems * 78;
                MainWindow mainView = new MainWindow()
                {
                    Height= height
                };
                mainView.Show();
            }
        }
    }
}
