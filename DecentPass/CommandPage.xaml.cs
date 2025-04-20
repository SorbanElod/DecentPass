using Gopass;
using Grpc.Net.Client;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;
namespace DecentPass;
public partial class CommandPage : ContentPage
{
    private readonly GopassService.GopassServiceClient _client;
    public CommandPage()
    {
        InitializeComponent();
        // connect to local grpc server
        var channel = GrpcChannel.ForAddress("http://localhost:50051");
        _client = new GopassService.GopassServiceClient(channel);
    }
    private async void OnCommandEntered(object sender, EventArgs e)
    {
        string command = CommandEntry.Text.Trim();
        if (string.IsNullOrEmpty(command))
        {
            return;
        }
        AppendOutput($"Command: {command}");
        CommandEntry.Text = string.Empty;
        try
        {
            var output = await RunCommand(command);
            AppendOutput(output);
        }
        catch (Exception ex)
        {
            AppendOutput($"Error: {ex.Message}");
        }
    }
    private void AppendOutput(string output)
    {
        OutputLabel.Text += $"{output}\n";
    }
    private async Task<string> RunCommand(string input)
    {
        var args = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (args.Length == 0)
        {
            return "No command provided.";
        }
        var request = new CommandRequest
        {
            TimeoutSeconds = 10, // adjust as needed
            WorkingDir = args[0], // optional, you can set default path
        };
        request.Args.AddRange(args);

        var response = await _client.ExecuteCommandAsync(request);
        string result = string.Empty;
        if (!string.IsNullOrWhiteSpace(response.Stdout))
        {
            result += $"Output: {response.Stdout}\n";
        }
        if (!string.IsNullOrWhiteSpace(response.Stderr))
        {
            result += $"Error: {response.Stderr}\n";
        }
        result += $"Exit Code: {response.ExitCode} - Success: {response.Success}\n";
        if (!string.IsNullOrWhiteSpace(response.ErrorMessage))
        {
            result += $"Error: {response.ErrorMessage}\n";
        }
        return result;
    }
}