using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using WorkflowCore.Interface;
using WorkflowCore.Models;
using WorkflowCore.TestAssets;
using Xunit;

namespace WorkflowCore.UnitTests
{
    public abstract class BasePersistenceFixture
    {
        protected abstract IPersistenceProvider Subject { get; }

        [Fact]
        public void CreateNewWorkflow_should_generate_id()
        {
            var workflow = new WorkflowInstance
            {
                Data = new { Value1 = 7 },
                Description = "My Description",
                Status = WorkflowStatus.Runnable,
                NextExecution = 0,
                Version = 1,
                WorkflowDefinitionId = "My Workflow"
            };
            workflow.ExecutionPointers.Add(new ExecutionPointer
            {
                Id = Guid.NewGuid().ToString(),
                Active = true,
                StepId = 0
            });

            var workflowId = Subject.CreateNewWorkflow(workflow).Result;

            workflowId.Should().NotBeNull();
            workflow.Id.Should().NotBeNull();
            Guid.Parse(workflowId).Should().NotBeEmpty(); 
        }

        [Fact]
        public void CreateNewWorkflow_should_create_duplicates_without_reference()
        {
            var workflow = new WorkflowInstance
            {
                Data = new { Value1 = 7 },
                Description = "My Description",
                Status = WorkflowStatus.Runnable,
                NextExecution = 0,
                Version = 1,
                WorkflowDefinitionId = "My Workflow"
            };
            workflow.ExecutionPointers.Add(new ExecutionPointer
            {
                Id = Guid.NewGuid().ToString(),
                Active = true,
                StepId = 0
            });

            var workflowId1 = Subject.CreateNewWorkflow(workflow).Result;

            workflow.Id = null;

            var workflowId2 = Subject.CreateNewWorkflow(workflow).Result;

            workflowId1.Should().NotBeNull();
            workflowId2.Should().NotBeNull();
            workflowId2.Should().NotBe(workflowId1);
        }

        [Fact]
        public void CreateNewWorkflow_with_duplicate_reference_should_fail()
        {
            var workflow = new WorkflowInstance
            {
                Data = new { Value1 = 7 },
                Description = "My Description",
                Status = WorkflowStatus.Runnable,
                NextExecution = 0,
                Version = 1,
                WorkflowDefinitionId = "My Workflow",
                Reference = ""
            };
            workflow.ExecutionPointers.Add(new ExecutionPointer
            {
                Id = Guid.NewGuid().ToString(),
                Active = true,
                StepId = 0
            });

            Func<Task> action1 = () => Subject.CreateNewWorkflow(workflow);

            action1.ShouldNotThrow();

            workflow.Id = null;

            Func<Task> action2 = () => Subject.CreateNewWorkflow(workflow);

            action2.ShouldThrow<Exception>();
        }

        [Fact]
        public void GetWorkflowInstance_should_retrieve_workflow()
        {
            var workflow = new WorkflowInstance
            {
                Data = new TestData { Value1 = 7 },
                Description = "My Description",
                Status = WorkflowStatus.Runnable,
                NextExecution = 0,
                Version = 1,
                WorkflowDefinitionId = "My Workflow",
                Reference = Guid.NewGuid().ToString()
            };
            workflow.ExecutionPointers.Add(new ExecutionPointer
            {
                Id = "1",
                Active = true,
                StepId = 0,
                SleepUntil = new DateTime(2000, 1, 1).ToUniversalTime(),
                Scope = new List<string> { "4", "3", "2", "1" }
            });
            var workflowId = Subject.CreateNewWorkflow(workflow).Result;

            var retrievedWorkflow = Subject.GetWorkflowInstance(workflowId).Result;

            retrievedWorkflow.ShouldBeEquivalentTo(workflow);
            retrievedWorkflow.ExecutionPointers.FindById("1")
                .Scope.Should().ContainInOrder(workflow.ExecutionPointers.FindById("1").Scope);
        }

        [Fact]
        public void GetWorkflowInstances_should_retrieve_workflows()
        {
            var workflow01 = new WorkflowInstance
            {
                Data = new TestData { Value1 = 7 },
                Description = "My Description",
                Status = WorkflowStatus.Runnable,
                NextExecution = 0,
                Version = 1,
                WorkflowDefinitionId = "My Workflow",
                Reference = Guid.NewGuid().ToString()
            };
            workflow01.ExecutionPointers.Add(new ExecutionPointer
            {
                Id = "1",
                Active = true,
                StepId = 0,
                SleepUntil = new DateTime(2000, 1, 1).ToUniversalTime(),
                Scope = new List<string> { "4", "3", "2", "1" }
            });
            var workflowId01 = Subject.CreateNewWorkflow(workflow01).Result;

            var workflow02 = new WorkflowInstance
            {
                Data = new TestData { Value1 = 7 },
                Description = "My Description",
                Status = WorkflowStatus.Runnable,
                NextExecution = 0,
                Version = 1,
                WorkflowDefinitionId = "My Workflow",
                Reference = Guid.NewGuid().ToString()
            };
            workflow02.ExecutionPointers.Add(new ExecutionPointer
            {
                Id = "1",
                Active = true,
                StepId = 0,
                SleepUntil = new DateTime(2000, 1, 1).ToUniversalTime(),
                Scope = new List<string> { "4", "3", "2", "1" }
            });
            var workflowId02 = Subject.CreateNewWorkflow(workflow02).Result;

            var workflow03 = new WorkflowInstance
            {
                Data = new TestData { Value1 = 7 },
                Description = "My Description",
                Status = WorkflowStatus.Runnable,
                NextExecution = 0,
                Version = 1,
                WorkflowDefinitionId = "My Workflow",
                Reference = Guid.NewGuid().ToString()
            };
            workflow03.ExecutionPointers.Add(new ExecutionPointer
            {
                Id = "1",
                Active = true,
                StepId = 0,
                SleepUntil = new DateTime(2000, 1, 1).ToUniversalTime(),
                Scope = new List<string> { "4", "3", "2", "1" }
            });
            var workflowId03 = Subject.CreateNewWorkflow(workflow03).Result;

            var retrievedWorkflows = Subject.GetWorkflowInstances(new[] { workflowId01, workflowId02, workflowId03 }).Result;

            retrievedWorkflows.Count().ShouldBeEquivalentTo(3);

            var retrievedWorkflow01 = retrievedWorkflows.Single(o => o.Id == workflowId01);
            retrievedWorkflow01.ShouldBeEquivalentTo(workflow01);
            retrievedWorkflow01.ExecutionPointers.FindById("1")
                .Scope.Should().ContainInOrder(workflow01.ExecutionPointers.FindById("1").Scope);

            var retrievedWorkflow02 = retrievedWorkflows.Single(o => o.Id == workflowId02);
            retrievedWorkflow02.ShouldBeEquivalentTo(workflow02);
            retrievedWorkflow02.ExecutionPointers.FindById("1")
                .Scope.Should().ContainInOrder(workflow02.ExecutionPointers.FindById("1").Scope);

            var retrievedWorkflow03 = retrievedWorkflows.Single(o => o.Id == workflowId03);
            retrievedWorkflow03.ShouldBeEquivalentTo(workflow03);
            retrievedWorkflow03.ExecutionPointers.FindById("1")
                .Scope.Should().ContainInOrder(workflow03.ExecutionPointers.FindById("1").Scope);
        }

        [Fact]
        public void PersistWorkflow()
        {
            var oldWorkflow = new WorkflowInstance
            {
                Data = new TestData { Value1 = 7 },
                Description = "My Description",
                Status = WorkflowStatus.Runnable,
                NextExecution = 0,
                Version = 1,
                WorkflowDefinitionId = "My Workflow",
                CreateTime = new DateTime(2000, 1, 1).ToUniversalTime(),
                Reference = Guid.NewGuid().ToString()
            };
            oldWorkflow.ExecutionPointers.Add(new ExecutionPointer
            {
                Id = Guid.NewGuid().ToString(),
                Active = true,
                StepId = 0,
                Scope = new List<string> { "1", "2", "3", "4" }
            });
            var workflowId = Subject.CreateNewWorkflow(oldWorkflow).Result;
            var newWorkflow = Utils.DeepCopy(oldWorkflow);
            newWorkflow.Data = oldWorkflow.Data;
            newWorkflow.Reference = oldWorkflow.Reference;
            newWorkflow.NextExecution = 7;
            newWorkflow.ExecutionPointers.Add(new ExecutionPointer { Id = Guid.NewGuid().ToString(), Active = true, StepId = 1 });

            Subject.PersistWorkflow(newWorkflow).Wait();

            var current = Subject.GetWorkflowInstance(workflowId).Result;
            current.ShouldBeEquivalentTo(newWorkflow);
        }
		
		[Fact]
        public void PersistWorkflow_with_subscriptions()
        {
            var workflow = new WorkflowInstance
            {
                Data = new TestData { Value1 = 7 },
                Description = "My Description",
                Status = WorkflowStatus.Runnable,
                NextExecution = 0,
                Version = 1,
                WorkflowDefinitionId = "My Workflow",
                CreateTime = new DateTime(2000, 1, 1).ToUniversalTime(),
                ExecutionPointers = new ExecutionPointerCollection(),
                Reference = Guid.NewGuid().ToString()
            };

            workflow.ExecutionPointers.Add(new ExecutionPointer
            {
                Id = Guid.NewGuid().ToString(),
                Active = true,
                StepId = 0,
                Scope = new List<string> { "1", "2", "3", "4" },
                EventName = "Event1"
            });

            workflow.ExecutionPointers.Add(new ExecutionPointer
            {
                Id = Guid.NewGuid().ToString(),
                Active = true,
                StepId = 1,
                Scope = new List<string> { "1", "2", "3", "4" },
                EventName = "Event2",
            });

            var workflowId = Subject.CreateNewWorkflow(workflow).Result;
            workflow.NextExecution = 0;

            List<EventSubscription> subscriptions = new List<EventSubscription>();
            foreach (var pointer in workflow.ExecutionPointers)
            {
                var subscription = new EventSubscription()
                {
                    WorkflowId = workflowId,
                    StepId = pointer.StepId,
                    ExecutionPointerId = pointer.Id,
                    EventName = pointer.EventName,
                    EventKey = workflowId,
                    SubscribeAsOf = DateTime.UtcNow,
                    SubscriptionData = "data"
                };

                subscriptions.Add(subscription);
            }

            Subject.PersistWorkflow(workflow, subscriptions).Wait();

            var current = Subject.GetWorkflowInstance(workflowId).Result;
            current.ShouldBeEquivalentTo(workflow);

            foreach (var pointer in workflow.ExecutionPointers)
            {
                subscriptions = Subject.GetSubscriptions(pointer.EventName, workflowId, DateTime.UtcNow).Result.ToList();
                subscriptions.Should().HaveCount(1);
                foreach (var subscription in subscriptions)
                {
                    Guid.Parse(subscription.Id).Should().NotBeEmpty();
                }
            }
        }

        [Fact]
        public void ConcurrentPersistWorkflow()
        {
            var subject = Subject; // Don't initialize in the thread.

            var actions = new List<Action>();

            for (int i = 0; i < 30; i++)
            {
                actions.Add(() =>
                {
                    var oldWorkflow = new WorkflowInstance
                    {
                        Data = new TestData { Value1 = 7 },
                        Description = "My Description",
                        Status = WorkflowStatus.Runnable,
                        NextExecution = 0,
                        Version = 1,
                        WorkflowDefinitionId = "My Workflow",
                        CreateTime = new DateTime(2000, 1, 1).ToUniversalTime()
                    };
                    oldWorkflow.ExecutionPointers.Add(new ExecutionPointer
                    {
                        Id = Guid.NewGuid().ToString(),
                        Active = true,
                        StepId = 0
                    });
                    var workflowId = subject.CreateNewWorkflow(oldWorkflow).Result;
                    var newWorkflow = Utils.DeepCopy(oldWorkflow);
                    newWorkflow.NextExecution = 7;
                    newWorkflow.ExecutionPointers.Add(new ExecutionPointer { Id = Guid.NewGuid().ToString(), Active = true, StepId = 1 });

                    subject.PersistWorkflow(newWorkflow).Wait(); // It will throw an exception if the persistence provider occurred resource competition.
                });
            }

            Parallel.ForEach(actions, action =>
            {
                action.ShouldNotThrow<InvalidOperationException>();
            });
        }

        [Fact]
        public void CreateNewEvent()
        {
            var @event = new Event
            {
                EventName = "Event",
                EventKey = "EventKey",
                EventData = "EventData",
                EventTime = DateTime.UtcNow,
                IsProcessed = true
            };

            var eventId = Subject.CreateEvent(@event).Result;

            eventId.Should().NotBeNull();
            Guid.Parse(eventId).Should().NotBeEmpty();
        }
    }

    public class TestData
    {
        public int Value1 { get; set; }
    }
}