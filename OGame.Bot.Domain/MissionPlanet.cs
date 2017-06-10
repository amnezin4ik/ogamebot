namespace OGame.Bot.Domain
{
    public class MissionPlanet
    {
        public Coordinates Coordinates { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return $"{Name} {Coordinates}";
        }
    }
}