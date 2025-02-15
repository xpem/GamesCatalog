using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GamesCatalog.Models.IGDBApi;
using Models.Resps;
using Services.Games;
using System.Collections.ObjectModel;

namespace GamesCatalog.ViewModels.IGDBSearch
{
    public partial class IGDBResultsVM : ObservableObject
    {
        private ObservableCollection<UIIGDBGame> listGames = [];

        private bool Searching { get; set; }

        string searchText;

        public string SearchText
        {
            get => searchText;
            set
            {
                if (searchText != value)
                {
                    SetProperty(ref (searchText), value);
                    _ = SearchGamesList();
                }
            }
        }

        public ObservableCollection<UIIGDBGame> ListGames
        {
            get => listGames;
            set => SetProperty(ref listGames, value);
        }


        private async Task SearchGamesList()
        {
            if (Searching)
                return;

            if (SearchText.Length < 3) return;

            Searching = true;

            while (Searching)
            {
                await Task.Delay(2500);

                if (ListGames.Count > 0)
                    ListGames.Clear();

                try
                {
                    await LoadIGDBGamesList(0);
                }
                catch (Exception e)
                {
                    throw;
                }

                Searching = false;
            }
        }

        private async Task LoadIGDBGamesList(int startIndex)
        {
            List<IGDBGame> resp = await IGDBGamesApiService.Get(SearchText, startIndex);

            DateTime? releaseDate = null;

            foreach (var item in resp)
            {
                if (item is null) continue;

                if(item.first_release_date is not null)
                    releaseDate = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(item.first_release_date)).UtcDateTime;
                else
                    releaseDate = null;

                ListGames.Add(new UIIGDBGame
                {
                    Id = item.id,
                    Name = item.name ?? "",
                    ReleaseDate = releaseDate?.Date.ToString("MM/yyyy") ?? "",
                    CoverUrl = item.cover?.id is not null ? $"https://images.igdb.com/igdb/image/upload/t_cover_big/{item.cover?.image_id}.jpg" : "",
                    Platforms = item.platforms?.Count > 0 ? string.Join(", ", item.platforms.Select(p => p.abbreviation)) : ""
                });
            }
        }
    }
}
