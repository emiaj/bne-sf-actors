using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SurveyWizard.Web.Controllers
{
    [Route("api/[controller]")]
    public class SurveyController : Controller
    {
        [Route("[action]")]
        [HttpGet]
        public async Task List(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        [Route("")]
        [HttpPost]
        public async Task<Guid> Create([FromBody] CreateSurveyModel model, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            return Guid.NewGuid();
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<SurveyDetails> Details(Guid id, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            return new SurveyDetails();
        }

        [Route("{id}")]
        [HttpDelete]
        public async Task Remove(Guid id, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        [Route("{id}/[action]")]
        [HttpPost]
        public async Task Vote(Guid id, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        [Route("{id}/results")]
        [HttpGet]
        public async Task<SurveyResults> Results(Guid id, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            return new SurveyResults();
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

    public class SurveyDetails
    {
        public SurveyDetails()
        {
            Alternatives = new AlternativeModel[0];
        }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public AlternativeModel[] Alternatives { get; set; }
    }

    public class SurveyResults
    {
        public SurveyResults()
        {
            Results = new Dictionary<string, int>();
        }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Dictionary<string, int> Results { get; set; }
    }
}