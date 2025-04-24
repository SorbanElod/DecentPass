using DecentPass.ViewModels;

namespace DecentPass.Views;

public partial class SecretsListPage : ContentPage
{
	public SecretsListPage()
	{
		InitializeComponent();
		BindingContext = new SecretsListViewModel(Navigation);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        if(BindingContext is SecretsListViewModel viewModel)
        {
            viewModel.RefreshCommand.Execute(null);
        }
    }
}