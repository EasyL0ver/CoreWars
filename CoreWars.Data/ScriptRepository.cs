using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.Event;
using Akka.Util.Internal;
using CoreWars.Common;
using CoreWars.Common.AkkaExtensions.Messages;
using CoreWars.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoreWars.Data
{
    public class ScriptRepository : ReceiveActor
    {
        private readonly HashSet<IActorRef> _subscribed = new();
        private readonly ILoggingAdapter _logger = Context.GetLogger();
        
        public ScriptRepository(IDataContext context)
        {
            Receive<Messages.Add<Script>>(msg =>
            {
                msg.Content.DateTimeCreated = DateTime.Now;
                msg.Content.DateTimeUpdated = DateTime.Now;
                
                var entry = context.Scripts.Add(msg.Content);
                context.Commit();

                BroadcastEvent(new Messages.AddedEvent<Script>(msg.Content));
                
                Sender.Tell(entry.Entity);
            });

            Receive<Messages.Update<Script>>(msg =>
            {
                var edited = context.Scripts
                    .Include(x => x.User)
                    .Include(x => x.Stats)
                    .Include(x => x.FailureInfo)
                    .FirstOrDefault(x => x.Id == msg.Content.Id);

                if (edited == null)
                {
                    _logger.Warning("Updated script with id {0} not found!", msg.Content.Id);
                    return;
                }
                edited.ScriptType = msg.Content.ScriptType;
                edited.CompetitionName = msg.Content.CompetitionName;
                edited.ScriptFiles = msg.Content.ScriptFiles;
                edited.Name = msg.Content.Name;
                edited.DateTimeUpdated = DateTime.Now;
                context.Commit();
                
                Self.Tell(new Messages.ClearScriptError(msg.Content.Id));
                BroadcastEvent(new Messages.UpdatedEvent<Script>(msg.Content));
                Sender.Tell(edited);
            });

            Receive<Messages.Delete<Script>>(msg =>
            {
                var deleted = context.Scripts
                    .FirstOrDefault(x => x.Id == msg.DeletedObjectId);

                if (deleted == null )
                {
                    _logger.Warning("Updated script with id {0} not found!", msg.DeletedObjectId);
                    return;
                }

                if (deleted.UserId != msg.UserRequestedId)
                {
                    _logger.Warning("Insufficient rights to delete script with id {0}", msg.DeletedObjectId);
                    return;
                }

                deleted.IsArchived = true;
                context.Commit();
                
                BroadcastEvent(new Messages.DeletedEvent<Script>(deleted));
                Sender.Tell(Acknowledged.Instance);
            });

            Receive<Messages.ClearScriptError>(msg =>
            {
                var failureEntities = context.Failures.Where(x => x.ScriptId == msg.ScriptId).ToArray();
                context.Failures.RemoveRange(failureEntities);
                context.Commit();
            });

            Receive<Messages.GetAllForCompetition>(msg =>
            {
                var competitionScripts = context.Scripts
                    .Include(x => x.User)
                    .Include(x => x.Stats)
                    .Include(x => x.FailureInfo)
                    .Where(x => x.CompetitionName == msg.CompetitionName)
                    .Where(x => !x.IsArchived)
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
                    .Where(x => !x.IsArchived)
                    .OrderBy(x => x.DateTimeCreated)
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
                    , Exception = msg.Exception.GetBaseException().Message
                };

                context.Failures.Add(failureEntity);
                context.Commit();
            });
        }

        private void BroadcastEvent(object eventParameters)
        {
            Extensions.ForEach(_subscribed, sub =>
            {
                sub.Tell(eventParameters);
            });
        }
        
        public static Props Props(IDataContext ctx) => Akka.Actor.Props.Create(() => new ScriptRepository(ctx)); 
    }
}