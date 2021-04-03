using System.Linq;
using Akka.Actor;
using CoreWars.Common.TypedActorQuery.Ask;
using CoreWars.Competition;
using CoreWars.Coordination.Messages;

namespace CoreWars.Coordination.GameSlot
{
    public class CompetitionSlot : FSM<CompetitionSlotState, ICompetitionSlotFSMData>
    {
        public CompetitionSlot(
            IActorRef competitorSource
            , IActorRef competitionsResultHandler
            , ICompetitionActorFactory competitionActorFactory)
        {
            StartWith(CompetitionSlotState.Idle, Uninitialized.Instance);
            
            When(CompetitionSlotState.Idle, state =>
            {
                if (state.FsmEvent is RunCompetition runCompetition)
                    return GoToLobby(competitorSource);

                return null;
            });
            
            When(CompetitionSlotState.Lobby, state =>
            {
                if (state.FsmEvent is AgentsOrderCompleted agentsOrderCompleted)
                {
                    var competitionActor = competitionActorFactory.Build(agentsOrderCompleted.Agents, Context);
                    
                    return GoTo(CompetitionSlotState.Game)
                        .Using(
                            new ActiveGameData(
                                competitionActor
                                , agentsOrderCompleted.Agents.ToList()));
                }

                return null;
            });
            
            When(CompetitionSlotState.Game, state =>
            {
                if (state.FsmEvent is CompetitionResultMessage competitionResult)
                {
                    var concludedGameState = new ConcludedGameData(Sender, competitionResult);
                    return GoTo(CompetitionSlotState.Conclude).Using(concludedGameState);
                }

                return null;
            });
            
            When(CompetitionSlotState.Conclude, state =>
            {
                ConcludedGameData currentData = state.StateData as ConcludedGameData;
                ConcludedGameData nextStateData = null;
                
                if (state.FsmEvent is ResultAcknowledged)
                    nextStateData = currentData.WithResultAcknowledged;
                if (state.FsmEvent is LobbyGameTerminated)
                    nextStateData = currentData.WithGameTerminated;

                return nextStateData.FullyConcluded
                    ? GoToLobby(competitorSource) 
                    : Stay().Using(nextStateData);
            });
            
            OnTransition((initialState, nextState) =>
            {
                switch (initialState)
                {
                    case CompetitionSlotState.Lobby when StateData is QueryData queryStateData:
                        Context.Stop(queryStateData.Query);
                        break;
                    case CompetitionSlotState.Game when StateData is ActiveGameData activeGameData:
                        Context.Stop(activeGameData.Game);
                        break;
                }

                switch (nextState)
                {
                    case CompetitionSlotState.Conclude when NextStateData is ConcludedGameData concludedGameData:
                        competitionsResultHandler.Tell(concludedGameData.Result);
                        break;
                    case CompetitionSlotState.Game when NextStateData is ActiveGameData nextStateGameData:
                        Context.WatchWith(nextStateGameData.Game, new LobbyGameTerminated(nextStateGameData.Game));
                        nextStateGameData.Game.Tell(new Competition.Messages.RunCompetitionMessage());
                        break;
                }
            });
            
            
            Initialize();
        }

        public static Props Props(
            IActorRef competitorsSource
            , IActorRef resultHandler
            , ICompetitionActorFactory competitorsFactory)
        {
            return Akka.Actor.Props.Create(() => new CompetitionSlot(competitorsSource, resultHandler, competitorsFactory));
        }

        private State<CompetitionSlotState, ICompetitionSlotFSMData> GoToLobby(IActorRef competitorSource)
        {
            var query = competitorSource.AskFor<AgentsOrderCompleted>(OrderAgents.Instance, Context);

            return GoTo(CompetitionSlotState.Lobby).Using(new QueryData(query));
        }
        
    }
}