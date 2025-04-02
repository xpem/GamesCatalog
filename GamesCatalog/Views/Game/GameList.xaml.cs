using GamesCatalog.Models;
using GamesCatalog.ViewModels.Game;

namespace GamesCatalog.Views.Game;

public partial class GameList : ContentPage
{
	public GameList(GameListVM gameListVM)
	{
		InitializeComponent();

        BindingContext = gameListVM;
    }

    private void GamesLstVw_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is UIGame tappedItem)
            Shell.Current.GoToAsync($"{nameof(AddGame)}", true, new Dictionary<string, object>
            {
                { "Game", tappedItem }
            });
    }
}