using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Event;
using CoreWars.Common;
using CoreWars.Common.Exceptions;
using CoreWars.Data.Entities;
using CoreWars.Player.Exceptions;
using CoreWars.Player.Messages;

namespace CoreWars.Player
{
    public class Competition : ReceiveActor
    {
        private readonly IActorRef _scriptRepository;
        private readonly IActorRef _lobby;
        private readonly ICompetitionInfo _competitionInfo;
        private readonly ICompetitorFactory _competitorFactory;
        private readonly IActorRef _resultsRepository;
        private readonly ILoggingAdapter _logger = Context.GetLogger();
        
        public Competition(
            IActorRef scriptRepository
            , ICompetitionInfo competitionInfo
            , IActorRef lobby
            , ICompetitorFactory competitorFactory
            , IActorRef resultsRepository)
        {
            _scriptRepository = scriptRepository;
            _competitionInfo = competitionInfo;
            _lobby = lobby;
            _competitorFactory = competitorFactory;
            _resultsRepository = resultsRepository;

            EnsureSubscription(_scriptRepository);

            Receive<IEnumerable<Script>>(OnScriptBatchReceived);
            Receive<Data.Entities.Messages.AddedEvent<Script>>(OnNewScriptAdded);
            Receive<Data.Entities.Messages.UpdatedEvent<Script>>(OnScriptUpdated);
            Receive<Data.Entities.Messages.DeletedEvent<Script>>(OnScriptDeleted);

        }

        private void OnScriptDeleted(Data.Entities.Messages.DeletedEvent<Script> obj)
        {
            if (obj.DeletedElement.CompetitionName != _competitionInfo.Name)
                return;
            
            _logger.Info($"Deleting competitor: {obj.DeletedElement.Name} from {obj.DeletedElement.ScriptType} script with id: {obj.DeletedElement.Id}");
            Context.ActorSelection($"{obj.DeletedElement.Id}").Tell(PoisonPill.Instance);
        }

        public static Props Props(
            IActorRef scriptRepository
            , ICompetitionInfo competitionInfo
            , IActorRef lobby
            , ICompetitorFactory factory
            , IActorRef resultRepository)
        {
            return Akka.Actor.Props.Create(() => new Competition(scriptRepository, competitionInfo, lobby, factory, resultRepository));
        }
        
        private void OnScriptUpdated(Data.Entities.Messages.UpdatedEvent<Script> obj)
        {
            if (obj.AddedElement.CompetitionName != _competitionInfo.Name)
                return;
            
            _logger.Info($"Updating competitor: {obj.AddedElement.Name} from {obj.AddedElement.ScriptType} script with id: {obj.AddedElement.Id}");

            var message = new CompetitorScriptUpdated(obj.AddedElement);
            
            Context.ActorSelection($"{obj.AddedElement.Id}").Tell(message);
        }

        private void OnNewScriptAdded(Data.Entities.Messages.AddedEvent<Script> obj)
        {
            if (obj.AddedElement.CompetitionName != _competitionInfo.Name)
                return;
            
            
            CreateCompetitor(obj.AddedElement, CompetitorState.Inconclusive);
        }

        private void OnScriptBatchReceived(IEnumerable<Script> obj)
        {
            //ensure children ale deleted
            Context.GetChildren().ForEach(child => child.Tell(PoisonPill.Instance));
            
            obj.ForEach(script =>
            {
                var scriptActive = script.Stats != null && script.Stats.GamesPlayed > 0;
                var scriptStatus = scriptActive ? CompetitorState.Active : CompetitorState.Inconclusive;
                
                CreateCompetitor(script, scriptStatus);
            });
        }

        protected override void PreStart()
        {
            base.PreStart();
            _scriptRepository.Tell(new Data.Entities.Messages.GetAllForCompetition(_competitionInfo.Name));
        }

        private void CreateCompetitor(Script script, CompetitorState initialState = CompetitorState.Inconclusive)
        {
            _logger.Info($"Creating competitor: {script.Name} from {script.ScriptType} script with id: {script.Id}");
            var competitorProps = Competitor.Props(_competitorFactory, _lobby, script.User, script, _resultsRepository, initialState);

            Context.ActorOf(competitorProps, script.Id.ToString());
        }

        private void EnsureSubscription(ICanTell repository)
        {
            Context.System.Scheduler.ScheduleTellRepeatedly(
                TimeSpan.Zero
                , TimeSpan.FromMinutes(1)
                , repository
                , Data.Entities.Messages.Subscribe.Instance
                , Self);
        }
        
        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                localOnlyDecider: ex =>
                {
                    switch (ex)
                    {
                        case CompetitorFaultedException competitorFaultedException:
                            var message = new Data.Entities.Messages.ReportScriptFailure(
                                competitorFaultedException.CompetitorId
                                , competitorFaultedException.GetBaseException());
                            
                            _scriptRepository.Tell(message);
                            
                            return Directive.Resume;
                        default:
                            return Directive.Escalate;
                    }
                });
        }
    }
}