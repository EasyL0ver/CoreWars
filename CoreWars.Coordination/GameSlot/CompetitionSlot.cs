using System;
using System.Linq;
using Akka.Actor;
using Akka.Event;
using CoreWars.Common;
using CoreWars.Common.TypedActorQuery.Ask;
using CoreWars.Coordination.Messages;

namespace CoreWars.Coordination.GameSlot
{
    public class CompetitionSlot : FSM<CompetitionSlotState, ICompetitionSlotFSMData>
    {
        private readonly IActorRef _competitorSource;
        private readonly ILoggingAdapter _logger = Context.GetLogger();

        // ReSharper disable once MemberCanBePrivate.Global
        // public constructor required for akka
        public CompetitionSlot(
            IActorRef competitorSource
            , IActorRef competitionsResultHandler
            , ICompetitionActorPropsFactory competitionActorPropsFactory)
        {
            _competitorSource = competitorSource;
            StartWith(CompetitionSlotState.Idle, Uninitialized.Instance);
            
            When(CompetitionSlotState.Idle, state =>
            {
                if (state.FsmEvent is RunCompetition runCompetition)
                    return GoToLobby(competitorSource);

                return null;
            });
            
            When(CompetitionSlotState.Lobby, state =>
            {
                if (state.FsmEvent is TypedAskResult<AgentsOrderCompleted> agentsOrderCompleted)
                {
                    var competitorActorProps = competitionActorPropsFactory.Build(agentsOrderCompleted.Answer.Agents);
                    var competitorActor = Context.ActorOf(competitorActorProps);
                    
                    return GoTo(CompetitionSlotState.Game)
                        .Using(
                            new ActiveGameData(
                                competitorActor
                                , agentsOrderCompleted.Answer.Agents.ToList()));
                }

                if (state.FsmEvent is NotEnoughPlayers)
                {
                    return GoTo(CompetitionSlotState.Idle);
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
                _logger.Info("Changed state from {0} to {1}", initialState, nextState);
                switch (initialState)
                {
                    case CompetitionSlotState.Lobby when StateData is QueryData queryStateData:
                        Context.Stop(queryStateData.Query);
                        break;
                    case CompetitionSlotState.Game when StateData is ActiveGameData activeGameData:
                        Context.Stop(activeGameData.Game);
                        break;
                    case CompetitionSlotState.Idle:
                    case CompetitionSlotState.Conclude:
                        break;
                    case CompetitionSlotState.Terminate:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(initialState), initialState, null);
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
                    case CompetitionSlotState.Idle:
                        Context.System.Scheduler.ScheduleTellOnce(
                            TimeSpan.FromSeconds(2),
                            Self,
                            RunCompetition.Instance,
                            Self);
                        break;
                    case CompetitionSlotState.Lobby:
                    case CompetitionSlotState.Terminate:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(nextState), nextState, null);
                }
            });
            
            
            Initialize();
        }

        public override void AroundPostRestart(Exception cause, object message)
        {
            base.AroundPostRestart(cause, message);
        }

        public static Props Props(
            IActorRef competitorsSource
            , IActorRef resultHandler
            , ICompetitionActorPropsFactory competitorsPropsFactory)
        {
            return Akka.Actor.Props.Create(() => new CompetitionSlot(competitorsSource, resultHandler, competitorsPropsFactory));
        }
        
        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                loggingEnabled: true,
                localOnlyDecider: ex =>
                {
                    switch (ex)
                    {
                        case TimeoutException:
                        case AskTypeMismatchException {MismatchedResponse: NotEnoughPlayers}:
                            return Directive.Restart;
                        default:
                            _logger.Error(ex, "Unhandled competition slot error");
                            return Directive.Escalate;
                    }
                });
        }

        private State<CompetitionSlotState, ICompetitionSlotFSMData> GoToLobby(
            IActorRef competitorSource)
        {
            var query = competitorSource
                .AskFor<AgentsOrderCompleted>(
                    OrderAgents.Instance
                    , Context);

            return GoTo(CompetitionSlotState.Lobby).Using(new QueryData(query));
        }
        
    }
}