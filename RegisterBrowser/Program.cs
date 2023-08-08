using browser_select;
using Microsoft.Win32;
using System.Diagnostics;

string application = "Browser Select";
string applicationClasses = "BrowserSelectHTML";

string applicationPath = $@"SOFTWARE\Clients\StartMenuInternet\{application}";
string applicationCapabilityPath = $"{applicationPath}\\Capabilities";

string appplicationExecutableFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
string appplicationExecutablePath = Path.Combine(appplicationExecutableFolder, "browser_select.exe");

bool have_errors = false;
if (!Path.Exists(appplicationExecutablePath))
{
    have_errors = true;
    var appplicationExecutableFolder2 = appplicationExecutableFolder.Replace("RegisterBrowser", "browser_select",StringComparison.InvariantCultureIgnoreCase);
    var appplicationExecutableFolder3 = appplicationExecutableFolder2.Replace(@"\bin\Debug\net7.0", @"\bin\Debug\net7.0-windows", StringComparison.InvariantCultureIgnoreCase);
    appplicationExecutablePath = Path.Combine(appplicationExecutableFolder3, "browser_select.exe");
}
if (!Path.Exists(appplicationExecutablePath))
{
    have_errors = true;
    appplicationExecutablePath = @"C:\Users\bkudryashov\Source\GitHub\browser_select\browser_select\bin\Debug\net7.0-windows\browser_select.exe";
}

// Registered Applications

    try
    {
        // SOFTWARE\RegisteredApplications
        var registeredApplicationsMenuKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\RegisteredApplications", true);
        if (args.Length == 1 && args[0] == "/Uninstall")
        {
            if (registeredApplicationsMenuKey.GetValue(application) != null)
            {
                registeredApplicationsMenuKey.DeleteValue(application);
                Console.Write(@"browser_select.exe deleted from \SOFTWARE\RegisteredApplications\");
            }
        }
        else
        {
            registeredApplicationsMenuKey.SetValue(application, applicationCapabilityPath);
            Console.Write(@"browser_select.exe added to \SOFTWARE\RegisteredApplications\");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error 1 {ex}");
        have_errors = true;
    }


// SOFTWARE\Classes
try
{
    var classesKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Classes", true);
    if (args.Length == 1 && args[0] == "/Uninstall")
    {
        if (classesKey.OpenSubKey(applicationClasses) != null)
        {
            classesKey.DeleteSubKeyTree(applicationClasses);
            Console.WriteLine(@"browser select removed from SOFTWARE\Classes");
        }
    }
    else
    {
        var classesApplicationsKey = classesKey.CreateSubKey(applicationClasses);
        var defaultIconKey = classesApplicationsKey.CreateSubKey("DefaultIcon");
        defaultIconKey.SetValue("", appplicationExecutablePath+",0");

        var shellKey = classesApplicationsKey.CreateSubKey("shell\\open\\command");
        shellKey.SetValue("", appplicationExecutablePath + " \"%1\"");
        Console.WriteLine(@"browser select added to SOFTWARE\Classes");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error 1 {ex}");
    have_errors = true;
}

// StartMenuInternet \ Capabilities

try
{
    if (args.Length == 1 && args[0] == "/Uninstall")
    {
        if (Registry.CurrentUser.OpenSubKey(applicationPath) != null)
        {
            Registry.CurrentUser.DeleteSubKeyTree(applicationPath);
            Console.WriteLine($"browser select removed from {applicationPath}");
        }
    }
    else
    {
        var applicationKey = Registry.CurrentUser.CreateSubKey(applicationPath);


        var capabilitiesKey = applicationKey.CreateSubKey("Capabilities");
        // register application description
        capabilitiesKey.SetValue($@"ApplicationDescription", "Select browser to open link");
        capabilitiesKey.SetValue($@"ApplicationName", application);
        capabilitiesKey.SetValue("ApplicationIcon", appplicationExecutablePath + ",0");

        var fileAssociationsKey = capabilitiesKey.CreateSubKey("FileAssociations");
        // register file associations
        var extensions = new string[]{
        ".htm",
        ".html",
        ".pdf",
        ".mht",
        ".mhtml",
        ".shtml",
        ".svg",
        ".webp",
        ".xht",
        ".xhtml",
        ".xml",
    };

        foreach (var extension in extensions)
        {
            fileAssociationsKey.SetValue(extension, applicationClasses);
        }

        var startMenuKey = capabilitiesKey.CreateSubKey("StartMenu");
        startMenuKey.SetValue("StartMenuInternet", application);

        var urlAssociationsKey = capabilitiesKey.CreateSubKey("URLAssociations");
        // register url associations
        var urlAssociations = new string[]{
        "ftp",
        "http",
        "https",
        "irc",
        "mailto",
        "mms",
        "news",
        "nntp",
        "sms",
        "smsto",
        "snews",
        "tel",
        "urn",
        "webcal",
    };

        foreach (var urlAssociation in urlAssociations)
        {
            urlAssociationsKey.SetValue(urlAssociation, applicationClasses);
        }

        var defaultIconKey = applicationKey.CreateSubKey("DefaultIcon");
        // register default icon
        defaultIconKey.SetValue("", $@"{appplicationExecutablePath},0");


        // register shell command
        var shellCommandKey = applicationKey.CreateSubKey(@"shell\open\command");
        shellCommandKey.SetValue("", $"{appplicationExecutablePath} %1");
        Console.WriteLine($"browser select added to {applicationPath}");
        // NotifySystemOfNewRegistration
        Exports.SHChangeNotify(HChangeNotifyEventID.SHCNE_ASSOCCHANGED, HChangeNotifyFlags.SHCNF_DWORD | HChangeNotifyFlags.SHCNF_FLUSH, IntPtr.Zero, IntPtr.Zero);
        Thread.Sleep(1000);
    }

}
catch (Exception ex)
{
    Console.WriteLine($"Error 2 {ex}");
    have_errors = true;
}

if (have_errors)
{
    Console.WriteLine("Errors!");
    Console.ReadLine();
}

