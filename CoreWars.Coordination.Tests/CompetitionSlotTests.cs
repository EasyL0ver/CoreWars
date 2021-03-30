using Akka.Actor;
using Akka.TestKit.NUnit;
using Moq;
using NUnit.Framework;

namespace CoreWars.Coordination.Tests
{
    public class CompetitionSlotTests : TestKit
    {
        private IActorRef _sut;
        private IActorRef _competitorSourceProbe;
        private IActorRef _resultHandlerProbe;

        [SetUp]
        public void Setup()
        {
            _competitorSourceProbe = CreateTestProbe();
            _resultHandlerProbe = CreateTestProbe();
            
            //_sut = this.Sys.ActorOf<>()
        }
    }
}