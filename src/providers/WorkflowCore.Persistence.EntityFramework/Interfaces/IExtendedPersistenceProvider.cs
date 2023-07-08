using System.Threading;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace WorkflowCore.Persistence.EntityFramework.Interfaces
{
    public interface IExtendedPersistenceProvider: IPersistenceProvider
    {
        Task<WorkflowInstance> GetWorkflowInstanceByReference(string reference, CancellationToken cancellationToken = default);
    }
}
