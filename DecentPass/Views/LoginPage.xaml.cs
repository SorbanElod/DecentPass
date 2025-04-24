using DecentPass.ViewModels;

namespace DecentPass.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            BindingContext = new LoginViewModel(Navigation);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if(BindingContext is LoginViewModel viewModel)
            {
                viewModel.ErrorMessage = string.Empty;
            }
        }
    }
}