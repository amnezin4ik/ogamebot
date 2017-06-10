using System;

namespace OGame.Bot.Domain
{
    public class Ship
    {
        public ShipType ShipType { get; set; }

        //TODO: Parse these data from OGame and store in memory.
        public int GetCapacity()
        {
            int capacity;
            switch (ShipType)
            {
                case ShipType.SmallTransport:
                    capacity = 5000;
                    break;
                case ShipType.LargeTransport:
                    capacity = 25000;
                    break;
                case ShipType.LightFighter:
                    capacity = 50;
                    break;
                case ShipType.HeavyFighter:
                    capacity = 100;
                    break;
                case ShipType.Cruiser:
                    capacity = 800;
                    break;
                case ShipType.Battleship:
                    capacity = 1500;
                    break;
                case ShipType.Colonizer:
                    capacity = 7500;
                    break;
                case ShipType.Recycler:
                    capacity = 20000;
                    break;
                case ShipType.SpyExplorer:
                    capacity = 0;
                    break;
                case ShipType.Bomber:
                    capacity = 500;
                    break;
                case ShipType.SolarSatellite:
                    capacity = 0;
                    break;
                case ShipType.Exterminator:
                    capacity = 2000;
                    break;
                case ShipType.DeathStar:
                    capacity = 1000000;
                    break;
                case ShipType.BattleCruiser:
                    capacity = 750;
                    break;
                default:
                    throw new NotSupportedException($"Can't get capacity for {ShipType} ship type");
            }
            return capacity;
        }

        public override string ToString()
        {
            return ShipType.ToString();
        }
    }
}