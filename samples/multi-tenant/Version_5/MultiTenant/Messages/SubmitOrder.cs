using NServiceBus;

namespace Messages
{
    public class SubmitOrder : IMessage
    {
        public string OrderId { get; set; }
        public decimal OrderValue { get; set; }
    }

    public class ShipOrder : IMessage
    {
        public string OrderId { get; set; }
    }
}
