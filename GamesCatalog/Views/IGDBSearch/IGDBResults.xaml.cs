using GamesCatalog.Models.IGDBApi;
using GamesCatalog.ViewModels;
using GamesCatalog.ViewModels.IGDBSearch;

namespace GamesCatalog.Views.IGDBSearch;

public partial class IGDBResults : ContentPage
{
    public IGDBResults(IGDBResultsVM iGDBResultsVM)
    {
        InitializeComponent();

        base.BindingContext = iGDBResultsVM;
    }

    private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is UIIGDBGame tappedItem)
            Shell.Current.GoToAsync($"{nameof(AddGame)}", true, new Dictionary<string, object>
            {
                { "Game", tappedItem }
            });
    }
}