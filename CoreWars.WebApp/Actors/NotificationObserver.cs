using System;
using System.Threading.Tasks;
using Akka.Actor;
using CoreWars.Common;
using CoreWars.Player.Messages;

namespace CoreWars.WebApp.Actors
{
    public class NotificationObserver : ReceiveActor
    {
        private const int IdentityTimeoutMillis = 5000;
        
        private readonly Guid _competitorId;
        private readonly Func<CompetitorStatus, Task> _notify;

        private ICancelable _timeoutCancellable;
        private IActorRef _watchedCompetitor;
        private CompetitorStatus _currentStatus;

        public NotificationObserver(Guid competitorId, Func<CompetitorStatus, Task> notify)
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

            _timeoutCancellable = Context.System.Scheduler
                .ScheduleTellOnceCancelable(
                    TimeSpan.FromMilliseconds(IdentityTimeoutMillis)
                    , Self
                    , Messages.IdentityTimeout.Instance
                    , Self);
        }

        private void WaitingForIdentity()
        {
            Receive<ActorIdentity>(msg =>
            {
                _timeoutCancellable.Cancel();
                _watchedCompetitor = msg.Subject;
                _watchedCompetitor.Tell(Subscribe.Instance);
                Context.Watch(_watchedCompetitor);
                Become(ListeningForStatus);
            });
            Receive<Messages.IdentityTimeout>(msg => throw new TimeoutException());
        }

        private void ListeningForStatus()
        {
            Receive<Messages.GetCurrent>(msg =>
            {
                Sender.Tell(_currentStatus);
            });
            Receive<CompetitorStatus>(async msg =>
            {
                _currentStatus = msg;
                await _notify(_currentStatus);
            });
            Receive<Terminated>(async msg =>
            {
                _currentStatus = _currentStatus.Terminated();
                await _notify(_currentStatus);
            });
        }
    }
}