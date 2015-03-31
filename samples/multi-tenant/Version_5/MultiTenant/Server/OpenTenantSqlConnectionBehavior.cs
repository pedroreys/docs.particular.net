using System;
using System.Configuration;
using System.Data.SqlClient;
using NServiceBus.Pipeline;
using NServiceBus.Pipeline.Contexts;
using NServiceBus.Settings;

namespace Server
{
    class OpenTenantSqlConnectionBehavior : IBehavior<IncomingContext>
    {
        public void Invoke(IncomingContext context, Action next)
        {
            var key = ConfigurationManager.ConnectionStrings["NServiceBus/Transport"].ConnectionString;

            string tenantId;
            if (context.PhysicalMessage.Headers.TryGetValue("TenantId", out tenantId))
            {
                var connectionString = ConfigurationManager.ConnectionStrings[tenantId].ConnectionString;
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    //HACK Substitute the transport connection with the new peristence-only connection
                    var connectionKey = string.Format("SqlConnection-{0}", key);
                    var outboxConnection = context.Get<SqlConnection>(connectionKey);
                    context.Set(connectionKey, connection);

                    next();

                    context.Set(connectionKey, outboxConnection);
                }
            }
            else
            {
                next();
            }
        }

        public class Registration : RegisterStep
        {
            public Registration()
                : base("OpenTenantSqlConnection", typeof(OpenTenantSqlConnectionBehavior), "Makes sure that there is an IDbConnection available on the pipeline")
            {
                InsertAfter("OutboxDeduplication");
                InsertBefore("OpenNHibernateSession");
            }
        }
    }
}