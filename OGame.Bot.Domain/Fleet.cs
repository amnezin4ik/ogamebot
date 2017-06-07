using System.Collections.Generic;

namespace OGame.Bot.Domain
{
    public class Fleet
    {
        public IEnumerable<ShipCell> ShipCells { get; set; }
    }
}