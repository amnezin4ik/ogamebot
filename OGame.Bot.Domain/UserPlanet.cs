namespace OGame.Bot.Domain
{
    public class UserPlanet
    {
        public string PlanetId { get; set; }

        public string Name { get; set; }

        public Coordinates Coordinates { get; set; }

        public override string ToString()
        {
            return $"{Name}-{PlanetId}{Coordinates}";
        }
    }
}