using Akka.Actor;

namespace CoreWars.Common.AkkaExtensions
{
    public static class SupervisorStrategies
    {
        public static SupervisorStrategy AlwaysEscalate(bool loggingEnabled = true)
        {
            return new OneForOneStrategy(
                loggingEnabled: true
                , localOnlyDecider: ex =>
                
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