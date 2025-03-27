using GamesCatalog.ViewModels;

namespace GamesCatalog.Views;

public partial class Main : ContentPage
{
	public Main(MainVM mainVM)
	{
		InitializeComponent();
        BindingContext = mainVM;
    }
}