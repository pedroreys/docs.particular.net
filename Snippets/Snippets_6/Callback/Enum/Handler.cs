namespace Snippets5.Callback.Enum
{
    using NServiceBus;

    #region EnumCallbackResponse

    public class Handler : IHandleMessages<Message>
    {
        IBus bus;

        public Handler(IBus bus)
        {
            this.bus = bus;
        }

        public void Handle(Message message)
        {
            bus.Reply(Status.OK);
        }
    }

    #endregion
}
