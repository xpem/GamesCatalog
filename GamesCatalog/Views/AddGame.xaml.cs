using GamesCatalog.ViewModels;

namespace GamesCatalog.Views;

public partial class AddGame : ContentPage
{
	public AddGame(AddGameVM addGameVM)
	{
		InitializeComponent();

        BindingContext = addGameVM;
    }
}