using System;
using System.Threading.Tasks;
using Akka.Actor;
using CoreWars.Common;
using CoreWars.Player.Messages;
using CoreWars.WebApp.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CoreWars.WebApp.Actors
{
    public class NotificationObserver : ReceiveActor
    {
        private readonly Guid _competitorId;
        private readonly Func<ICompetitorStatus, Task> _notify;

        public NotificationObserver(Guid competitorId, Func<ICompetitorStatus, Task> notify)
        {
            _competitorId = competitorId;
            _notify = notify;

            Receive<ICompetitorStatus>(async msg => await _notify(msg));
        }

        protected override void PreStart()
        {
            var competitorPath = "/user/competitors/*/" + _competitorId.ToString();
            var selection = Context.ActorSelection(competitorPath);

            //stay subscribed
            Context.System.Scheduler
                .ScheduleTellRepeatedly(
                    TimeSpan.Zero
                    , TimeSpan.FromMinutes(1)
                    , selection
                    , Subscribe.Instance
                    , Self);
        }
    }
}