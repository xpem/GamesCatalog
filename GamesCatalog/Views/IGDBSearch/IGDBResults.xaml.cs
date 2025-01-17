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
}