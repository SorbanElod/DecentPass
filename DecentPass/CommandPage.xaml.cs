using Grpc.Net.Client;

namespace DecentPass;

public partial class CommandPage : ContentPage
{
    private GopassService.GopassServiceClient _client;
    public CommandPage()
	{
		InitializeComponent();

        // connect to local grpc server
        var channel = GrpcChannel.ForAddress("http://localhost:50051");
        _client = new GopassService.GopassServiceClient(channel);
    }

    private void OnCommandEntered(object sender, EventArgs e)
    {

    }
}