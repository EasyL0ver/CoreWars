using Akka.Actor;

namespace CoreWars.Player.Messages
{
    public class CompetitorFactoryUpdated
    {
        public CompetitorFactoryUpdated(Props newFactory)
        {
            NewFactory = newFactory;
        }

        public Props NewFactory { get; }
    }
}