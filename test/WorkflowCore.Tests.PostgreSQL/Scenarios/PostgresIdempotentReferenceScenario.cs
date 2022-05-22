using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using WorkflowCore.IntegrationTests.Scenarios;
using WorkflowCore.Interface;
using WorkflowCore.Models;
using WorkflowCore.Persistence.EntityFramework.Exceptions;
using WorkflowCore.Persistence.EntityFramework.Interfaces;
using WorkflowCore.Testing;
using Xunit;

namespace WorkflowCore.Tests.PostgreSQL.Scenarios
{
    [Collection("Postgres collection")]
    public class PostgresIdempotentReferenceScenario : WorkflowTest<PostgresIdempotentReferenceScenario.IdempotentReferenceScenarioWorkflow, object>
    {
        public class IdempotentReferenceScenarioWorkflow : IWorkflow
        {
            public string Id => "CorrelationIdWorkflow";
            public int Version => 1;
            public void Build(IWorkflowBuilder<object> builder)
            {
                builder
                    .StartWith(context => ExecutionResult.Next());
            }
        }

        protected override void ConfigureServices(IServiceCollection services)
        {
            services.AddWorkflow(x => x.UsePostgreSQL(PostgresDockerSetup.ScenarioConnectionString, true, true));
        }

        public PostgresIdempotentReferenceScenario()
        {
            Setup();
        }

        [Fact]
        public async Task Scenario()
        {
            var reference = Guid.NewGuid().ToString();
            var def = new IdempotentReferenceScenarioWorkflow();
            var workflowId = await Host.StartWorkflow(def.Id, reference: reference);

            Func<Task> action1 = () => Host.StartWorkflow(def.Id, reference: reference);

            WaitForWorkflowToComplete(workflowId, TimeSpan.FromSeconds(30));

            var wf = await ((IExtendedPersistenceProvider)PersistenceProvider).GetWorkflowInstanceByReference(reference);

            GetStatus(workflowId).Should().Be(WorkflowStatus.Complete);
            UnhandledStepErrors.Count.Should().Be(0);

            wf.Should().NotBeNull();
            wf.Id.Should().Be(workflowId);

            action1.ShouldThrow<WorkflowExistsException>();
        }
    }
}