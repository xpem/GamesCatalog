using CommunityToolkit.Mvvm.ComponentModel;
using GamesCatalog.Models.IGDBApi;
using System.Collections.ObjectModel;

namespace GamesCatalog.ViewModels.IGDBSearch
{
    public partial class IGDBResultsVM : ObservableObject
    {

        //list with a mock of games
        private ObservableCollection<UIIGDBGame> listGames = [
            new UIIGDBGame
            {
                Id = "1",
                Name = "Game 1",
                ReleaseDate = "01/01/2024",
                CoverUrl = "https://images.igdb.com/igdb/image/upload/t_cover_big/co2ed3.jpg",
                Platforms = "PC, PS4, PS5, Xbox One, Xbox Series X/S"
            },
            new UIIGDBGame
            {
                Id = "2",
                Name = "Game 2",
                ReleaseDate = "01/02/2024",
                CoverUrl = "https://images.igdb.com/igdb/image/upload/t_cover_big/co2ed3.jpg",
                Platforms = "PC, PS4, PS5, Xbox One, Xbox Series X/S"
            },
            new UIIGDBGame
            {
                Id = "3",
                Name = "Game 3",
                ReleaseDate = "01/03/2024",
                CoverUrl = "https://images.igdb.com/igdb/image/upload/t_cover_big/co2ed3.jpg",
                Platforms = "PC, PS4, PS5, Xbox One, Xbox Series X/S"
            }
            ];
                
        public ObservableCollection<UIIGDBGame> ListGames
        {
            get => listGames;
            set => SetProperty(ref listGames, value);
        }
    }
}
