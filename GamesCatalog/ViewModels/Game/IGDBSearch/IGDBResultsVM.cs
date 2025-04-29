using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GamesCatalog.Models.IGDBApi;
using Models.Resps;
using Services;
using System.Collections.ObjectModel;

namespace GamesCatalog.ViewModels.IGDBSearch
{
    public partial class IGDBResultsVM(IIGDBGamesApiService iGDBGamesApiService) : ViewModelBase
    {
        private ObservableCollection<UIIGDBGame> listGames = [];

        //private bool Searching { get; set; }

        string searchText = "";

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

        public int CurrentIndex { get; set; }

        public ObservableCollection<UIIGDBGame> ListGames
        {
            get => listGames;
            set => SetProperty(ref listGames, value);
        }

        private readonly SemaphoreSlim searchSemaphore = new(1, 1);
        private CancellationTokenSource? searchDelayTokenSource;

        private async Task SearchGamesList()
        {
            if (SearchText.Length < 3)
                return;

            searchDelayTokenSource?.Cancel();
            searchDelayTokenSource = new CancellationTokenSource();
            var token = searchDelayTokenSource.Token;

            await Task.Delay(1500, token);

            if (!token.IsCancellationRequested) // Only run if it has not been canceled / Só executa se não foi cancelado
            {
                if (ListGames.Count > 0)
                    ListGames.Clear();

                CurrentIndex = 0;
                await LoadIGDBGamesList(CurrentIndex);
            }
        }

        [RelayCommand]
        public async Task LoadMore()
        {
            if (CurrentIndex < 0) return;

            CurrentIndex += 10;
            await LoadIGDBGamesList(CurrentIndex);
        }


        private async Task LoadIGDBGamesList(int startIndex)
        {
            IsBusy = true;

            await searchSemaphore.WaitAsync();

            try
            {
                List<IGDBGame> resp = await iGDBGamesApiService.GetAsync(SearchText, startIndex);

                DateTime? releaseDate = null;

                if (resp.Count < 10)
                {
                    CurrentIndex = -10;
                }

                foreach (var item in resp)
                {
                    if (item is null) continue;

                    if (item.first_release_date is not null)
                        releaseDate = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(item.first_release_date)).UtcDateTime;
                    else
                        releaseDate = null;

                    ListGames.Add(new UIIGDBGame
                    {
                        Id = item.id,
                        Name = item.name ?? "",
                        ReleaseDate = releaseDate?.Date.ToString("MM/yyyy") ?? "",
                        CoverUrl = item.cover?.image_id is not null ? $"https://images.igdb.com/igdb/image/upload/t_cover_big/{item.cover?.image_id}.jpg" : "",
                        Platforms = item.platforms?.Count > 0 ? string.Join(", ", item.platforms.Select(p => p.abbreviation)) : "",
                        Summary = item.summary ?? "",
                    });
                }
            }
            finally
            {
                searchSemaphore.Release();
            }

            IsBusy = false;
        }
    }
}
