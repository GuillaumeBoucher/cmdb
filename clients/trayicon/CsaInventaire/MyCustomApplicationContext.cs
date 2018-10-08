using AutoUpdaterDotNET;
using CsaInventaire;
using CsaInventaire.Properties;
using System;
using System.Windows.Forms;


public class MyCustomApplicationContext : ApplicationContext
{
    private NotifyIcon trayIcon;

    public MyCustomApplicationContext()
    {
        AutoUpdater.Start("http://rbsoft.org/updates/AutoUpdaterTest.xml");
        // Initialize Tray Icon
        trayIcon = new NotifyIcon()
        {
            Icon = Resources.computer_search_icon,
            ContextMenu = new ContextMenu(new MenuItem[] {
                new MenuItem("Version", MVersion),
                new MenuItem("Config", Configuration),
                new MenuItem("Exit", Exit)                
            }),
            Visible = true
        };
    }

    private void MVersion(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private void Configuration(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    void Exit(object sender, EventArgs e)
    {
        // Hide tray icon, otherwise it will remain shown until user mouses over it
        trayIcon.Visible = false;

        Application.Exit();
    }
}