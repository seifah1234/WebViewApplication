using Microsoft.Web.WebView2.Core;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace WebViewApplication
{
    public partial class MainWindow : Window
    {
        private const int WDA_MONITOR = 1;
        private const int WDA_EXCLUDEFROMCAPTURE = 11;
        private bool _isManualFullscreen = false;
        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        [DllImport("user32.dll")]
        private static extern bool SetWindowDisplayAffinity(IntPtr hWnd, int dwAffinity);


        [DllImport("user32.dll")]
        static extern IntPtr GetActiveWindow();

        private readonly string[] forbiddenProcesses = new string[]
        {
            "obs", "bandicam", "camtasia", "fraps", "snagit", "screencast",
            "screenrec", "xsplit", "dxtory", "action", "flashback", "debut",
            "icecream", "apowersoft", "nvidia", "geforce", "plays", "mirillis",
            "movavi", "capture", "recorder", "camstudio", "hypercam", "bbflashback",
            "gamebar", "game dvr", "overwolf", "msrtsc", "mstsc", "teamviewer",
            "anydesk", "parsec", "vnc", "radmin", "rustdesk", "chrome remote",
            "amddvr", "relive", "raptr", "shadowplay"
        };

        private DispatcherTimer recorderTimer;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                string userDataFolder = Path.Combine(
                                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                    "Original\\WebView\\WebView2Data");

                if (!Directory.Exists(userDataFolder))
                {
                    Directory.CreateDirectory(userDataFolder);
                }

                var env = await CoreWebView2Environment.CreateAsync(
                    userDataFolder: userDataFolder);

                SetWindowDisplayAffinity(GetActiveWindow(), WDA_MONITOR);
                await webView.EnsureCoreWebView2Async(env);

                webView.CoreWebView2InitializationCompleted += WebView_CoreWebView2InitializationCompleted;
                webView.Source = new Uri("https://hany-al-nashar-disktop-web.vercel.app");

                recorderTimer = new DispatcherTimer();
                recorderTimer.Interval = TimeSpan.FromSeconds(5);
                recorderTimer.Tick += SecurityTimer_Tick;
                recorderTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Initialization failed: {ex.Message}");
            }
        }

        private async void WebView_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                try
                {
                    IntPtr hwnd = ((System.Windows.Interop.HwndSource)PresentationSource.FromVisual(webView)).Handle;
                    if (Environment.OSVersion.Version.Build >= 19041)
                    {
                        SetWindowDisplayAffinity(hwnd, WDA_EXCLUDEFROMCAPTURE);
                    }
                    else
                    {
                        SetWindowDisplayAffinity(hwnd, WDA_MONITOR);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to set display affinity.\n\nError: {ex.Message}");
                }

            }
            else
            {
                MessageBox.Show($"WebView2 initialization failed: {e.InitializationException.Message}");
            }
        }

        private void EnterFullscreen()
        {

            this.WindowStyle = WindowStyle.None;
            this.Topmost = true;

        }

        private void ExitFullscreen()
        {

            this.WindowStyle = WindowStyle.SingleBorderWindow;
            this.Topmost = false;
        }

        
        private void SecurityTimer_Tick(object sender, EventArgs e)
        {
            var processes = Process.GetProcesses();
            foreach (var process in processes)
            {
                try
                {
                    string name = process.ProcessName.ToLower();
                    if (forbiddenProcesses.Any(p => name.Contains(p)))
                    {
                        MessageBox.Show("Screen recording software detected. The app will now close.", "Security Alert", MessageBoxButton.OK, MessageBoxImage.Warning);
                        Application.Current.Shutdown();
                        break;
                    }
                }
                catch
                {
                    // Ignore process access errors
                }
            }
        }


        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == System.Windows.Input.Key.F11)
            {
                if (_isManualFullscreen)
                {
                    ExitFullscreen();
                }
                else
                {
                    EnterFullscreen();
                }
                _isManualFullscreen = !_isManualFullscreen;
            }
        }

    }
}