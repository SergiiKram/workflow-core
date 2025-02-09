﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Npgsql;
using System;
using System.Threading;
using System.Threading.Tasks;
using WorkflowCore.Exceptions;
using WorkflowCore.Persistence.EntityFramework.Models;
using WorkflowCore.Persistence.EntityFramework.Services;

namespace WorkflowCore.Persistence.PostgreSQL
{
    public class PostgresContext : WorkflowDbContext
    {
        private readonly string _connectionString;
        private readonly string _schemaName;

        public PostgresContext(string connectionString,string schemaName)
            :base()
        {   
            _connectionString = connectionString;
            _schemaName = schemaName;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseNpgsql(_connectionString);
        }

        protected override void ConfigureSubscriptionStorage(EntityTypeBuilder<PersistedSubscription> builder)
        {
            builder.ToTable("Subscription", _schemaName);
            builder.Property(x => x.PersistenceId).ValueGeneratedOnAdd();
        }

        protected override void ConfigureWorkflowStorage(EntityTypeBuilder<PersistedWorkflow> builder)
        {
            builder.ToTable("Workflow", _schemaName);
            builder.Property(x => x.PersistenceId).ValueGeneratedOnAdd();
        }
                
        protected override void ConfigureExecutionPointerStorage(EntityTypeBuilder<PersistedExecutionPointer> builder)
        {
            builder.ToTable("ExecutionPointer", _schemaName);
            builder.Property(x => x.PersistenceId).ValueGeneratedOnAdd();
        }

        protected override void ConfigureExecutionErrorStorage(EntityTypeBuilder<PersistedExecutionError> builder)
        {
            builder.ToTable("ExecutionError", _schemaName);
            builder.Property(x => x.PersistenceId).ValueGeneratedOnAdd();
        }

        protected override void ConfigureExetensionAttributeStorage(EntityTypeBuilder<PersistedExtensionAttribute> builder)
        {
            builder.ToTable("ExtensionAttribute", _schemaName);
            builder.Property(x => x.PersistenceId).ValueGeneratedOnAdd();
        }

        protected override void ConfigureEventStorage(EntityTypeBuilder<PersistedEvent> builder)
        {
            builder.ToTable("Event", _schemaName);
            builder.Property(x => x.PersistenceId).ValueGeneratedOnAdd();
        }

        protected override void ConfigureScheduledCommandStorage(EntityTypeBuilder<PersistedScheduledCommand> builder)
        {
            builder.ToTable("ScheduledCommand", _schemaName);
            builder.Property(x => x.PersistenceId).ValueGeneratedOnAdd();
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            }
            catch (DbUpdateException e)
                when (e.InnerException is PostgresException pe)
            {
                if (pe.Message.Contains("Reference", StringComparison.OrdinalIgnoreCase)
                    && pe.Message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase))
                {
                    throw new WorkflowExistsException(pe);
                }

                throw;
            }
        }
    }
}

