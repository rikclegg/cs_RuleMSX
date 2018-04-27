using System;
using com.bloomberg.samples.rulemsx;
using com.bloomberg.ioiapi.samples;
using Action = com.bloomberg.samples.rulemsx.Action;
using LogRmsx = com.bloomberg.samples.rulemsx.Log;
using LogIOI = com.bloomberg.ioiapi.samples.Log;
using System.Threading;

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

        private static void log(String msg)
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

            log("Creating rule for IOI_NEW");
            Rule ruleIOINew = rsIOITrack.AddRule("IOIIOINew");
            ruleIOINew.AddRuleCondition(new RuleCondition("IOINew", new IOINew()));
            ruleIOINew.AddAction(this.rmsx.CreateAction("ShowIOINew", new ShowIOIState(this, "new IOI")));

            log("Creating rule for IOI_REPLACED");
            Rule ruleIOIReplaced = rsIOITrack.AddRule("IOIReplaced");
            ruleIOIReplaced.AddRuleCondition(new RuleCondition("IOIReplaced", new IOIReplaced()));
            ruleIOIReplaced.AddAction(this.rmsx.CreateAction("ShowIOIReplaced", new ShowIOIState(this, "IOI replaced")));

            log("Creating rule for IOI_CANCELLED");
            Rule ruleIOICancelled = rsIOITrack.AddRule("IOICancelled");
            ruleIOICancelled.AddRuleCondition(new RuleCondition("IOICancelled", new IOICancelled()));
            ruleIOICancelled.AddAction(this.rmsx.CreateAction("ShowIOICancelled", new ShowIOIState(this, "IOI cancelled")));

            log("Creating rule for IOI_EXPIRED");
            Rule ruleIOIExpired = rsIOITrack.AddRule("IOIExpired");
            ruleIOIExpired.AddRuleCondition(new RuleCondition("IOIExpired", new IOIExpired()));
            ruleIOIExpired.AddAction(this.rmsx.CreateAction("ShowIOIExpired", new ShowIOIState(this, "IOI expired")));

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
            newDataSet.AddDataPoint("change", new IOIFieldDataPointSource(i, i.field("change")));
            newDataSet.AddDataPoint("ioi_goodUntil", new IOIFieldDataPointSource(i, i.field("ioi_goodUntil")));
            newDataSet.AddDataPoint("expired", new TimedBoolean(false, DateTime.Parse(i.field("ioi_goodUntil").Value())));
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

        class TimedBoolean : DataPointSource
        {
            bool value;
            Timer timer;

            internal TimedBoolean(bool initialState, DateTime triggerTime)
            {
                TimeSpan ts = triggerTime - DateTime.Now;
                this.timer = new Timer(trigger,null, (int)ts.TotalMilliseconds,Timeout.Infinite);
            }


            public override object GetValue()
            {
                return this.value;
            }

            public void trigger(object info)
            {
                this.value = !this.value;
                this.SetStale();
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
                log("IOI " + dataSet.GetDataPoint("handle").GetValue() + ": " + this.state);
            }
        }

        class IOINew : RuleEvaluator
        {
            public IOINew()
            {
                this.AddDependantDataPointName("change");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                IOIFieldDataPointSource ioiChangeSource = (IOIFieldDataPointSource)dataSet.GetDataPoint("change").GetSource();

                String currentChange = Convert.ToString(ioiChangeSource.GetValue());

                return (currentChange == "New");
            }
        }

        class IOIReplaced : RuleEvaluator
        {
            public IOIReplaced()
            {
                this.AddDependantDataPointName("change");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                IOIFieldDataPointSource ioiChangeSource = (IOIFieldDataPointSource)dataSet.GetDataPoint("change").GetSource();

                String currentChange = Convert.ToString(ioiChangeSource.GetValue());

                return (currentChange == "Replace");
            }
        }


        class IOICancelled : RuleEvaluator
        {
            public IOICancelled()
            {
                this.AddDependantDataPointName("change");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                IOIFieldDataPointSource ioiChangeSource = (IOIFieldDataPointSource)dataSet.GetDataPoint("change").GetSource();

                String currentChange = Convert.ToString(ioiChangeSource.GetValue());

                return (currentChange == "Cancel");
            }
        }

        class IOIExpired : RuleEvaluator
        {
            public IOIExpired()
            {
                this.AddDependantDataPointName("expired");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                TimedBoolean isExpiredSource = (TimedBoolean)dataSet.GetDataPoint("expired").GetSource();

                return (Convert.ToBoolean(isExpiredSource.GetValue()));
            }
        }

    }
}
