using System;
using NServiceBus;
using Raven.Client;
using Raven.Client.Document;
using NServiceBus.Persistence;

static class Program
{

    static IDocumentStore CreateDocumentStore()
    {
        return new DocumentStore
                        {
                            Url = "http://localhost:8080",
                            DefaultDatabase = "Samples.UseSharedSession"
                        }
                        .Initialize();
    }

    static void Main()
    {
        Console.Title = "Sample process";
        BusConfiguration busConfiguration = new BusConfiguration();
        busConfiguration.EndpointName("Samples.UseSharedSession");

        var store = CreateDocumentStore();

        busConfiguration
            .UsePersistence<RavenDBPersistence>()
                //.SetDefaultDocumentStore(store)
                .UseSharedSession(() =>
                {
                    Console.WriteLine("Open shared session...");
                    IDocumentSession session = store.OpenSession();
                    // customize the session properties here
                    return session;
                })
                .DoNotSetupDatabasePermissions()
                ;

        busConfiguration.EnableInstallers();

        int orderNumber = 1;

        using (IBus bus = Bus.Create(busConfiguration).Start())
        {
            Console.Out.WriteLine("Press any key to send a message. Press `q` to quit");

            Console.Out.WriteLine("After storing a few orders you can open a browser and view them at http://localhost:32076/studio/index.html#databases/documents?collection=Orders&database=Samples.UoWWithChildContainers");

            while (Console.ReadKey().ToString() != "q")
            {
                bus.SendLocal(new PlaceOrder
                {
                    OrderNumber = string.Format("Order-{0}", orderNumber),
                    OrderValue = 100
                });

                orderNumber++;
            }
        }
    }
}
