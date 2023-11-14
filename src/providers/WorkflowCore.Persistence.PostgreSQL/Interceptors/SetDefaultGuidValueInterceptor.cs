using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WorkflowCore.Persistence.EntityFramework.Models;

namespace WorkflowCore.Persistence.PostgreSQL.Interceptors
{
    /// <summary>
    /// Database doesn't override value if it is not default(Guid.Empty for Guid, 0 for int...)
    /// So we need to set value to default to get sequential UUID 
    /// </summary>
    public class SetDefaultGuidValueInterceptor : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            var entries = eventData.Context.ChangeTracker
                .Entries().Where(x => x.State == Microsoft.EntityFrameworkCore.EntityState.Added)
                .ToList();

            foreach (var entry in entries) 
            {
                if(entry.Entity is PersistedWorkflow workflow)
                    workflow.InstanceId = Guid.Empty;

                if (entry.Entity is PersistedEvent @event)
                    @event.EventId = Guid.Empty;

                if (entry.Entity is PersistedSubscription subscription)
                    subscription.SubscriptionId = Guid.Empty;
            }

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
