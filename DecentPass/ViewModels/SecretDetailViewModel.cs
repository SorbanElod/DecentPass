using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DecentPass.Services;
using DecentPass.Views;
using Gopass;

namespace DecentPass.ViewModels
{
    public class SecretDetailViewModel : BaseViewModel
    {
        private readonly GopassClient _gopassClient;
        private readonly INavigation _navigation;
        private string _secretName;
        private string _secretValue;
        private string _oldSecretName;
        private bool _isNewSecret;
        private bool _showPassword = false;

        public string SecretName
        {
            get => _secretName;
            set => SetProperty(ref _secretName, value);
        }

        public string SecretValue
        {
            get => _secretValue;
            set => SetProperty(ref _secretValue, value);
        }

        public bool ShowPassword
        {
            get => _showPassword;
            set => SetProperty(ref _showPassword, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand TogglePasswordVisibilityCommand { get; }
        public ICommand CopyToClipboardCommand { get; }

        public SecretDetailViewModel(INavigation navigation, string secretName)
        {
            _navigation = navigation;
            _gopassClient = Application.Current.Resources["GopassClient"] as GopassClient;
            _isNewSecret = string.IsNullOrEmpty(secretName);
            _oldSecretName = secretName ?? string.Empty;

            SecretName = secretName ?? string.Empty;
            Title = _isNewSecret ? "New Password" : "Password Details";
            SaveCommand = null;
            if(!_isNewSecret)
            {
                SaveCommand = new Command(async () => await UpdateSecret());
            }
            else
            {
                SaveCommand = new Command(async () => await SaveSecretAsync());
            }
            
            DeleteCommand = new Command(async () => await DeleteSecretAsync());
            TogglePasswordVisibilityCommand = new Command(() => ShowPassword = !ShowPassword);
            CopyToClipboardCommand = new Command(async () => await CopyToClipboardAsync());

            // Load secret if editing an existing one
            if (!_isNewSecret)
            {
                Task.Run(async () => await LoadSecretAsync());
            }
        }

        private async Task LoadSecretAsync()
        {
            if (IsBusy || string.IsNullOrEmpty(SecretName))
                return;

            IsBusy = true;

            try
            {
                SecretValue = await _gopassClient.GetSecretAsync(SecretName);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to load secret: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SaveSecretAsync()
        {
            if (IsBusy)
                return;

            if (string.IsNullOrWhiteSpace(SecretName))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Secret name cannot be empty", "OK");
                return;
            }

            IsBusy = true;

            try
            {
                await _gopassClient.SetSecretAsync(SecretName, SecretValue ?? string.Empty);
                await _navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to save secret: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task DeleteSecretAsync()
        {
            if (IsBusy || _isNewSecret)
                return;

            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Confirm Delete",
                $"Are you sure you want to delete '{SecretName}'?",
                "Delete",
                "Cancel");

            if (!confirm)
                return;

            IsBusy = true;

            try
            {
                await _gopassClient.RemoveSecretAsync(SecretName);
                await _navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to delete secret: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task RenameSecret()
        {
            if (IsBusy || _isNewSecret)
                return;
            IsBusy = true;
            try
            {
                await _gopassClient.RenameSecretAsync(_oldSecretName, SecretName);
                await _navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to rename secret: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task UpdateSecret()
        {
            if (IsBusy || _isNewSecret)
                return;
            IsBusy = true;
            try
            {
                await _gopassClient.RenameSecretAsync(_oldSecretName, SecretName);
                await _gopassClient.SetSecretAsync(SecretName, SecretValue);
                await _navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to update secret: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task CopyToClipboardAsync()
        {
            try
            {
                await Clipboard.SetTextAsync(SecretValue);
                await Application.Current.MainPage.DisplayAlert("Success", "Password copied to clipboard", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to copy to clipboard: {ex.Message}", "OK");
            }
        }
    }
}
