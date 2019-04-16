using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using SurveyWizard.ActorSystem.Interfaces;

namespace SurveyWizard.ActorSystem
{
    [StatePersistence(StatePersistence.Persisted)]
    public class SurveyActor : Actor, ISurveyActor
    {
        public SurveyActor(ActorService actorService, ActorId actorId) : base(actorService, actorId)
        {
        }

        public async Task UpdateDetails(string title, string description, string[] alternatives, CancellationToken cancellationToken)
        {
            var state = new SurveyState
            {
                Alternatives = alternatives,
                Votes = new List<string>(),
                Description = description,
                Title = title
            };
            await StateManager.AddOrUpdateStateAsync(nameof(SurveyState), state, (key, current) => state, cancellationToken);
        }

        public async Task RegisterVote(string alternative, CancellationToken cancellationToken)
        {
            var state = await StateManager.TryGetStateAsync<SurveyState>(nameof(SurveyState), cancellationToken);
            if (state.HasValue)
            {
                if (state.Value.Alternatives.Any(a => a == alternative))
                {
                    state.Value.Votes.Add(alternative);
                }
                await StateManager.AddOrUpdateStateAsync(nameof(SurveyState), state.Value, (key, current) => state.Value, cancellationToken);
            }
        }

        public async Task<SurveyResults> GetResults(CancellationToken cancellationToken)
        {
            var state = await StateManager.TryGetStateAsync<SurveyState>(nameof(SurveyState), cancellationToken);
            if (state.HasValue)
            {
                var results = new SurveyResults
                {
                    Title = state.Value.Title,
                    Description = state.Value.Description,
                    Results = state.Value.Alternatives.ToDictionary(
                        alternative => alternative,
                        alternative => state.Value.Votes.Count(vote => vote == alternative))
                };
                return results;
            }

            return null;

        }

        public async Task<SurveyDetails> GetDetails(CancellationToken cancellationToken)
        {
            var state = await StateManager.TryGetStateAsync<SurveyState>(nameof(SurveyState), cancellationToken);
            if (state.HasValue)
            {
                return new SurveyDetails
                {
                    Alternatives = state.Value.Alternatives,
                    Description = state.Value.Description,
                    Title = state.Value.Title
                };
            }

            return null;
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