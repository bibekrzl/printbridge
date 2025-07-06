using System.Drawing;
using System.Windows.Forms;

namespace PrintBridgeTrayApp;

static class Program
{
    private static NotifyIcon? trayIcon;
    private static WebServer? webServer;

    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        
        // Create and configure the main form (hidden)
        var mainForm = new Form
        {
            WindowState = FormWindowState.Minimized,
            ShowInTaskbar = false,
            Visible = false
        };

        // Set up tray icon
        SetupTrayIcon();

        // Start web server in background
        StartWebServer();

        // Optional: Add to Windows startup
        AddToStartup();

        Console.WriteLine("PrintBridge Tray App started. Web server running on http://localhost:8080");
        Console.WriteLine("Right-click tray icon to exit.");

        // Run the application (form will be hidden)
        Application.Run(mainForm);
    }

    private static void SetupTrayIcon()
    {
        trayIcon = new NotifyIcon
        {
            Icon = SystemIcons.Application, // You can replace with custom icon
            Text = "PrintBridge Running",
            Visible = true
        };

        // Create context menu
        var contextMenu = new ContextMenuStrip();
        contextMenu.Items.Add("Exit", null, (sender, e) => ExitApplication());
        
        trayIcon.ContextMenuStrip = contextMenu;
        trayIcon.DoubleClick += (sender, e) => ShowMainWindow();
    }

    private static void ShowMainWindow()
    {
        // Show the main window if needed (optional)
        Application.OpenForms[0]?.Show();
    }

    private static void ExitApplication()
    {
        trayIcon?.Dispose();
        webServer?.Stop();
        Application.Exit();
    }

    private static void StartWebServer()
    {
        webServer = new WebServer();
        var serverThread = new Thread(() =>
        {
            try
            {
                webServer.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Web server error: {ex.Message}");
            }
        });
        serverThread.IsBackground = true;
        serverThread.Start();
    }

    private static void AddToStartup()
    {
        try
        {
            var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            
            if (key != null)
            {
                var appPath = Application.ExecutablePath;
                key.SetValue("PrintBridge", appPath);
                Console.WriteLine("Added to Windows startup.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to add to startup: {ex.Message}");
        }
    }
} 