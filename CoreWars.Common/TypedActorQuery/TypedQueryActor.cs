using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using JetBrains.Annotations;

namespace CoreWars.Common.TypedActorQuery
{
    //todo enable restart
    //todo watch for actor termination during query
    //todo log received messages of invalid type
    [UsedImplicitly]
    public class TypedQueryActor<TResponse> : ReceiveActor
    {
        private readonly IReadOnlyList<IActorRef> _queriedActors;
        private readonly Func<IActorRef, object> _getQueryMessage;
        private readonly IDictionary<IActorRef, TResponse> _responses;
        private readonly ICancelable _timeoutCancelable;
        private readonly TypedQueryResultHandler<TResponse> _resultHandler;
        
        public TypedQueryActor(
            IEnumerable<IActorRef> queriedActors
            , Func<IActorRef, object>  getQueryMessage
            , TypedQueryResultHandler<TResponse> resultHandler
            , TimeSpan timeoutTimeSpan)
        {
            _queriedActors = queriedActors.ToList();
            _getQueryMessage = getQueryMessage;
            _resultHandler = resultHandler;

            _responses = new Dictionary<IActorRef, TResponse>(_queriedActors.Count);
            
            _timeoutCancelable = Context.System.Scheduler
                .ScheduleTellOnceCancelable(
                    timeoutTimeSpan
                    , Self
                    , new TimeoutException()
                    , Self);

            Receive<TResponse>(response =>
            {
                _responses[Sender] = response;

                if (_responses.Count != _queriedActors.Count)
                    return;
                
                _resultHandler.Invoke(
                    Context
                    , new TypedQueryResult<TResponse>(_responses));
                
                Context.Stop(Self);
            });

            Receive<TimeoutException>(ex =>
            {
                var timedOutActors = _queriedActors
                    .Where(actor => !_responses.ContainsKey(actor));

                throw new TypedQueryTimeoutException(timedOutActors);
            });
        }
        
        protected override void PreStart()
        {
            _queriedActors.ForEach(actor => actor.Tell(_getQueryMessage(actor)));
        }
        
        protected override void PostStop()
        {
            _timeoutCancelable.Cancel();
        }
    }
}