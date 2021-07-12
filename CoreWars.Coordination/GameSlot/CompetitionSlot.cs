using System;
using System.Collections.Generic;
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
        private readonly ILoggingAdapter _logger = Context.GetLogger();

        // ReSharper disable once MemberCanBePrivate.Global
        // public constructor required for akka
        public CompetitionSlot(
            IActorRef competitorSource
            , ICompetitionActorPropsFactory competitionActorPropsFactory)
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
                if (state.FsmEvent is TypedAskResult<IEnumerable<GeneratedAgent>> agentsOrderCompleted)
                {
                    var competitorActorProps = competitionActorPropsFactory.Build(agentsOrderCompleted.Answer);
                    var competitorActor = Context.ActorOf(competitorActorProps);
                    
                    return GoTo(CompetitionSlotState.Game)
                        .Using(
                            new ActiveGameData(
                                competitorActor
                                , agentsOrderCompleted.Answer.ToList()));
                }

                if (state.FsmEvent is NotEnoughPlayers)
                {
                    return GoTo(CompetitionSlotState.Idle);
                }

                return null;
            });
            
            When(CompetitionSlotState.Game, state =>
            {
                if (state.FsmEvent is LobbyGameTerminated competitionResult)
                {
                    return GoToLobby(competitorSource);
                }

                return null;
            });
            
            
            OnTransition((initialState, nextState) =>
            {
                _logger.Debug("Changed state from {0} to {1}", initialState, nextState);
                switch (initialState)
                {
                    case CompetitionSlotState.Lobby when StateData is QueryData queryStateData:
                        Context.Stop(queryStateData.Query);
                        break;
                    case CompetitionSlotState.Game when StateData is ActiveGameData activeGameData:
                        Context.Stop(activeGameData.Game);
                        break;
                    case CompetitionSlotState.Idle:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(initialState), initialState, null);
                }

                switch (nextState)
                {

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
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(nextState), nextState, null);
                }
            });
            
            
            Initialize();
        }

        public static Props Props(
            IActorRef competitorsSource
            , ICompetitionActorPropsFactory competitorsPropsFactory)
        {
            return Akka.Actor.Props.Create(() => new CompetitionSlot(competitorsSource, competitorsPropsFactory));
        }
        
        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                loggingEnabled: false,
                localOnlyDecider: ex =>
                {
                    switch (ex)
                    {
                        case TimeoutException:
                        case AskTypeMismatchException {MismatchedResponse: NotEnoughPlayers}:
                            _logger.Debug(ex,"Cannot start game because of exception - restarting");
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
                .AskFor<IEnumerable<GeneratedAgent>>(
                    OrderAgents.Instance
                    , Context);

            return GoTo(CompetitionSlotState.Lobby).Using(new QueryData(query));
        }
        
    }
}