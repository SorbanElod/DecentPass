using DecentPass.Services;
using DecentPass.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DecentPass.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly GopassClient _gopassClient;
        private readonly INavigation _navigation;
        private string _serverUrl;
        private string _passphrase;
        private string _errorMessage;

        public string ServerUrl
        {
            get => _serverUrl;
            set => SetProperty(ref _serverUrl, value);
        }

        public string Passphrase
        {
            get => _passphrase;
            set => SetProperty(ref _passphrase, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand LoginCommand { get; }

        public LoginViewModel(INavigation navigation)
        {
            Title = "Login";
            _navigation = navigation;
            ServerUrl = "http://localhost:50051"; // Default server URL
            LoginCommand = new Command(async () => await LoginAsync());
        }

        private async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(ServerUrl) || string.IsNullOrWhiteSpace(Passphrase))
            {
                ErrorMessage = "Please enter server URL and passphrase";
                return;
            }

            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                var gopassClient = new GopassClient(ServerUrl);
                bool success = await gopassClient.AuthenticateAsync(Passphrase);

                if (success)
                {
                    // Store the client in App.Current.Resources for global access
                    Application.Current.Resources["GopassClient"] = gopassClient;

                    // Navigate to secrets list page
                    await _navigation.PushAsync(new SecretsListPage());
                }
                else
                {
                    ErrorMessage = "Authentication failed. Please check your passphrase.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }


}
