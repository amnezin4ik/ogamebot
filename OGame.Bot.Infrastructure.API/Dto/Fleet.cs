using System.Collections.Generic;

namespace OGame.Bot.Infrastructure.API.Dto
{
    public class Fleet
    {
        public IEnumerable<ShipCell> Ships { get; set; } 
    }
}
