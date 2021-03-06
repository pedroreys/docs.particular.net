namespace Snippets5.Conventions
{
    using System;
    using NServiceBus;

    public class Usage
    {
        public Usage()
        {
            #region MessageConventions

            BusConfiguration busConfiguration = new BusConfiguration();
            ConventionsBuilder conventions = busConfiguration.Conventions();
            conventions.DefiningCommandsAs(t => t.Namespace != null && t.Namespace == "MyNamespace" && t.Namespace.EndsWith("Commands"));
            conventions.DefiningEventsAs(t => t.Namespace != null && t.Namespace == "MyNamespace" && t.Namespace.EndsWith("Events"));
            conventions.DefiningMessagesAs(t => t.Namespace != null && t.Namespace == "Messages");
            conventions.DefiningEncryptedPropertiesAs(p => p.Name.StartsWith("Encrypted"));
            conventions.DefiningDataBusPropertiesAs(p => p.Name.EndsWith("DataBus"));
            conventions.DefiningExpressMessagesAs(t => t.Name.EndsWith("Express"));
            conventions.DefiningTimeToBeReceivedAs(t => t.Name.EndsWith("Expires") ? TimeSpan.FromSeconds(30) : TimeSpan.MaxValue);

            #endregion
        }
    }
}