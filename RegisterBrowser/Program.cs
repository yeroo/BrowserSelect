using browser_select;
using Microsoft.Win32;

string application = "browser_select";
string applicationClasses = "browser_selectHTML";

string applicationPath = $@"SOFTWARE\Clients\StartMenuInternet\{application}";
string applicationCapabilityPath = $"{applicationPath}\\Capabilities";

string appplicationExecutablePath = @"C:\Users\bkudryashov\Source\GitHub\browser_select\browser_select\bin\Debug\net6.0-windows\browser_select.exe";

// SOFTWARE\RegisteredApplications
var registeredApplicationsMenuKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\RegisteredApplications", true);
if (args.Length == 1 && args[0] == "unregister")
{
    if (registeredApplicationsMenuKey.GetValue(application) != null)
    {
        registeredApplicationsMenuKey.DeleteValue(application);
    }
}
else
{
    registeredApplicationsMenuKey.SetValue(application, applicationCapabilityPath);
}


// SOFTWARE\Classes

var classesKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Classes", true);
if (args.Length == 1 && args[0] == "unregister")
{
    if (classesKey.OpenSubKey(applicationClasses) != null)
    {
        classesKey.DeleteSubKeyTree(applicationClasses);
    }
}
else
{
    var classesApplicationsKey = classesKey.CreateSubKey(applicationClasses);
    var defaultIconKey = classesApplicationsKey.CreateSubKey("DefaultIcon");
    defaultIconKey.SetValue("", appplicationExecutablePath+",0");

    var shellKey = classesApplicationsKey.CreateSubKey("shell\\open\\command");
    shellKey.SetValue("", appplicationExecutablePath + " \"%1\"");
}


if (args.Length == 1 && args[0] == "unregister")
{
    if (Registry.LocalMachine.OpenSubKey(applicationPath) != null)
    {
        Registry.LocalMachine.DeleteSubKeyTree(applicationPath);
    }
}
else
{
    var applicationKey = Registry.LocalMachine.CreateSubKey(applicationPath);


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

    // NotifySystemOfNewRegistration
    Exports.SHChangeNotify(HChangeNotifyEventID.SHCNE_ASSOCCHANGED, HChangeNotifyFlags.SHCNF_DWORD | HChangeNotifyFlags.SHCNF_FLUSH, IntPtr.Zero, IntPtr.Zero);
    Thread.Sleep(1000);
}



