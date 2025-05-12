using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace WebViewApplication
{
    /// <summary>  
    /// Interaction logic for App.xaml  
    /// </summary>  
    public partial class App : Application
    {
        private bool IsFirstRun()
        {
            // Check a flag file or registry entry
            string flagPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WebView", "firstrun.flag");

            if (!File.Exists(flagPath))
            {
                // Create the flag file for next run
                Directory.CreateDirectory(Path.GetDirectoryName(flagPath));
                File.WriteAllText(flagPath, "1");
                return true;
            }
            return false;
        }

        private bool IsDotNetRuntimeInstalled()
        {
            try
            {
                // Try using a .NET 8 specific feature
                var test = typeof(System.Runtime.Versioning.RequiresPreviewFeaturesAttribute);
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            // Skip if this is the installer process
            if (e.Args.Contains("--installer-mode"))
            {
                base.OnStartup(e);
                return;
            }

            if (IsFirstRun() && !IsDotNetRuntimeInstalled())
            {
                var result = MessageBox.Show(
                    ".NET Desktop Runtime 8.0 is required to run this application. Install now?",
                    "Runtime Required",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Path where your installer bundled the runtime
                        string installerPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "runtimes", "windowsdesktop-runtime-8.0.15-win-x64.exe");

                        Process.Start(new ProcessStartInfo
                        {
                            FileName = installerPath,
                            Arguments = "/install",
                            Verb = "runas", // Run as admin
                            UseShellExecute = true
                        });

                        // Close the app to allow installation
                        Application.Current.Shutdown();
                        return;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to install runtime: {ex.Message}");
                    }
                }
                else
                {
                    MessageBox.Show("The application cannot run without .NET 8.0 Runtime.");
                    Application.Current.Shutdown();
                    return;
                }
            }

            base.OnStartup(e);
        }

    }

}
