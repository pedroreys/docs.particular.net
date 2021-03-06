﻿namespace Snippets4.Sagas.Timeouts
{
    using System;
    using NServiceBus;
    using NServiceBus.Saga;

    #region saga-with-timeout

    public class MySaga : Saga<MySagaData>,
        IAmStartedByMessages<Message1>,
        IHandleMessages<Message2>,
        IHandleTimeouts<MyCustomTimeout>
    {
        public override void ConfigureHowToFindSaga()
        {
            ConfigureMapping<Message2>(m => m.SomeID)
                .ToSaga(s => s.SomeID);
        }

        public void Handle(Message1 message)
        {
            Data.SomeID = message.SomeID;
            RequestTimeout<MyCustomTimeout>(TimeSpan.FromHours(1));
        }

        public void Handle(Message2 message)
        {
            Data.Message2Arrived = true;
            ReplyToOriginator(new AlmostDoneMessage
            {
                SomeID = Data.SomeID
            });
        }

        public void Timeout(MyCustomTimeout state)
        {
            if (!Data.Message2Arrived)
            {
                ReplyToOriginator(new TiredOfWaitingForMessage2());
            }
        }
    }

    #endregion


}