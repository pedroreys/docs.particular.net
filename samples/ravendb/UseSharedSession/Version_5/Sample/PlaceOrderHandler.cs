using System;
using NServiceBus;
using Raven.Client;
using NServiceBus.RavenDB.Persistence;

#region PlaceOrderHandler
public class PlaceOrderHandler : IHandleMessages<PlaceOrder>
{
    //public ISessionProvider SessionProvider { get; set; }
    readonly IDocumentSession Session;

    public PlaceOrderHandler(ISessionProvider sessionProvider)
    {
        Session = sessionProvider.Session;
    }

    //public PlaceOrderHandler(IDocumentSession session)
    //{
    //    Session = session; 
    //}

    public void Handle(PlaceOrder message)
    {
        Session.Store(new Order { OrderNumber = message.OrderNumber, OrderValue = message.OrderValue });

        Console.Out.WriteLine("Order {0} stored", message.OrderNumber);
    }
}
#endregion