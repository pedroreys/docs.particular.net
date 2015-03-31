using System;
using Messages;
using NServiceBus;
using NServiceBus.Persistence.NHibernate;

namespace Server
{
    public class ShipOrderHandler : IHandleMessages<ShipOrder>
    {
        public NHibernateStorageContext StorageContext { get; set; }

        public void Handle(ShipOrder message)
        {
            var order = StorageContext.Session.Load<Order>(message.OrderId);

            Console.WriteLine("Order {0} worth {1} shipped.", order.OrderId, order.Value);
        }
    }
}