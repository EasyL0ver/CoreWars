using System;
using Akka.Actor;

namespace BotArena
{
    public interface ICompetitionAgentInfo
    {
        public string Alias { get; }
        public string Author { get; }
    }
    public interface ICompetitionAgent  : ICompetitionAgentInfo
    {
        public IActorRef Reference { get; }
    }

    public class CompetitionAgent : ICompetitionAgent
    {
        public CompetitionAgent(IActorRef reference)
        {
            Reference = reference;
        }

        public string Alias { get; }
        public string Author { get; }
        public IActorRef Reference { get; }
    }
    

}