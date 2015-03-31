using System;
using System.Linq;
using Messages;
using NServiceBus;

namespace Client
{
    class Program
    {
        const string Letters = "ABCDEFGHIJKLMNOPQRSTUVXYZ";
        private static readonly string[] TenantIds = {"A", "B", "C"};
        static readonly Random Random = new Random();

        static void Main(string[] args)
        {
            var busConfig = new BusConfiguration();
            busConfig.UseTransport<SqlServerTransport>();
            busConfig.UsePersistence<InMemoryPersistence>();
            busConfig.EnableOutbox();

            var bus = Bus.Create(busConfig).Start();

            while (true)
            {
                Console.WriteLine("Press <enter> to submit an order");
                Console.ReadLine();

                var tenantId = TenantIds[Random.Next(TenantIds.Length)];
                var orderId = new String(Enumerable.Range(0, 4).Select(x => Letters[Random.Next(Letters.Length)]).ToArray());
                var orderValue = Random.Next(100);

                Console.WriteLine("Submitting order {0} worth {1} to tenant {2}",orderId, orderValue, tenantId);

                bus.OutgoingHeaders["TenantId"] = tenantId;
                bus.Send(new SubmitOrder
                {
                    OrderId = orderId,
                    OrderValue = orderValue
                });
            }
        }
    }
}
