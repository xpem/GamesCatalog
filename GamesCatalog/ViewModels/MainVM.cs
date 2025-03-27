using CommunityToolkit.Mvvm.Input;
using GamesCatalog.Views;
using GamesCatalog.Views.IGDBSearch;
using Microsoft.Maui;
using Microsoft.Maui.Storage;
using Services;
using System.Collections.ObjectModel;

namespace GamesCatalog.ViewModels;

public partial class MainVM : ViewModelBase
{
   static string filePath = Path.Combine(GameService.ImagesPath, "201156.jpg");

    public ObservableCollection<ImageT2> Imagens2 { get; set; } = new ObservableCollection<ImageT2>
{
 new ImageT2(filePath,new Thickness(0)),
 new ImageT2(filePath,new Thickness(-45,0,0,0)),
 new ImageT2(filePath, new Thickness(-45,0,0,0)),
 new ImageT2(filePath,new Thickness(-45,0,0,0)),
 new ImageT2(filePath, new Thickness(-45,0,0,0))
};

    private bool btnAddGameIsEnabled = true;

    public bool BtnAddGameIsEnabled { get => btnAddGameIsEnabled; set { if (value != btnAddGameIsEnabled) { SetProperty(ref (btnAddGameIsEnabled), value); } } }

    private Color isConnectedColor = Colors.Green;

    public Color IsConnectedColor { get => isConnectedColor; set { if (value != isConnectedColor) { SetProperty(ref (isConnectedColor), value); } } }

    [RelayCommand]
    private Task GoToIGDBResults() => Shell.Current.GoToAsync($"{nameof(IGDBResults)}");

    [RelayCommand]
    private async Task Appearing()
    {
        if (!(Connectivity.NetworkAccess == NetworkAccess.Internet))
        {
            IsConnectedColor = Colors.Red;
            BtnAddGameIsEnabled = false;
        }
        else
        {
            IsConnectedColor = Colors.Green;
            BtnAddGameIsEnabled = true;
        }
    }
}

public record ImageT2(string url, Thickness margin);
