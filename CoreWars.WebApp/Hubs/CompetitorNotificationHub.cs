using System;
using System.Threading.Tasks;
using Akka.Actor;
using CoreWars.Common;
using CoreWars.Competition;
using JetBrains.Annotations;
using Microsoft.AspNetCore.SignalR;
using Messages = CoreWars.WebApp.Actors.Messages;

namespace CoreWars.WebApp.Hubs
{
    [UsedImplicitly]
    public class CompetitorNotificationHub : Hub
    {
        private readonly IGameService _gameService;

        public CompetitorNotificationHub(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HubMethodName("Register")]
        public Task Register(string user, string message)
        {
            var connectionId = Context.ConnectionId;
            var competitorId = Guid.Parse(message);
            var registerMessage = new Messages.RegisterCompetitorNotifications(competitorId, connectionId);

            return _gameService.NotificationProvider.Ask<Acknowledged>(registerMessage, TimeSpan.FromSeconds(5));
        }
    }
}