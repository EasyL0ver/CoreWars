using Akka.Actor;
using Akka.TestKit;
using Akka.TestKit.NUnit;
using CoreWars.Coordination.Messages;
using Moq;
using NUnit.Framework;

namespace CoreWars.Coordination.Tests
{
    public class CompetitionLobbyTests : TestKit
    {
        private IActorRef _sut;
        private Mock<ILobbyConfig> _lobbyConfigMock;
        private Mock<ICollectionSelectionStrategy<IActorRef>> _selectionStrategyMock;
        private TestProbe _resultHandlerActorTestProbe;

        [SetUp]
        public void SetUp()
        {
            
            _lobbyConfigMock = new Mock<ILobbyConfig>();
            _selectionStrategyMock = new Mock<ICollectionSelectionStrategy<IActorRef>>();
            _resultHandlerActorTestProbe = CreateTestProbe("result-handler");
            
            var sutProps =
                Props.Create<CompetitionRoot>(
                    _selectionStrategyMock.Object
                    );
        }

    }
}