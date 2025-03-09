using CommunityToolkit.Mvvm.Input;
using GamesCatalog.Models.IGDBApi;

namespace GamesCatalog.ViewModels
{
    public partial class AddGameVM : ViewModelBase, IQueryAttributable
    {
        private UIIGDBGame Game { get; set; }

        public string Id = "", name = "", releaseDate = "", coverUrl = "", platforms = "", summary = "";

        private bool confirmIsVisible = false;

        static readonly Color BgButtonStatusSelectedColor = Color.FromArgb("#2b9b74");
        static readonly Color BgButtonStatusNormalColor = Color.FromArgb("#2B659B");

        private enum GameStatus
        {
            Want,
            Playing,
            Played
        }

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

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query is not null)
            {
                if (query.TryGetValue("Game", out object? game))
                {
                    if (game is not null and UIIGDBGame uiGame)
                    {
                        Game = uiGame;

                        Id = Game.Id;
                        Name = Game.Name;
                        ReleaseDate = Game.ReleaseDate;
                        CoverUrl = Game.CoverUrl;
                        Platforms = Game.Platforms;
                        Summary = Game.Summary;
                    }
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

    }
}
