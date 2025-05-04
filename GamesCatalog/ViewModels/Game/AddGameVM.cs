using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Input;
using GamesCatalog.Models.IGDBApi;
using Models.DTOs;
using Services;
using Services.Interfaces;

namespace GamesCatalog.ViewModels
{
    public partial class AddGameVM(IGameService gameService) : ViewModelBase, IQueryAttributable
    {
        private UIIGDBGame Game { get; set; }

        public string IgdbId = "", name = "", releaseDate = "", coverUrl = "", platforms = "", summary = "";
        public int? Id;

        private bool confirmIsVisible = false;

        static readonly Color BgButtonStatusSelectedColor = Color.FromArgb("#2b9b74");
        static readonly Color BgButtonStatusNormalColor = Color.FromArgb("#2B659B");

        private GameStatus? GameSelectedStatus = null;

        private Color wantBgColor = BgButtonStatusNormalColor, playingBgColor = BgButtonStatusNormalColor, playedBgColor = BgButtonStatusNormalColor;

        public Color WantBgColor
        {
            get => wantBgColor;
            set => SetProperty(ref wantBgColor, value);
        }

        public Color PlayingBgColor
        {
            get => playingBgColor;
            set => SetProperty(ref playingBgColor, value);
        }

        public Color PlayedBgColor
        {
            get => playedBgColor;
            set => SetProperty(ref playedBgColor, value);
        }

        public bool ConfirmIsVisible
        {
            get => confirmIsVisible;
            set => SetProperty(ref confirmIsVisible, value);
        }

        private bool confirmIsEnabled = true;

        public bool ConfirmIsEnabled
        {
            get => confirmIsEnabled;
            set => SetProperty(ref confirmIsEnabled, value);
        }

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public string ReleaseDate
        {
            get => releaseDate;
            set => SetProperty(ref releaseDate, value);
        }

        public string CoverUrl
        {
            get => coverUrl;
            set => SetProperty(ref coverUrl, value);
        }

        public string Platforms
        {
            get => platforms;
            set => SetProperty(ref platforms, value);
        }

        public string Summary
        {
            get => summary;
            set => SetProperty(ref summary, value);
        }

        private int? rate = 0;

        public int? Rate
        {
            get => rate;
            set
            {
                if (RatingBarIsVisible)
                    ConfirmIsVisible = true;

                SetProperty(ref rate, value);
            }
        }

        private bool ratingBarIsVisible = false;

        public bool RatingBarIsVisible
        {
            get => ratingBarIsVisible;
            set => SetProperty(ref ratingBarIsVisible, value);
        }

        private bool expanderIsVisible = false;

        public bool ExpanderIsVisible
        {
            get => expanderIsVisible;
            set => SetProperty(ref expanderIsVisible, value);
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query is not null && query.TryGetValue("Game", out object? game))
            {
                if (game is not null and UIIGDBGame uiGame)
                {
                    Game = uiGame;

                    IgdbId = Game.Id;
                    Name = Game.Name;
                    ReleaseDate = Game.ReleaseDate;
                    CoverUrl = Path.Combine(GameService.ImagesPath, $"{Game.Id}.jpg");
                    Platforms = Game.Platforms;
                    Summary = Game.Summary;

                    _ = BuildGameStatus();
                }
            }
        }

        public async Task BuildGameStatus()
        {
            var gameDTO = await gameService.GetByIGDBIdAsync(int.Parse(IgdbId), ((App)Application.Current).Uid.Value);

            if (gameDTO is null) return;

            Id = gameDTO.Id;

            if (!gameDTO.Inactive)
            {
                if (Id is not null)
                    ExpanderIsVisible = true;

                switch (gameDTO.Status)
                {
                    case GameStatus.Want: _ = Want(); break;
                    case GameStatus.Playing: _ = Playing(); break;
                    case GameStatus.Played:
                        _ = Played();
                        Rate = gameDTO.Rate;
                        break;
                }
            }
        }

        [RelayCommand]
        private Task Want()
        {
            GameSelectedStatus = GameStatus.Want;
            WantBgColor = BgButtonStatusSelectedColor;
            PlayingBgColor = BgButtonStatusNormalColor;
            PlayedBgColor = BgButtonStatusNormalColor;
            ConfirmIsVisible = true;
            RatingBarIsVisible = false;
            Rate = 0;

            return Task.CompletedTask;
        }

        [RelayCommand]
        private Task Playing()
        {
            GameSelectedStatus = GameStatus.Playing;
            WantBgColor = BgButtonStatusNormalColor;
            PlayingBgColor = BgButtonStatusSelectedColor;
            PlayedBgColor = BgButtonStatusNormalColor;
            ConfirmIsVisible = true;
            RatingBarIsVisible = false;
            Rate = 0;

            return Task.CompletedTask;
        }

        [RelayCommand]
        private Task Played()
        {
            GameSelectedStatus = GameStatus.Played;
            WantBgColor = BgButtonStatusNormalColor;
            PlayingBgColor = BgButtonStatusNormalColor;
            PlayedBgColor = BgButtonStatusSelectedColor;
            ConfirmIsVisible = false;
            Rate = 0;
            RatingBarIsVisible = true;

            return Task.CompletedTask;
        }

        [RelayCommand]
        public async Task Confirm()
        {
            // Disable button to avoid multiple requests
            ConfirmIsEnabled = false;

            int? _rate = null;
            string displayMessage;

            // If the game is played, the rate is required
            if (GameSelectedStatus == GameStatus.Played)
                _rate = Rate;

            if (Id is null)
            {
                if (IsOn && CoverUrl is not null)
                    await IGDBGamesApiService.SaveImageAsync(CoverUrl, $"{IgdbId}.jpg");

                GameDTO game = new()
                {
                    IGDBId = int.Parse(IgdbId),
                    Name = Name,
                    ReleaseDate = ReleaseDate,
                    CoverUrl = CoverUrl,
                    Platforms = Platforms,
                    Summary = Summary,
                    Status = GameSelectedStatus.Value,
                    Rate = _rate,
                    UserId = ((App)Application.Current).Uid.Value,
                };

                await gameService.CreateAsync(game, IsOn);

                displayMessage = "Status Added!";
            }
            else
            {
                await gameService.UpdateStatusAsync(Id.Value, GameSelectedStatus.Value, _rate, IsOn);

                displayMessage = "Status Updated!";
            }

            if (DeviceInfo.Platform == DevicePlatform.iOS || DeviceInfo.Platform == DevicePlatform.Android)
            {
                ToastDuration duration = ToastDuration.Short;

                var toast = Toast.Make(displayMessage, duration, 15);
                await toast.Show();
            }
            else
                await Application.Current.Windows[0].Page.DisplayAlert("Success", displayMessage, null, "Ok");

            await Shell.Current.GoToAsync("../..");

            ConfirmIsEnabled = true;
        }

        [RelayCommand]
        public async Task Inactivate()
        {
            if (await Application.Current.Windows[0].Page.DisplayAlert("Confirm", "Remove from list?", "Yes", "Cancel"))
            {
                _ = gameService.InactivateAsync(((App)Application.Current).Uid.Value, Id.Value, IsOn);

                if (DeviceInfo.Platform == DevicePlatform.iOS || DeviceInfo.Platform == DevicePlatform.Android)
                {
                    ToastDuration duration = ToastDuration.Short;

                    var toast = Toast.Make("Game removed", duration, 15);
                    await toast.Show();
                }
                else
                    await Application.Current.Windows[0].Page.DisplayAlert("Success", "Game removed", null, "Ok");

                await Shell.Current.GoToAsync("..");
            }
        }
    }
}