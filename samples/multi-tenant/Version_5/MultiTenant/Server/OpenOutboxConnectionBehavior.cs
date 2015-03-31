using System;
using System.Configuration;
using System.Data.SqlClient;
using NServiceBus.Pipeline;
using NServiceBus.Pipeline.Contexts;

namespace Server
{
    internal class OpenOutboxConnectionBehavior : IBehavior<IncomingContext>
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
                    context.Set(string.Format("SqlConnection-{0}", key), connection);

                    next();
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
                : base("OpenOutboxConnection", typeof(OpenOutboxConnectionBehavior), "Makes sure that there is an IDbConnection available on the pipeline")
            {
                InsertBefore("OutboxDeduplication");
            }
        }
    }
}