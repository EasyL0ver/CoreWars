using System;
using System.Threading.Tasks;
using Akka.Actor;
using CoreWars.Common;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Messages = CoreWars.WebApp.Actors.Messages;

namespace CoreWars.WebApp.Hubs
{
    [UsedImplicitly]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId = Context.ConnectionId;
            var unregisterMessage = new Messages.NotificationUserDisconnected(connectionId);

            await _gameService.NotificationProvider
                .Ask<Acknowledged>(unregisterMessage, TimeSpan.FromSeconds(5));
            
            await base.OnDisconnectedAsync(exception);
        }
    }
}