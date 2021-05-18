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

            Receive<Messages.GetAllForCompetition>(msg =>
            {
                var competitionScripts = context.Scripts
                    .Include(x => x.User)
                    .Include(x => x.Stats)
                    .Include(x => x.FailureInfo)
                    .Where(x => x.CompetitionName == msg.CompetitionName)
                    .Where(x => x.FailureInfo == null)
                    .ToList();
                
                Sender.Tell(competitionScripts);
            });
            
            Receive<Messages.GetAllForUser>(msg =>
            {
                var competitionScripts = context.Scripts
                    .Include(x => x.User)
                    .Include(x => x.Stats)
                    .Include(x => x.FailureInfo)
                    .Where(x => x.UserId == msg.UserId)
                    .ToList();
                
                Sender.Tell(competitionScripts);
            });
            
            Receive<Messages.Subscribe>(msg =>
            {
                _subscribed.Add(Sender);
            });
            Receive<Messages.ReportScriptFailure>(msg =>
            {
                var failureEntity = new ScriptFailure()
                {
                    ScriptId = msg.ScriptId
                    , FailureDateTime = msg.FailureDateTime
                    , Exception = msg.Exception.ToString()
                };

                context.Failures.Add(failureEntity);
                context.Commit();
            });
        }
        
        public static Props Props(IDataContext ctx) => Akka.Actor.Props.Create(() => new ScriptRepositoryActor(ctx)); 
    }
}