using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using CoreWars.Common.TypedActorQuery.Ask;

namespace CoreWars.Common.TypedActorQuery.Query
{
    public class TypedQuery<TResponse> : ReceiveActor
    {
        private readonly TimeSpan? _timeout;
        private readonly IActorRef _resultHandler;
        private readonly IReadOnlyDictionary<IActorRef, MessageResponsePair<TResponse>> _aggregatedOperations;

        public TypedQuery(
            IDictionary<IActorRef, object> targetMessages
            , IActorRef resultHandler
            , TimeSpan? timeout = null)
        {
            _resultHandler = resultHandler;
            _timeout = timeout;
            _aggregatedOperations = targetMessages.ToDictionary(
                x => x.Key
                , y => new MessageResponsePair<TResponse>(y.Value));
            
            WaitingForStartingMessage();
        }
        
        public static Props Props(
            IDictionary<IActorRef, object> targetMessages
            , IActorRef resultHandler
            , TimeSpan? timeout = null) 
            => Akka.Actor.Props.Create(() => new TypedQuery<TResponse>(targetMessages, resultHandler, timeout));
        
        private void WaitingForStartingMessage()
        {
            Receive<RunTypedQuery>(msg =>
            {
                _aggregatedOperations.ForEach(pair =>
                {
                    var target = pair.Key;
                    var message = pair.Value.Message;

                    target.AskFor<TResponse>(message, Context, _timeout);
                });
                
                Become(ReceivingResponses);
            });
        }

        private void ReceivingResponses()
        {
            Receive<TypedAskResult<TResponse>>(msg =>
            {
                _aggregatedOperations[msg.Sender].Response = msg.Answer;

                if (_aggregatedOperations.Values.All(x => x.Response != null))
                    Self.Tell(ConcludeTypedQuery.Instance);
            });

            Receive<ConcludeTypedQuery>(msg =>
            {
                var resultDictionary = _aggregatedOperations
                    .ToDictionary(
                        x => x.Key
                        , x => x.Value.Response);
                
                _resultHandler.Tell(new TypedQueryResult<TResponse>(resultDictionary));
                
                Context.Stop(Self);
            });
        }
        
        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                localOnlyDecider: ex =>
                {
                    switch (ex)
                    {
                        default:
                            return Directive.Escalate;
                    }
                });
        }
        
 

    }
}