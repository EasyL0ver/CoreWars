using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;

namespace CoreWars.Common.TypedActorQuery
{
    public static class ActorQuery
    {
        public static ActorQueryBuilder<T> WithExpectedResponse<T>()
        {
            return new ActorQueryBuilder<T>();
        }

        public static ActorQueryBuilder<Acknowledged> WithoutResponse()
        {
            return new ActorQueryBuilder<Acknowledged>();
        }
    }
    public class ActorQueryBuilder<TOut>
    {
        private Func<IActorRef, object> _messageSelector;
        private TimeSpan _timeoutTimeSpan = TimeSpan.MaxValue;
        private IActorRef[] _recipients;
        private TypedQueryResultHandler<TOut> _resultHandler;
        
        
        public ActorQueryBuilder<TOut> WithMessage(object message)
        {
            if (_messageSelector != null)
                throw new InvalidOperationException("query message already set");
            
            _messageSelector = (a) => message;
            return this;
        }
        
        public ActorQueryBuilder<TOut> WithMessageSelector(Func<IActorRef, object> message)
        {
            if (_messageSelector != null)
                throw new InvalidOperationException("query message already set");
            
            _messageSelector = message;
            return this;
        }
        
        public ActorQueryBuilder<TOut> WithRecipient(IActorRef recipientRef)
        {
            _recipients = new[] {recipientRef};
            return this;
        }
        
        public ActorQueryBuilder<TOut> WithRecipients(IEnumerable<IActorRef> recipientRefs)
        {
            _recipients = recipientRefs.ToArray();
            return this;
        }
        
        public ActorQueryBuilder<TOut> MessageResultToParent()
        {
            _resultHandler = QueryResultStrategy.ReportResultToParent<TOut>();
            return this;
        }
        
        public ActorQueryBuilder<TOut> RunOnFinished(TypedQueryResultHandler<TOut> action)
        {
            _resultHandler = action;
            return this;
        }
        
        public ActorQueryBuilder<TOut> WithTimeout(TimeSpan timeoutTimeSpan)
        {
            _timeoutTimeSpan = timeoutTimeSpan;
            return this;
        }

        public Props GetProps()
        {
            return Props.Create<TypedQueryActor<TOut>>(_recipients, _messageSelector, _resultHandler, _timeoutTimeSpan);
        }

        public IActorRef RunOn(IActorContext context)
        {
            var props = GetProps();
            return context.ActorOf(props);
        }
        
    }
}