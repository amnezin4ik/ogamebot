using System.Collections.Generic;
using System.Text;

namespace OGame.Bot.Domain
{
    public class Fleet
    {
        public IEnumerable<ShipCell> ShipCells { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("Ships: ");
            foreach (var shipCell in ShipCells)
            {
                sb.Append($"{shipCell}; ");
            }
            return sb.ToString();
        }
    }
}