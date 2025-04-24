using DecentPass.Views;

namespace DecentPass
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register additional routes if needed
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        }
    }
}
