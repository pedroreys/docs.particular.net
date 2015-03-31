using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;
using NServiceBus;
using NServiceBus.Persistence;
using Configuration = NHibernate.Cfg.Configuration;

namespace Server
{
    class Program
    {
        private static readonly string[] TenantIds = { "A", "B", "C" };

        static void Main(string[] args)
        {
            var hibernateConfig = new Configuration();
            hibernateConfig.DataBaseIntegration(x =>
            {
                x.ConnectionStringName = "NServiceBus/Persistence";
                x.Dialect<MsSql2012Dialect>();
            });
            var mapper = new ModelMapper();
            mapper.AddMapping<OrderMap>();
            hibernateConfig.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());

            var busConfig = new BusConfiguration();
            busConfig.UseTransport<SqlServerTransport>();
            busConfig.UsePersistence<NHibernatePersistence>().UseConfiguration(hibernateConfig).DisableSchemaUpdate();
            busConfig.EnableOutbox();
            busConfig.Pipeline.Register<OpenTenantSqlConnectionBehavior.Registration>();
            busConfig.Pipeline.Register<OpenOutboxConnectionBehavior.Registration>();

            using (Bus.Create(busConfig).Start())
            {
                var schemaExport = new SchemaExport(hibernateConfig);
                foreach (var tenantId in TenantIds)
                {
                    var connectionString = ConfigurationManager.ConnectionStrings[tenantId].ConnectionString;
                    using (var connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        schemaExport.Execute(false, true, false, connection, new StringWriter());
                    }
                }

                Console.WriteLine("Press <enter> to exit.");
                Console.ReadLine();
            }
        }
    }
}
