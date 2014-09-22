using System;
using AllureCSharpCommons.Events;

namespace MSTestAllureAdapter
{
    public class TestCaseFailureWithTimeEvent : TestCaseFailureEvent
    {
        public TestCaseFailureWithTimeEvent(DateTime finished)
        {
            Finished = finished;
        }

        public DateTime Finished { get; private set; }

        public override void Process(AllureCSharpCommons.AllureModel.testcaseresult context)
        {
            base.Process(context);
            //context.status = AllureCSharpCommons.AllureModel.status.failed;
        }
    }
}

