namespace OGame.Bot.Domain
{
    public class Resources
    {
        public int Metal { get; set; }

        public int Crystal { get; set; }

        public int Deuterium { get; set; }

        public override string ToString()
        {
            return $"m: {Metal}, c:{Crystal}, d:{Deuterium}";
        }
    }
}