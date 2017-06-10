namespace OGame.Bot.Domain
{
    public class PlanetOverview
    {
        public UserPlanet UserPlanet { get; set; }

        public Resources Resources { get; set; }

        public int Energy { get; set; }

        public override string ToString()
        {
            return $"{UserPlanet} {Resources}, energy: {Energy}";
        }
    }
}