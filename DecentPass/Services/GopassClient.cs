using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Gopass;

namespace DecentPass.Services
{
    public class GopassClient
    {
        private readonly GrpcChannel _channel;
        private readonly GopassService.GopassServiceClient _client;
        private bool _isAuthenticated = false;

        public GopassClient(string serverUrl)
        {
            _channel = GrpcChannel.ForAddress(serverUrl);
            _client = new GopassService.GopassServiceClient(_channel);
        }

        public async Task<bool> AuthenticateAsync(string passphrase)
        {
            try
            {
                var request = new AuthRequest { Passphrase = passphrase };
                var response = await _client.AuthenticateAsync(request);
                _isAuthenticated = response.Status == "authenticated";
                return _isAuthenticated;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Authentication error: {ex.Message}");
                return false;
            }
        }

        public async Task<List<string>> ListSecretsAsync()
        {
            if (!_isAuthenticated)
                throw new UnauthorizedAccessException("Authentication required");

            try
            {
                var request = new ListRequest();
                var response = await _client.ListSecretsAsync(request);
                return new List<string>(response.Secrets);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"List secrets error: {ex.Message}");
                throw;
            }
        }

        public async Task<string> GetSecretAsync(string name, string revision = "")
        {
            if (!_isAuthenticated)
                throw new UnauthorizedAccessException("Authentication required");

            try
            {
                var request = new GetRequest { Name = name, Revision = revision };
                var response = await _client.GetSecretAsync(request);
                return response.Secret;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get secret error: {ex.Message}");
                throw;
            }
        }

        public async Task SetSecretAsync(string name, string secret)
        {
            if (!_isAuthenticated)
                throw new UnauthorizedAccessException("Authentication required");

            try
            {
                var request = new SetRequest { Name = name, Secret = secret };
                await _client.SetSecretAsync(request);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Set secret error: {ex.Message}");
                throw;
            }
        }

        public async Task RemoveSecretAsync(string name)
        {
            if (!_isAuthenticated)
                throw new UnauthorizedAccessException("Authentication required");

            try
            {
                var request = new RemoveRequest { Name = name };
                await _client.RemoveSecretAsync(request);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Remove secret error: {ex.Message}");
                throw;
            }
        }

        public async Task RemoveAllSecretsWithPrefixAsync(string prefix)
        {
            if (!_isAuthenticated)
                throw new UnauthorizedAccessException("Authentication required");

            try
            {
                var request = new RemoveAllRequest { Prefix = prefix };
                await _client.RemoveAllSecretsWithPrefixAsync(request);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Remove all secrets error: {ex.Message}");
                throw;
            }
        }

        public async Task RenameSecretAsync(string src, string dest)
        {
            if (!_isAuthenticated)
                throw new UnauthorizedAccessException("Authentication required");

            try
            {
                var request = new RenameRequest { Src = src, Dest = dest };
                await _client.RenameSecretAsync(request);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Rename secret error: {ex.Message}");
                throw;
            }
        }

        public void Dispose()
        {
            _channel?.Dispose();
        }
    }
}