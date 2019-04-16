using System.Collections.Generic;
using System.Linq;
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
            await StateManager.TryAddStateAsync(nameof(SurveyState), new SurveyState
            {
                Alternatives = alternatives,
                Votes = new List<string>(),
                Description = description,
                Title = title
            }, cancellationToken);
        }

        public async Task RegisterVote(string alternative, CancellationToken cancellationToken)
        {
            var state = await StateManager.GetStateAsync<SurveyState>(nameof(SurveyState), cancellationToken);
            if (state.Alternatives.Any(a => a == alternative))
            {
                state.Votes.Add(alternative);
            }
            await StateManager.AddOrUpdateStateAsync(nameof(SurveyState), state, (key, current) => state, cancellationToken);
        }

        public async Task<SurveyResults> GetResults(CancellationToken cancellationToken)
        {
            var state = await StateManager.GetStateAsync<SurveyState>(nameof(SurveyState), cancellationToken);
            var results = new SurveyResults
            {
                Title = state.Title,
                Description = state.Description,
                Results = state.Alternatives.ToDictionary(
                    alternative => alternative,
                    alternative => state.Votes.Count(vote => vote == alternative))
            };
            return results;
        }

        public async Task<SurveyDetails> GetDetails(CancellationToken cancellationToken)
        {
            var state = await StateManager.GetStateAsync<SurveyState>(nameof(SurveyState), cancellationToken);

            return new SurveyDetails
            {
                Alternatives = state.Alternatives,
                Description = state.Description,
                Title = state.Title
            };
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