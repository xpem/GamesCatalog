using GamesCatalog.ViewModels;

namespace GamesCatalog.Views.Game;

public partial class AddGame : ContentPage
{
	public AddGame(AddGameVM addGameVM)
	{
		InitializeComponent();

        BindingContext = addGameVM;
    }
}