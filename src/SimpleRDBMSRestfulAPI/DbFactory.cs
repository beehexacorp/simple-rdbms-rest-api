using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using hexasync.common;
using hexasync.domain.managers;
using Microsoft.Extensions.Logging;
using hexasync.domain.profile.models;
using hexasync.domain.cluster_management.DTO;

namespace SimpleRDBMSRestfulAPI
{
    public sealed class DbFactory : BaseDbFactory
    {
        public DbFactory(IConnectionStringReader connectionStringReader, ILoggerFactory loggerFactory) 
            : base(connectionStringReader, loggerFactory)
        {
        }

        public override async Task<IDbConnection> CreateWorkerConnectionAsync(Guid profileId, DatabaseQueryType type = DatabaseQueryType.DML_READ, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("CreateWorkerConnectionAsync is not implemented yet.");
        }

        public override async Task Reconcile(Guid profileId, ProfileDatabaseInformationResponse? info = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("Reconcile is not implemented yet.");
        }
    }
}