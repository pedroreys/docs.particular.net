using System;
using Messages;
using NServiceBus;
using NServiceBus.Persistence.NHibernate;

namespace Server
{
    public class SubmitOrderHandler : IHandleMessages<SubmitOrder>
    {
        static readonly Random Random = new Random();

        public IBus Bus { get; set; }
        public NHibernateStorageContext StorageContext { get; set; }

        public void Handle(SubmitOrder message)
        {
            StorageContext.Session.Save(new Order
            {
                OrderId = message.OrderId,
                Value = message.OrderValue
            });
            if (Random.Next(4) == 0)
            {
                throw new Exception("After saving order " + message.OrderId);
            }
            Bus.Reply(new OrderAccepted
            {
                OrderId = message.OrderId,
                TenantId = Bus.GetMessageHeader(message,"TenantId")
            });
            if (Random.Next(4) == 0)
            {
                throw new Exception("After sending reply for order " + message.OrderId);
            }
            Console.WriteLine("Order {0} accepted.", message.OrderId);
        }
    }
}