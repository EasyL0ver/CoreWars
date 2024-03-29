using Akka.Actor;
using CoreWars.Common;
using CoreWars.Scripting;

namespace CoreWars.WebApp
{
    public interface IActorSystemService
    {
        ActorSystem ActorSystem { get; }
    }

    public interface IGameService : IActorSystemService
    {
        IActorRef ResultsHandler { get; }
        IActorRef ScriptRepository { get; }
        IActorRef NotificationProvider { get; }
        IActorRef CompetitorsRoot { get; }
        
        ICompetitorFactory CompetitorFactory { get; }
    }

}