using System;
using AllureCSharpCommons.Events;

namespace MSTestAllureAdapter
{
    public class TestSuiteStartedWithTimeEvent : TestSuiteStartedEvent
    {
        public TestSuiteStartedWithTimeEvent(string uid, string name, DateTime started)
            : base(uid, name)
        {
            Started = started;
        }

        public DateTime Started { get; private set; }

        public override void Process(AllureCSharpCommons.AllureModel.testsuiteresult context)
        {
            base.Process(context);
            context.start = Started.ToUnixEpochTime();
        }
    }
}

