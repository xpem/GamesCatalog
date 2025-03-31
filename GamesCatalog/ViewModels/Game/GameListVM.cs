using CommunityToolkit.Mvvm.Input;
using GamesCatalog.Models;
using Models.DTOs;
using Services;
using System.Collections.ObjectModel;

namespace GamesCatalog.ViewModels.Game
{
    public partial class GameListVM(IGameService gameService) : ViewModelBase, IQueryAttributable
    {
        private ObservableCollection<UIGame> games = [];

        public ObservableCollection<UIGame> Games
        {
            get => games;
            set => SetProperty(ref games, value);
        }

        private GameStatus GameStatus { get; set; }

        private string titleStatus = "";

        public string TitleStatus
        {
            get => titleStatus;
            set => SetProperty(ref titleStatus, value);
        }

        private int CurrentPage { get; set; }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query != null && query.TryGetValue("GameStatus", out object? outValue))
            {
                if (outValue is null) throw new ArgumentNullException("gameStatus");

                GameStatus = (GameStatus)Convert.ToInt32(outValue);

                TitleStatus = GameStatus switch
                {
                    GameStatus.Want => "Want to play",
                    GameStatus.Playing => "Playing",
                    GameStatus.Played => "Played",
                    _ => throw new ArgumentOutOfRangeException("gameStatus"),
                };
            }
            else throw new ArgumentNullException(nameof(query));

            CurrentPage = 1;

            _ = LoadGames();
        }

        private async Task LoadGames()
        {
            IsBusy = true;

            List<GameDTO> games = await gameService.GetByStatusAsync(null, GameStatus, CurrentPage);

            if (games.Count < 10) CurrentPage = -1;

            foreach (var game in games)
            {
                Games.Add(new UIGame
                {
                    Id = game.IGDBId.ToString() ?? throw new ArgumentNullException("IGDBId"),
                    LocalId = game.Id,
                    CoverUrl = game.CoverUrl ?? "",
                    Status = game.Status,
                    Rate = game.Status == GameStatus.Played ? game.Rate : 0,
                    Name = game.Name,
                    ReleaseDate = game.ReleaseDate ?? "",
                    Platforms = game.Platforms ?? "",
                    Summary = game.Summary ?? "",
                });
            }

            IsBusy = false;
        }

        [RelayCommand]
        public async Task LoadMore()
        {
            if (CurrentPage < 0) return;

            CurrentPage += 1;
            await LoadGames();
        }
    }
}
