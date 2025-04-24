using DecentPass.ViewModels;
using DecentPass.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DecentPass
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            // Start the platform sidecar
            StartPlatformSidecar();

            // Create and return the main window with AppShell
            return new Window(new AppShell());
        }

        // Run the gRPC server
        // Currently implemented for Windows and macOS
        private static void StartPlatformSidecar()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                string sidecarPath = Path.Combine(AppContext.BaseDirectory, "Sidecars", "gopass-grpc.exe");
                if (File.Exists(sidecarPath))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = sidecarPath,
                        UseShellExecute = true,
                        CreateNoWindow = true
                    });
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                // If it's a .app bundle
                var appBundlePath = Path.Combine(AppContext.BaseDirectory, "Sidecars", "gopass-grpc.app");
                if (Directory.Exists(appBundlePath))
                {
                    Process.Start("open", $"\"{appBundlePath}\"");
                }
                else
                {
                    // If it's just a Unix binary
                    var unixBinary = Path.Combine(AppContext.BaseDirectory, "Sidecars", "gopass-grpc");
                    if (File.Exists(unixBinary))
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = unixBinary,
                            UseShellExecute = true
                        });
                    }
                }
            }
            else
            {
                // On mobile platforms, log or skip
                System.Diagnostics.Debug.WriteLine("Sidecar launching not supported on this platform.");
            }
        }
    }
}