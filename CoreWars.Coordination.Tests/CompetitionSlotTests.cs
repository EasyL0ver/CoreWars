using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.TestKit;
using Akka.TestKit.NUnit;
using CoreWars.Common;
using CoreWars.Competition;
using CoreWars.Coordination.GameSlot;
using CoreWars.Coordination.Messages;
using Moq;
using NUnit.Framework;

namespace CoreWars.Coordination.Tests
{
    public class CompetitionSlotTests : TestKit
    {
        private IActorRef _sut;
        private TestProbe _competitorSourceProbe;
        private TestProbe _resultHandlerProbe;
        private TestProbe _gameProbe;
        private Mock<ICompetitionActorPropsFactory> _competitionActorFactoryMock;

        [SetUp]
        public void Setup()
        {
            _competitorSourceProbe = CreateTestProbe();
            _resultHandlerProbe = CreateTestProbe();
            _gameProbe = CreateTestProbe();

            _competitionActorFactoryMock = new Mock<ICompetitionActorPropsFactory>();
            // _competitionActorFactoryMock
            //     .Setup(
            //         x => x.Build(
            //             It.IsAny<IEnumerable<IActorRef>>()
            //                 , It.IsAny<IActorContext>()))
            //     .Returns(_gameProbe);

            var sutProps = CompetitionSlot.Props(
                _competitorSourceProbe
                , _resultHandlerProbe
                , _competitionActorFactoryMock.Object);
            
            _sut = this.Sys.ActorOf(sutProps);
        }

        [Test]
        public void ChangeStateToLobby_CompetitorsSourceIsQueriedOnce()
        {
            _sut.Tell(RunCompetition.Instance);
            _competitorSourceProbe.ExpectMsg<OrderAgents>();
        }
        
        [Test]
        public void ChangeStateToGame_GameActorIsInitialized()
        {
            _sut.Tell(RunCompetition.Instance);
            _competitorSourceProbe.Send(_sut, new AgentsOrderCompleted(Array.Empty<IActorRef>()));

            _gameProbe.ExpectMsg<Competition.Messages.RunCompetitionMessage>();
        }
        
        // [Test]
        // public void CompleteGame_NewLobbyIsInstantiated()
        // {
        //     _sut.Tell(RunCompetition.Instance);
        //     _competitorSourceProbe.Send(_sut, new AgentsOrderCompleted(Array.Empty<IActorRef>()));
        //     
        // }
  
    }
}