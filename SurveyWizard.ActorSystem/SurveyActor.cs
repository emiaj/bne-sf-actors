using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using SurveyWizard.ActorSystem.Interfaces;

namespace SurveyWizard.ActorSystem
{
    public class SurveyActor : Actor, ISurveyActor
    {
        public SurveyActor(ActorService actorService, ActorId actorId) : base(actorService, actorId)
        {
        }

        public async Task UpdateDetails(string title, string description, string[] alternatives, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public async Task RegisterVote(string alternative, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public async Task<SurveyResults> GetResults(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public async Task<SurveyDetails> GetDetails(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }

    public class SurveyState
    {
        public SurveyState()
        {
            Votes = new List<string>();
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public string[] Alternatives { get; set; }
        public List<string> Votes { get; set; }
    }
}