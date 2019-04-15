using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace SurveyWizard.ActorSystem.Interfaces
{
    public interface ISurveyActor : IActor
    {
        Task UpdateDetails(string title, string description, string[] alternatives, CancellationToken cancellationToken);

        Task RegisterVote(string alternative, CancellationToken cancellationToken);

        Task<SurveyResults> GetResults(CancellationToken cancellationToken);

        Task<SurveyDetails> GetDetails(CancellationToken cancellationToken);
    }

    public class SurveyResults
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Dictionary<string, int> Results { get; set; }
    }

    public class SurveyDetails
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string[] Alternatives { get; set; }
    }
}