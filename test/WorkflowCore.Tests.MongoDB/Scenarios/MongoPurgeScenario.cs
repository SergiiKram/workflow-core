﻿using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using WorkflowCore.IntegrationTests.Scenarios;
using Xunit;

namespace WorkflowCore.Tests.MongoDB.Scenarios
{
    [Collection("Mongo collection")]
    public class MongoPurgeScenario : WorkflowPurgeScenario
    {
        protected override void ConfigureServices(IServiceCollection services)
        {
            services.AddWorkflow(x => x.UseMongoDB(MongoDockerSetup.ConnectionString, nameof(MongoRetrySagaScenario)));
        }

        [Fact]
        public Task RunAsync()
        {
            return ScenarioAsync();
        }
    }
}