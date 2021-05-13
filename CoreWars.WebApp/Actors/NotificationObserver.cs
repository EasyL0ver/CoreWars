using System;
using System.Threading.Tasks;
using Akka.Actor;
using CoreWars.Common;
using CoreWars.Player.Messages;

namespace CoreWars.WebApp.Actors
{
    //todo keep win/loss ratio after competitor faulting
    //todo implement restart after timeout!
    public class NotificationObserver : ReceiveActor
    {
        private readonly Guid _competitorId;
        private readonly Func<ICompetitorStatus, Task> _notify;

        private IActorRef _watchedCompetitor;
        private ICompetitorStatus _currentStatus;

        public NotificationObserver(Guid competitorId, Func<ICompetitorStatus, Task> notify)
        {
            _competitorId = competitorId;
            _notify = notify;

            WaitingForIdentity();
        }
        
        protected override void PreStart()
        {
            var competitorPath = "/user/competitors/*/" + _competitorId.ToString();
            var selection = Context.ActorSelection(competitorPath);
            
            selection.Tell(new Identify(_competitorId));
        }

        private void WaitingForIdentity()
        {
            //todo throw in timeout
            Receive<ActorIdentity>(msg =>
            {
                _watchedCompetitor = msg.Subject;
                _watchedCompetitor.Tell(Subscribe.Instance);
                Context.Watch(_watchedCompetitor);
                Become(ListeningForStatus);
            });
        }

        private void ListeningForStatus()
        {
            Receive<Messages.GetCurrent>(msg =>
            {
                Sender.Tell(_currentStatus);
            });
            Receive<ICompetitorStatus>(async msg =>
            {
                _currentStatus = msg;
                await _notify(_currentStatus);
            });
            Receive<Terminated>(async msg =>
            {
                _currentStatus = CompetitorStatus.Terminated;
                await _notify(_currentStatus);
            });
        }
    }
}