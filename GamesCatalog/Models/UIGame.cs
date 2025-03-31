using GamesCatalog.Models.IGDBApi;
using Models.DTOs;

namespace GamesCatalog.Models
{
    public class UIGame : UIIGDBGame
    {
        public int LocalId { get; set; }

        public GameStatus Status { get; set; }

        public int? Rate { get; set; }

        public bool RateIsVisible => Rate > 0;
    }
}
