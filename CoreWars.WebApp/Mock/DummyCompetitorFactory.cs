using Akka.Actor;
using CoreWars.Player;

namespace CoreWars.WebApp.Mock
{
    public class DummyCompetitorFactory : IPlayerAgentActorFactory
    {
        public IActorRef Build(IActorContext context)
        {
            return context.ActorOf(Props.Create<DummyCompetitor>());
        }
    }
}