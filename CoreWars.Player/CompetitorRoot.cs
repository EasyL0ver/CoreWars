using System;
using System.Collections.Generic;
using Akka.Actor;
using CoreWars.Common;
using CoreWars.Data.Entities;

namespace CoreWars.Player
{
    public class CompetitorRoot : ReceiveActor
    {
        private readonly IActorRef _scriptRepository;
        private readonly IActorRef _lobby;
        private readonly ICompetitionInfo _competitionInfo;
        private readonly ICompetitorFactory _competitorFactory;
        
        public CompetitorRoot(
            IActorRef scriptRepository
            , ICompetitionInfo competitionInfo
            , IActorRef lobby
            , ICompetitorFactory competitorFactory)
        {
            _scriptRepository = scriptRepository;
            _competitionInfo = competitionInfo;
            _lobby = lobby;
            _competitorFactory = competitorFactory;

            EnsureSubscription(_scriptRepository);

            Receive<IEnumerable<Script>>(OnScriptBatchReceived);
            Receive<Data.Entities.Messages.AddedEvent<Script>>(OnNewScriptAdded);

        }

        public static Props Props(
            IActorRef scriptRepository
            , ICompetitionInfo competitionInfo
            , IActorRef lobby
            , ICompetitorFactory factory)
        {
            return Akka.Actor.Props.Create(() => new CompetitorRoot(scriptRepository, competitionInfo, lobby, factory));
        }

        private void OnNewScriptAdded(Data.Entities.Messages.AddedEvent<Script> obj)
        {
            if (obj.AddedElement.CompetitionName != _competitionInfo.Name)
                return;
            
            CreateCompetitor(obj.AddedElement);
        }

        private void OnScriptBatchReceived(IEnumerable<Script> obj)
        {
            //ensure children ale deleted
            Context.GetChildren().ForEach(child => child.Tell(PoisonPill.Instance));
            
            obj.ForEach(CreateCompetitor);
        }

        protected override void PreStart()
        {
            base.PreStart();
            _scriptRepository.Tell(new Data.Entities.Messages.GetAllForCompetition(_competitionInfo.Name));
        }

        private void CreateCompetitor(Script script)
        {
            var competitorAgentProps = _competitorFactory.Build(script);
            var competitorProps = Competitor.Props(competitorAgentProps, _lobby, script.User, script);

            Context.ActorOf(competitorProps);
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
    }
}