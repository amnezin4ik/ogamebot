using OGame.Bot.Infrastructure.API.Dto;

namespace OGame.Bot.Infrastructure.API.Helpers
{
    public class CoordinatesParser
    {
        public Coordinates ParseCoordinatesFromString(string coordinatesString)
        {
            Coordinates coordinates = null;
            if (!string.IsNullOrWhiteSpace(coordinatesString))
            {
                var coordinatesArray = coordinatesString.Trim().TrimStart('[').TrimEnd(']').Split(':');
                if (coordinatesArray.Length == 3)
                {
                    int galaxy, system, position;
                    if (int.TryParse(coordinatesArray[0], out galaxy) &&
                        int.TryParse(coordinatesArray[1], out system) &&
                        int.TryParse(coordinatesArray[2], out position))
                    {
                        coordinates = new Coordinates
                        {
                            Galaxy = galaxy,
                            System = system,
                            Position = position
                        };
                    }
                }
            }
            return coordinates;
        }
    }
}