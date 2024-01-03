using WorkflowCore.Interface;
using WorkflowCore.Models;
using WorkflowCore.Persistence.EntityFramework.Interfaces;
using WorkflowCore.Persistence.EntityFramework.Services;
using WorkflowCore.Persistence.PostgreSQL;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static WorkflowOptions UsePostgreSQL(this WorkflowOptions options,
            string connectionString, bool canCreateDB, bool canMigrateDB, string schemaName="wfc")
        {
            static EntityFrameworkPersistenceProvider GetPersistenceFactory(string connectionString, bool canCreateDB, bool canMigrateDB, string schemaName)
            {
                return new EntityFrameworkPersistenceProvider(new PostgresContextFactory(connectionString, schemaName), canCreateDB, canMigrateDB);
            }

            options.UsePersistence(sp => GetPersistenceFactory(connectionString, canCreateDB, canMigrateDB, schemaName));
            options.Services.AddTransient<IWorkflowPurger>(sp => new WorkflowPurger(new PostgresContextFactory(connectionString, schemaName), options.WorkflowsPurgerOptions));
            options.Services.AddTransient<IEventsPurger>(sp => new EventsPurger(new PostgresContextFactory(connectionString, schemaName), options.EventsPurgerOptions));
            options.Services.AddTransient<IExtendedPersistenceProvider>(sp => GetPersistenceFactory(connectionString, canCreateDB, canMigrateDB, schemaName));
            return options;
        }
    }
}
