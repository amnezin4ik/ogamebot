namespace OGame.Bot.Domain.Messages
{
    public class MessagesComparer : IMessagesComparer
    {
        public bool Equals(Message x, Message y)
        {
            var hashX = x.GetHashCode();
            var hashY = y.GetHashCode();
            return hashX == hashY;
        }

        public int GetHashCode(Message obj)
        {
            return obj.GetHashCode();
        }
    }
}