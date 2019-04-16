using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Health;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Query;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using SurveyWizard.ActorSystem.Interfaces;

namespace SurveyWizard.Web.Controllers
{
    [Route("api/[controller]")]
    public class SurveyController : Controller
    {
        [Route("[action]")]
        [HttpGet]
        public async Task<SurveySummaryModel[]> List(CancellationToken cancellationToken)
        {
            var serviceName = new Uri("fabric:/SurveyWizard/SurveyActorService");
            using (var fabricClient = new FabricClient())
            {
                var partitionList = await fabricClient.QueryManager.GetPartitionListAsync(serviceName);

                var activeActors = new List<ActorInformation>();
                foreach (var partition in partitionList)
                {
                    if (partition.HealthState != HealthState.Ok)
                    {
                        continue;
                    }

                    var key = (Int64RangePartitionInformation) partition.PartitionInformation;

                    var actorServiceProxy = ServiceProxy.Create<IActorService>(serviceName, new ServicePartitionKey(key.LowKey));
                    ContinuationToken continuationToken = null;
                    do
                    {
                        var page = await actorServiceProxy.GetActorsAsync(continuationToken, cancellationToken);
                        activeActors.AddRange(page.Items);

                        continuationToken = page.ContinuationToken;
                    } while (continuationToken != null && activeActors.Count < 10);
                }

                var surveys = new List<SurveySummaryModel>();

                foreach (var actorInformation in activeActors)
                {
                    var surveyActor = ActorProxy.Create<ISurveyActor>(actorInformation.ActorId);
                    try
                    {
                        var details = await surveyActor.GetDetails(cancellationToken);
                        if (details == null)
                        {
                            continue;
                        }
                        surveys.Add(new SurveySummaryModel
                        {
                            Id = actorInformation.ActorId.GetGuidId(),
                            Description = details.Description,
                            Title = details.Title
                        });
                    }
                    catch (KeyNotFoundException)
                    {
                    }
                   
                }


                return surveys.ToArray();
            }
        }

        [Route("")]
        [HttpPost]
        public async Task<Guid> Create([FromBody] CreateSurveyModel model, CancellationToken cancellationToken)
        {
            var id = Guid.NewGuid();
            var surveyActor = ActorProxy.Create<ISurveyActor>(new ActorId(id));
            await surveyActor.UpdateDetails(model.Title, model.Description,
                model.Alternatives.Select(a => a.Value).ToArray(), cancellationToken);
            return id;
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<SurveyDetailsModel> Details(Guid id, CancellationToken cancellationToken)
        {
            var surveyActor = ActorProxy.Create<ISurveyActor>(new ActorId(id));

            var details = await surveyActor.GetDetails(cancellationToken);

            return new SurveyDetailsModel
            {
                Description = details.Description,
                Title = details.Title,
                Alternatives = details.Alternatives.Select(a => new AlternativeModel {Value = a}).ToArray(),
                Id = id
            };
        }

        [Route("{id}")]
        [HttpDelete]
        public async Task Remove(Guid id, CancellationToken cancellationToken)
        {
            var serviceName = new Uri("fabric:/SurveyWizard/SurveyActorService");
            var surveyActor = new ActorId(id);
            var actorServiceProxy = ActorServiceProxy.Create(serviceName, surveyActor);

            await actorServiceProxy.DeleteActorAsync(surveyActor, cancellationToken);
        }

        [Route("{id}/[action]")]
        [HttpPost]
        public async Task Vote(Guid id, [FromBody] VoteModel request, CancellationToken cancellationToken)
        {
            var surveyActor = ActorProxy.Create<ISurveyActor>(new ActorId(id));
            await surveyActor.RegisterVote(request.Vote.Value, cancellationToken);
        }

        [Route("{id}/results")]
        [HttpGet]
        public async Task<SurveyResultsModel> Results(Guid id, CancellationToken cancellationToken)
        {
            var surveyActor = ActorProxy.Create<ISurveyActor>(new ActorId(id));
            var results = await surveyActor.GetResults(cancellationToken);
            return new SurveyResultsModel
            {
                Description = results.Description,
                Title = results.Title,
                Id = id,
                Results = results.Results
            };
        }
    }

    public class AlternativeModel
    {
        public string Value { get; set; }
    }

    public class CreateSurveyModel
    {
        public CreateSurveyModel()
        {
            Alternatives = new AlternativeModel[0];
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public AlternativeModel[] Alternatives { get; set; }
    }

    public class VoteModel
    {
        public Guid Id { get; set; }
        public AlternativeModel Vote { get; set; }
    }


    public class SurveySummaryModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }


    public class SurveyDetailsModel
    {
        public SurveyDetailsModel()
        {
            Alternatives = new AlternativeModel[0];
        }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public AlternativeModel[] Alternatives { get; set; }
    }

    public class SurveyResultsModel
    {
        public SurveyResultsModel()
        {
            Results = new Dictionary<string, int>();
        }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Dictionary<string, int> Results { get; set; }
    }
}