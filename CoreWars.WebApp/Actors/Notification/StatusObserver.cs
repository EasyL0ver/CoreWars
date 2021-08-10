using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using CoreWars.Common;
using CoreWars.Common.Exceptions;
using CoreWars.Player;
using CoreWars.Player.Messages;
using CoreWars.WebApp.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CoreWars.WebApp.Actors.Notification
{
    public class StatusObserver : ReceiveActor
    {
        private const int IdentityTimeoutMillis = 1000;

        private readonly Guid _competitorId;
        private readonly CompetitorStatusCache _cache;
        private readonly IHubContext<CompetitorNotificationHub> _hubContext;
        private readonly string _connectionId;
        private readonly ILoggingAdapter _logger = Context.GetLogger();

        private ICancelable _timeoutCancellable;
        private IActorRef _watchedCompetitor;

        public StatusObserver(Guid competitorId, IHubContext<CompetitorNotificationHub> hubContext, string connectionId)
        {
            _competitorId = competitorId;
            _hubContext = hubContext;
            _connectionId = connectionId;
            _cache = new CompetitorStatusCache(){CompetitorId = _competitorId};

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
                
                Context.System.Scheduler.ScheduleTellRepeatedly(
                    TimeSpan.Zero
                    , TimeSpan.FromSeconds(3)
                    , Self
                    , Messages.ScheduleUpdate.Instance
                    , Self);
                
                Become(ListeningForStatus);
            });
            Receive<Messages.IdentityTimeout>(msg =>
            {
                _logger.Warning("Failed to identify competitor with id: {0} ", _competitorId);
                throw new FailedNotificationHookException(_connectionId, _competitorId);
            });
    
        }

        private void ListeningForStatus()
        {
            Receive<Messages.GetCurrent>(msg => { Sender.Tell(_cache); });
            Receive<Data.Entities.Messages.ScriptStatisticsUpdated>(msg =>
            {
                _cache.Wins = msg.Wins;
                _cache.GamesPlayed = msg.GamesPlayed;
            });
            Receive<CompetitorState>(msg =>
            {
                _cache.State = msg;
            });
            Receive<AgentFailureState>(msg =>
            {
                _cache.Exception = msg.Exception;
            });
            Receive<Messages.ScheduleUpdate>(msg =>
            {
                var updateMessage = _cache.GetMessage();
                NotifyUser(_connectionId, updateMessage).PipeTo(Self);
            });
            ReceiveAny(msg => throw new InvalidOperationException($"Unknown message type received. Received object: {msg}"));
        }

        private async Task NotifyUser(string connectionId, object status)
        {
            await _hubContext.Clients.Client(connectionId).SendAsync("Status", status);
        }
    }
}