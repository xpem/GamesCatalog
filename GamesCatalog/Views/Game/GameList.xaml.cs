using GamesCatalog.ViewModels.Game;

namespace GamesCatalog.Views.Game;

public partial class GameList : ContentPage
{
	public GameList(GameListVM gameListVM)
	{
		InitializeComponent();

        BindingContext = gameListVM;
    }
}