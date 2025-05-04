using GamesCatalog.ViewModels;

namespace GamesCatalog.Views;

public partial class FirstSyncProcess : ContentPage
{
	public FirstSyncProcess(FirstSyncProcessVM firstSyncProcessVM)
	{
		InitializeComponent();
        BindingContext = firstSyncProcessVM;
    }
}