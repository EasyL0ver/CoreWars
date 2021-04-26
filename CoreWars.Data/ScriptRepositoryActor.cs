using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.Util.Internal;
using CoreWars.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoreWars.Data
{
    public class ScriptRepositoryActor : ReceiveActor
    {
        private readonly HashSet<IActorRef> _subscribed = new();
        
        public ScriptRepositoryActor(IDataContext context)
        {
            Receive<Messages.Add<GameScript>>(msg =>
            {
                context.Scripts.Add(msg.Content);
                context.Commit();

                var @event = new Messages.AddedEvent<GameScript>(msg.Content);
                _subscribed.ForEach(sub =>
                {
                    sub.Tell(@event);
                });
            });

            //todo replace with stream ??
            Receive<Messages.GetAll>(msg =>
            {
                Sender.Tell(context.Scripts.ToList());
            });

            Receive<Messages.GetAllForCompetition>(msg =>
            {
                Sender.Tell(context.Scripts.Where(x => x.CompetitionType == msg.CompetitionName).ToList());
            });

            Receive<Messages.Subscribe>(msg =>
            {
                _subscribed.Add(Sender);
            });
            
            
        }
        
        public static Props Props(IDataContext ctx) => Akka.Actor.Props.Create(() => new ScriptRepositoryActor(ctx)); 
    }
}