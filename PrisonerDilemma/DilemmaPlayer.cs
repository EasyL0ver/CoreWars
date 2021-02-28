using System.Collections.Generic;
using Akka.Actor;
using CoreWars.Competition;

namespace PrisonerDilemma
{
    public class DilemmaPlayer : IDilemmaPlayer
    {
        private readonly ICompetitionAgent _innerAgent;

        public DilemmaPlayer(ICompetitionAgent innerAgent)
        {
            _innerAgent = innerAgent;
        }

        public string Alias => _innerAgent.Alias;
        public string Author => _innerAgent.Author;
        public IActorRef Reference => _innerAgent.Reference;
        public int Score { get; set; } = 0;
        public List<PrisonerAction> ActionLog { get; } = new List<PrisonerAction>();
        public PrisonerAction CurrentAction { get; set; }
        public void ClearAction()
        {
            ActionLog.Add(CurrentAction);
            CurrentAction = PrisonerAction.NoAction;
        }
    }
}