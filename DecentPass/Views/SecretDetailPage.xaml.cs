using DecentPass.ViewModels;

namespace DecentPass.Views
{
    public partial class SecretDetailPage : ContentPage
    {
        public SecretDetailPage(string secretName)
        {
            InitializeComponent();
            BindingContext = new SecretDetailViewModel(Navigation, secretName);
        }
    }
}