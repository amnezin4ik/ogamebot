namespace OGame.Bot.Domain
{
    public class ShipCell
    {
        public Ship Ship { get; set; }

        public int Count { get; set; }

        public override string ToString()
        {
            return $"{Ship} ({Count})";
        }
    }
}