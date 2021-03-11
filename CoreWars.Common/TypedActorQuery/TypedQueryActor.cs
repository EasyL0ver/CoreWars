using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;

namespace CoreWars.Common.TypedActorQuery
{
    //todo enable restart
    //todo watch for actor termination during query
    public class TypedQueryActor<TQuery, TResponse> : ReceiveActor
    {
        private readonly IReadOnlyList<IActorRef> _queriedActors;
        private readonly TQuery _queryMessage;
        private readonly IActorRef _queryResultCallback;
        private readonly IDictionary<IActorRef, TResponse> _responses;
        private readonly ICancelable _timeoutCancelable;
        
        public TypedQueryActor(
            IEnumerable<IActorRef> queriedActors
            , TQuery queryMessage
            , IActorRef queryResultCallback
            , TimeSpan timeoutTimeSpan)
        {
            _queriedActors = queriedActors.ToList();
            _queryMessage = queryMessage;
            _queryResultCallback = queryResultCallback;

            _responses = new Dictionary<IActorRef, TResponse>(_queriedActors.Count);
            
            _timeoutCancelable = Context.System.Scheduler
                .ScheduleTellOnceCancelable(
                    timeoutTimeSpan
                    , Self
                    , new TimeoutException()
                    , Self);

            Become(WaitingForResponses);
        }
        
        protected override void PreStart()
        {
            _queriedActors.ForEach(actor => actor.Tell(_queryMessage));
        }
        
        protected override void PostStop()
        {
            _timeoutCancelable.Cancel();
        }

        private void WaitingForResponses(object message)
        {
            Receive<TResponse>(response =>
            {
                _responses[Sender] = response;

                if (_responses.Count != _queriedActors.Count)
                    return;
                
                _queryResultCallback.Tell(new TypedQueryResult<TResponse>(_responses));
                Context.Stop(Self);
            });

            Receive<TimeoutException>(ex =>
            {
                var timedOutActors = _queriedActors
                    .Where(actor => !_responses.ContainsKey(actor));

                throw new TypedQueryTimeoutException(timedOutActors);
            });
        }
    }
}