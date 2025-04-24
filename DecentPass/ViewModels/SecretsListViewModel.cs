using DecentPass.Services;
using DecentPass.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DecentPass.ViewModels
{
    public class SecretsListViewModel : BaseViewModel
    {
        private readonly GopassClient _gopassClient;
        private readonly INavigation _navigation;
        private ObservableCollection<string> _secrets;
        private string _searchText;
        private List<string> _allSecrets;

        public ObservableCollection<string> Secrets
        {
            get => _secrets;
            set => SetProperty(ref _secrets, value);
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    FilterSecrets();
                }
            }
        }

        public ICommand RefreshCommand { get; }
        public ICommand AddSecretCommand { get; }
        public ICommand SelectSecretCommand { get; }
        public ICommand LogoutCommand { get; }

        public SecretsListViewModel(INavigation navigation)
        {
            Title = "Passwords";
            _navigation = navigation;
            _gopassClient = Application.Current.Resources["GopassClient"] as GopassClient;
            Secrets = new ObservableCollection<string>();

            RefreshCommand = new Command(async () => await LoadSecretsAsync());
            AddSecretCommand = new Command(async () => await AddSecretAsync());
            SelectSecretCommand = new Command<string>(async (secret) => await SelectSecretAsync(secret));
            LogoutCommand = new Command(async () => await LogoutAsync());

            // Load secrets when the view model is created
            Task.Run(async () => await LoadSecretsAsync());
        }

        private async Task LoadSecretsAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                _allSecrets = await _gopassClient.ListSecretsAsync();
                FilterSecrets();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to load secrets: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void FilterSecrets()
        {
            if (_allSecrets == null)
                return;

            Secrets.Clear();
            var filteredSecrets = string.IsNullOrWhiteSpace(SearchText)
                ? _allSecrets
                : _allSecrets.FindAll(s => s.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

            foreach (var secret in filteredSecrets)
            {
                Secrets.Add(secret);
            }
        }

        private async Task AddSecretAsync()
        {
            await _navigation.PushAsync(new SecretDetailPage(null));
        }

        private async Task SelectSecretAsync(string secretName)
        {
            await _navigation.PushAsync(new SecretDetailPage(secretName));
        }

        private async Task LogoutAsync()
        {
            Application.Current.Resources.Remove("GopassClient");
            await _navigation.PopToRootAsync();
        }
    }
}
