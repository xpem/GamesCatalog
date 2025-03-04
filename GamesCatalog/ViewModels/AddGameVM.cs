using GamesCatalog.Models.IGDBApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace GamesCatalog.ViewModels
{
    public partial class AddGameVM : ViewModelBase, IQueryAttributable
    {
        private UIIGDBGame Game { get; set; }

        public string Id = "", name = "", releaseDate = "", coverUrl = "", platforms = "", summary = "";      

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

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
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
    }
}
