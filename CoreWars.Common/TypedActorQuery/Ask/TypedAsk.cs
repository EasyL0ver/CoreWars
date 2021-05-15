using System;
using Akka.Actor;
using JetBrains.Annotations;

namespace CoreWars.Common.TypedActorQuery.Ask
{
    //todo monitor target for termination
    [UsedImplicitly]
    internal class TypedAsk<TAnswer> : ReceiveActor
    {
        private const int DefaultAskTimeoutMillis = 1000;
        
        private readonly TimeSpan? _timeoutTimeSpan;
        private readonly IActorRef _target;
        private readonly object _queryMessage;
        private readonly IActorRef _resultHandler;
        
        private ICancelable _timeoutCancelable;

        public TypedAsk(
            IActorRef target
            , object  queryMessage
            , IActorRef resultHandler
            , TimeSpan? timeoutTimeSpan = null) 
        {
            _target = target;
            _queryMessage = queryMessage;
            _resultHandler = resultHandler;
            
            _timeoutTimeSpan = timeoutTimeSpan 
                               ?? TimeSpan.FromMilliseconds(DefaultAskTimeoutMillis);
            
            Receive<TAnswer>(response =>
            {
                _resultHandler.Tell(new TypedAskResult<TAnswer>(response, Sender));
                Context.Stop(Self);
            });
            
            Receive<TimeElapsed>(ex => throw new TimeoutException());
            Receive<Terminated>(msg => throw new AskTargetTerminatedException(msg.ActorRef, "watched ask target terminated"));
            ReceiveAny(msg =>
            {
                var unhandledMessageType =
                    $"Invalid type response for typed ask. Received message: {msg} from: {Sender}";
                throw new AskTypeMismatchException(unhandledMessageType, msg);
            });
        }

        public static Props Props(
            IActorRef target
            , object askMessage
            , IActorRef resultHandler
            , TimeSpan? timeout = null)
        {
            return Akka.Actor.Props
                .Create(() => new TypedAsk<TAnswer>(target, askMessage, resultHandler, timeout));
        }

        protected override void PreStart()
        {
            base.PreStart();
            Context.Watch(_target);
            _target.Tell(_queryMessage);
            RefreshTimer();
        }

        protected override void PostRestart(Exception reason)
        {
            base.PostRestart(reason);
            RefreshTimer();
        }

        private void RefreshTimer()
        {
            _timeoutCancelable?.Cancel();
            if (_timeoutTimeSpan != null)
                _timeoutCancelable = ScheduleTimeoutMessage(_timeoutTimeSpan.Value);
        }

        private ICancelable ScheduleTimeoutMessage(TimeSpan after)
        {
            return Context.System.Scheduler
                .ScheduleTellOnceCancelable(
                    after
                    , Self
                    , new TimeElapsed()
                    , Self);
        }
        
        protected override void PostStop()
        {
            _timeoutCancelable?.Cancel();
        }
    }
}