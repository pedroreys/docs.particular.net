using System;
using Messages;
using NServiceBus;

namespace Client
{
    public class OrderAcceptedHandler : IHandleMessages<OrderAccepted>
    {
        public IBus Bus { get; set; }

        public void Handle(OrderAccepted message)
        {
            Console.WriteLine("Order {0} accepted. Shipping...", message.OrderId);
            Bus.OutgoingHeaders["TenantId"] = message.TenantId;
            Bus.Send(new ShipOrder
            {
                OrderId = message.OrderId
            });
        }
    }
}