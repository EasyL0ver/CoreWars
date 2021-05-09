using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.Util.Internal;
using CoreWars.Common;
using CoreWars.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoreWars.Data
{
    public class ScriptRepositoryActor : ReceiveActor
    {
        private readonly HashSet<IActorRef> _subscribed = new();
        
        public ScriptRepositoryActor(IDataContext context)
        {
            Receive<Script>(msg =>
            {
                context.Scripts.Add(msg);
                context.Commit();

                var @event = new Messages.AddedEvent<Script>(msg);
                Extensions.ForEach(_subscribed, sub =>
                {
                    sub.Tell(@event);
                });
                
                Sender.Tell(new Acknowledged());
            });

            //todo replace with stream ??
            Receive<Messages.GetAll>(msg =>
            {
                Sender.Tell(context.Scripts.ToList());
            });

            Receive<Messages.GetAllForCompetition>(msg =>
            {
                var competitionScripts = context.Scripts
                    .Where(x => x.CompetitionName == msg.CompetitionName)
                    .Include(x => x.User)
                    .ToList();
                
                Sender.Tell(competitionScripts);
            });

            Receive<Messages.Subscribe>(msg =>
            {
                _subscribed.Add(Sender);
            });
            
            
        }
        
        public static Props Props(IDataContext ctx) => Akka.Actor.Props.Create(() => new ScriptRepositoryActor(ctx)); 
    }
}