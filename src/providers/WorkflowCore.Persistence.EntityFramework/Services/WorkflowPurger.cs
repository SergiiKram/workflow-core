using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WorkflowCore.Interface;
using WorkflowCore.Models;
using WorkflowCore.Persistence.EntityFramework.Interfaces;
using WorkflowCore.Persistence.EntityFramework.Models;

namespace WorkflowCore.Persistence.EntityFramework.Services
{
    public class WorkflowPurger : IWorkflowPurger
    {
        private readonly IWorkflowDbContextFactory _contextFactory;
        private readonly WorkflowsPurgerOptions _options;

        public WorkflowPurger(IWorkflowDbContextFactory contextFactory, WorkflowsPurgerOptions options)
        {
            _contextFactory = contextFactory;
            _options = options;
        }

        public async Task PurgeWorkflows(WorkflowStatus status, DateTime olderThan, CancellationToken cancellationToken = default)
        {
            var olderThanUtc = olderThan.ToUniversalTime();
            using (var db = ConstructDbContext())
            {
                int deleteEvents = _options.BatchSize;
                db.Database.SetCommandTimeout(_options.DeleteCommandTimeoutSeconds);

                while(deleteEvents != 0)
                {
                    deleteEvents = await db.Set<PersistedWorkflow>()
                        .Where(x => x.Status == status && x.CompleteTime < olderThanUtc)
                        .Take(_options.BatchSize)
                        .ExecuteDeleteAsync(cancellationToken);
                }
            }
        }
        
        
        private WorkflowDbContext ConstructDbContext()
        {
            return _contextFactory.Build();
        }
    }
}