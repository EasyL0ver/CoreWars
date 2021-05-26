using System;
using System.Threading.Tasks;
using Akka.Actor;
using CoreWars.Common;
using CoreWars.Common.Exceptions;
using CoreWars.Player.Messages;
using CoreWars.WebApp.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CoreWars.WebApp.Actors.Notification
{
    public class StatusObserver : ReceiveActor
    {
        private const int IdentityTimeoutMillis = 5000;
        
        private readonly Guid _competitorId;
        private readonly CompetitorStatusCache _cache;
        private readonly IHubContext<CompetitorNotificationHub> _hubContext;
        private readonly string _connectionId;

        private ICancelable _timeoutCancellable;
        private IActorRef _watchedCompetitor;
        
        public StatusObserver(Guid competitorId, IHubContext<CompetitorNotificationHub> hubContext, string connectionId)
        {
            _competitorId = competitorId;
            _hubContext = hubContext;
            _connectionId = connectionId;
            _cache = new CompetitorStatusCache();

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

                if (msg.Subject == null)
                {
                    _cache.State = CompetitorState.Faulted;
                    Self.Tell(Messages.ScheduleUpdate.Instance);
                    return;
                }

                _watchedCompetitor = msg.Subject;
                _watchedCompetitor.Tell(Subscribe.Instance);

                _cache.State = CompetitorState.Active;
                
                Context.Watch(_watchedCompetitor);
                Become(ListeningForStatus);
            });
            Receive<Messages.IdentityTimeout>(msg =>
            {
                _cache.State = CompetitorState.Faulted;
                Self.Tell(Messages.ScheduleUpdate.Instance);
            });
            Receive<Messages.ScheduleUpdate>(async msg =>
            {
                var updateMessage = _cache.GetMessage();
                await NotifyUser(_connectionId, updateMessage);
            });
        }

        private void ListeningForStatus()
        {
            Receive<Messages.GetCurrent>(msg =>
            {
                Sender.Tell(_cache);
            });
            Receive<Data.Entities.Messages.ScriptStatisticsUpdated>(msg =>
            {
                _cache.Wins = msg.Wins;
                _cache.GamesPlayed = msg.GamesPlayed;
                _cache.State = CompetitorState.Active;
                
                Self.Tell(Messages.ScheduleUpdate.Instance);
            });
            Receive<Terminated>(msg =>
            {
                _cache.State = CompetitorState.Faulted;
                
                Self.Tell(Messages.ScheduleUpdate.Instance);
            });
            Receive<Messages.ScheduleUpdate>(async msg =>
            {
                var updateMessage = _cache.GetMessage();
                await NotifyUser(_connectionId, updateMessage);
            });
        }
        
        private async Task NotifyUser(string connectionId, object status)
        {
            await _hubContext.Clients.Client(connectionId).SendAsync("Status", status);
        }
    }
}