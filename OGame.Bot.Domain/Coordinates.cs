namespace OGame.Bot.Domain
{
    public class Coordinates
    {
        public Coordinates(int galaxy, int system, int position)
        {
            Galaxy = galaxy;
            System = system;
            Position = position;
        }

        public int Galaxy { get; }

        public int System { get; }

        public int Position { get; }

        public static bool operator == (Coordinates a, Coordinates b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }
            return a.Galaxy == b.Galaxy && a.System == b.System && a.Position == b.Position;
        }

        public static bool operator != (Coordinates a, Coordinates b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Coordinates)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Galaxy;
                hashCode = (hashCode * 397) ^ System;
                hashCode = (hashCode * 397) ^ Position;
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"[{Galaxy}:{System}:{Position}]";
        }

        protected bool Equals(Coordinates other)
        {
            return Galaxy == other.Galaxy && System == other.System && Position == other.Position;
        }
    }
}