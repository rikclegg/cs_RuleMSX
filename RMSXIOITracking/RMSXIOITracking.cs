using System;
using com.bloomberg.samples.rulemsx;
using com.bloomberg.ioiapi.samples;
using Action = com.bloomberg.samples.rulemsx.Action;
using LogRmsx = com.bloomberg.samples.rulemsx.Log;
using LogIOI = com.bloomberg.ioiapi.samples.Log;

namespace RMSXIOITracking
{
    class RMSXIOITracking : NotificationHandler
    {

        RuleMSX rmsx;
        EasyIOI eioi;

        static void Main(string[] args)
        {
            RMSXIOITracking Test = new RMSXIOITracking();
            Test.Run();

            System.Console.WriteLine("Press enter to terminate...");
            System.Console.ReadLine();

            Test.Stop();

            System.Console.WriteLine("Terminating.");
        }

        private void log(String msg)
        {
            System.Console.WriteLine(DateTime.Now.ToString("yyyyMMddHHmmssfffzzz") + "(RMSXIOITracking..): \t" + msg);
        }

        private void Run()
        {

            log("RMSXIOITracking - Track incoming IOI messages");

            log("Initializing RuleMSX...");
            this.rmsx = new RuleMSX();
            LogRmsx.logLevel = LogRmsx.LogLevels.NONE;
            LogRmsx.logPrefix = "(RuleMSX..........)";

            log("RuleMSX initialized.");

            log("Initializing EasyIOI...");
            this.eioi = new EasyIOI();
            log("EasyIOI initialized.");

            LogIOI.logPrefix = "(EasyIOI..........)";
            LogIOI.logLevel = com.bloomberg.ioiapi.samples.Log.LogLevels.DETAILED;

            log("Create ruleset...");
            BuildRules();
            log("Ruleset ready.");

            this.eioi.iois.addNotificationHandler(this);

            log("Starting EasyIOI");
            this.eioi.start();
            log("EasyIOI started");
        }

        private void Stop()
        {
            log("Stopping RuleMSX");
            this.rmsx.Stop();
            log("RuleMSX stopped");
        }

        private void BuildRules()
        {
            log("Building rules...");

            log("Creating RuleSet rsIOITrack");
            RuleSet rsIOITrack = this.rmsx.CreateRuleSet("IOITrack");

            log("Creating rule for IOI_CHANGED");
            Rule ruleIOIChanged = rsIOITrack.AddRule("IOIChanged");
            ruleIOIChanged.AddRuleCondition(new RuleCondition("IOIChanged", new IOIChanged()));
            ruleIOIChanged.AddAction(this.rmsx.CreateAction("ShowIOIChanged", new ShowIOIState(this, "IOI has changed")));

            log("Rules built.");

        }

        public void ProcessNotification(Notification notification)
        {

            if (notification.category == Notification.NotificationCategory.IOIDATA)
            {
                if (notification.type == Notification.NotificationType.NEW)
                {
                    this.parseOrder(notification.GetIOI());
                }
            }
        }

        public void parseOrder(IOI i)
        {
            DataSet newDataSet = this.rmsx.CreateDataSet("DS_IOI_" + i.field("id_value").Value());
            newDataSet.AddDataPoint("handle", new IOIFieldDataPointSource(i, i.field("id_value")));
            this.rmsx.GetRuleSet("IOITrack").Execute(newDataSet);
        }

        class IOIFieldDataPointSource : DataPointSource, NotificationHandler
        {
            Field field;
            String value;

            internal IOIFieldDataPointSource(IOI i, Field field)
            {
                this.field = field;
                this.value = field.Value();

                i.addNotificationHandler(this);

            }

            public override object GetValue()
            {
                return this.field.Value();
            }

            public object GetPreviousValue()
            {
                return this.field.previousValue();
            }

            public void ProcessNotification(Notification notification)
            {
                if (this.field.previousValue() != this.field.Value()) this.SetStale();
            }

        }


        class ShowIOIState : ActionExecutor
        {
            RMSXIOITracking parent;
            String state = "";
            public ShowIOIState(RMSXIOITracking parent, String state)
            {
                this.parent = parent;
                this.state = state;
            }

            public void Execute(DataSet dataSet)
            {
                this.parent.log("IOI " + dataSet.GetDataPoint("handle").GetValue() + ": " + this.state);
            }
        }

        class IOIChanged : RuleEvaluator
        {
            public IOIChanged()
            {
                this.AddDependantDataPointName("handle");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                IOIFieldDataPointSource ioiHandleSource = (IOIFieldDataPointSource)dataSet.GetDataPoint("handle").GetSource();

                String currentHandle = Convert.ToString(ioiHandleSource.GetValue());
                String previousHandle = Convert.ToString(ioiHandleSource.GetPreviousValue());

                return (previousHandle != currentHandle);
            }
        }
    }
}
