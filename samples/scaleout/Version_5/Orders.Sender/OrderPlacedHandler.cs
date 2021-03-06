﻿namespace Orders.Sender
{
    using System;
    using NServiceBus;
    using Orders.Events;

    #region sender-event-handler

    public class OrderPlacedHandler : IHandleMessages<OrderPlaced>
    {
        public void Handle(OrderPlaced orderPlaced)
        {
            Console.WriteLine("Received Event OrderPlaced for orderId: " + orderPlaced.OrderId);
        }
    }

    #endregion

}