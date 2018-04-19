using System;
using com.bloomberg.samples.rulemsx;
using com.bloomberg.emsx.samples;
using Action = com.bloomberg.samples.rulemsx.Action;
using System.Globalization;
using LogRmsx = com.bloomberg.samples.rulemsx.Log;

namespace RMSXEMSXRouteStateIdentifier
{
    class RMSXEMSXRouteStateIdentifier : NotificationHandler
    {

        RuleMSX rmsx;
        EasyMSX emsx;

        static void Main(string[] args)
        {
            RMSXEMSXRouteStateIdentifier Test = new RMSXEMSXRouteStateIdentifier();
            Test.Run();

            System.Console.WriteLine("Press enter to terminate...");
            System.Console.ReadLine();

            Test.Stop();

            System.Console.WriteLine("Terminating.");
        }

        private void log(String msg)
        {
            System.Console.WriteLine(DateTime.Now.ToString("yyyyMMddHHmmssfffzzz") + "(RMSXRouteFillTest): \t" + msg);
        }

        private void Run()
        {

            log("RMSXEMSXRouteStateIdentifier - Identify state changes in EMSX routes\n\n");

            log("Initializing RuleMSX...");
            this.rmsx = new RuleMSX();
            LogRmsx.logLevel = LogRmsx.LogLevels.DETAILED;
            LogRmsx.logPrefix = "(RuleMSX..........)";

            log("RuleMSX initialized.");

            log("Initializing EasyMSX...");
            this.emsx = new EasyMSX();
            log("EasyMSX initialized.");

            log("Create ruleset...");
            BuildRules();
            log("Ruleset ready.");

            this.emsx.routes.addNotificationHandler(this);

            log("Starting EasyMSX");
            this.emsx.start();
            log("EasyMSX started");
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

            log("Creating RuleSet rsRouteStates");
            RuleSet rsRouteStates = this.rmsx.CreateRuleSet("RouteStates");

            log("Creating rule for ROUTE_NEW");
            Rule ruleRouteNew = rsRouteStates.AddRule("RouteNew");
            ruleRouteNew.AddRuleCondition(new RuleCondition("RouteNew", new RouteNew()));
            ruleRouteNew.AddAction(this.rmsx.CreateAction("ShowRouteNew", new ShowRouteState(this, "New Route")));

            log("Creating rule for ROUTE_SET_WORKING_AMOUNT");
            Rule ruleRouteSetWorkingAmount = rsRouteStates.AddRule("RouteSetWorkingAmount");
            ruleRouteSetWorkingAmount.AddRuleCondition(new RuleCondition("RouteSetWorkingAmount", new RouteSetWorkingAmount()));
            ruleRouteSetWorkingAmount.AddAction(this.rmsx.CreateAction("ShowRouteSetWorkingAmount", new ShowRouteState(this, "Setting working amount")));

            log("Creating rule for ROUTE_NEW_BROKER_ACK");
            Rule ruleRouteNewBrokerAck = rsRouteStates.AddRule("RouteNewBrokerAck");
            ruleRouteNewBrokerAck.AddRuleCondition(new RuleCondition("RouteNewBrokerAck", new RouteNewBrokerAck()));
            ruleRouteNewBrokerAck.AddAction(this.rmsx.CreateAction("ShowRouteNewBrokerAck", new ShowRouteState(this, "Broker acknowledged new route")));

            log("Creating rule for ROUTE_FILL_FIRST_MULTI");
            Rule ruleRouteFillFirstMulti = rsRouteStates.AddRule("RouteFillFirstMulti");
            ruleRouteFillFirstMulti.AddRuleCondition(new RuleCondition("RouteFillFirstMulti", new RouteFillFirstMulti()));
            ruleRouteFillFirstMulti.AddAction(this.rmsx.CreateAction("ShowRouteFillFirstMulti", new ShowRouteState(this, "First partial fill")));

            log("Creating rule for ROUTE_FILL_MULTI");
            Rule ruleRouteFillMulti = rsRouteStates.AddRule("RouteFillMulti");
            ruleRouteFillMulti.AddRuleCondition(new RuleCondition("RouteFillMulti", new RouteFillMulti()));
            ruleRouteFillMulti.AddAction(this.rmsx.CreateAction("ShowRouteFillMulti", new ShowRouteState(this, "Partial fill")));

            log("Creating rule for ROUTE_FILL_FINAL_MULTI");
            Rule ruleRouteFillFinalMulti = rsRouteStates.AddRule("RouteFillFinalMulti");
            ruleRouteFillFinalMulti.AddRuleCondition(new RuleCondition("RouteFillFinalMulti", new RouteFillFinalMulti()));
            ruleRouteFillFinalMulti.AddAction(this.rmsx.CreateAction("ShowRouteFillMulti", new ShowRouteState(this, "Final partial fill. Route filled")));

            log("Creating rule for ROUTE_FILL_FIRST_FULL");
            Rule ruleRouteFillFirstFull = rsRouteStates.AddRule("RouteFillFirstFull");
            ruleRouteFillFirstFull.AddRuleCondition(new RuleCondition("RouteFillFirstFull", new RouteFillFirstFull()));
            ruleRouteFillFirstFull.AddAction(this.rmsx.CreateAction("ShowRouteFillMulti", new ShowRouteState(this, "Single full fill. Route filled")));

            log("Creating rule for ROUTE_INIT_PAINT_FILLED");
            Rule ruleRouteInitPaintFilled = rsRouteStates.AddRule("RouteInitPaintFilled");
            ruleRouteInitPaintFilled.AddRuleCondition(new RuleCondition("RouteInitPaintFilled", new RouteInitPaintFilled()));
            ruleRouteInitPaintFilled.AddAction(this.rmsx.CreateAction("ShowRouteFillMulti", new ShowRouteState(this, "Initial paint shows route Filled")));

            log("Creating rule for ROUTE_INIT_PAINT_WORKING");
            Rule ruleRouteInitPaintWorking = rsRouteStates.AddRule("RouteInitPaintWorking");
            ruleRouteInitPaintWorking.AddRuleCondition(new RuleCondition("RouteInitPaintWorking", new RouteInitPaintWorking()));
            ruleRouteInitPaintWorking.AddAction(this.rmsx.CreateAction("ShowRouteInitPaintWorking", new ShowRouteState(this, "Initial paint shows route Working")));

            log("Creating rule for ROUTE_INIT_PAINT_PARTFILL");
            Rule ruleRouteInitPaintPartfill = rsRouteStates.AddRule("RouteInitPaintPartfill");
            ruleRouteInitPaintPartfill.AddRuleCondition(new RuleCondition("RouteInitPaintPartfill", new RouteInitPaintPartfill()));
            ruleRouteInitPaintPartfill.AddAction(this.rmsx.CreateAction("ShowRouteInitPaintPartfill", new ShowRouteState(this, "Initial paint shows route Partfilled")));

            log("Creating rule for ROUTE_INIT_PAINT_CANCEL_REQUESTED");
            Rule ruleRouteInitPaintCancelRequested = rsRouteStates.AddRule("RouteInitPaintCancelRequested");
            ruleRouteInitPaintCancelRequested.AddRuleCondition(new RuleCondition("RouteInitPaintCancelRequested", new RouteInitPaintCancelRequested()));
            ruleRouteInitPaintCancelRequested.AddAction(this.rmsx.CreateAction("ShowRouteInitPaintCancelRequested", new ShowRouteState(this, "Initial paint shows route is awaiting cancel")));

            log("Creating rule condition for ROUTE_INIT_PAINT_CANCEL_REQUESTED");


            log("Creating rule condition for ROUTE_CANCEL_REQUESTED_ON_WORKING");


            log("Creating rule condition for ROUTE_CANCEL_REJECTED_ON_WORKING_FROM_REQUEST");


            log("Creating rule condition for ROUTE_CANCEL_BROKER_ACK");


            log("Creating rule condition for ROUTE_CANCEL_REJECTED_ON_WORKING_FROM_PENDING");


            log("Creating rule condition for ROUTE_CANCEL_FROM_REQUESTED");


            log("Creating rule condition for ROUTE_CANCEL_FROM_PENDING");


            log("Creating rule condition for ROUTE_CANCEL_REQUESTED_ON_PARTFILL");


            log("Creating rule condition for ROUTE_CANCEL_REJECTED_ON_PARTFILL);


            log("Creating rule condition for ROUTE_CANCEL_REJECTED_ON_PARTFILL);


            log("Creating rule condition for ROUTE_MODIFY_REQUESTED_ON_WORKING


            log("Creating rule condition for ROUTE_MODIFY_BROKER_ACK


            log("Creating rule condition for ROUTE_MODIFY_REJECTED_ON_WORKING



            log("Creating rule condition for ROUTE_MODIFY_APPLIED_ON_WORKING



            log("Creating rule condition for ROUTE_MODIFY_REQUESTED_ON_PARTFILL


            log("Creating rule condition for ROUTE_MODIFY_REJECTED_ON_PARTFILL



            log("Creating rule condition for ROUTE_MODIFY_APPLIED_ON_PARTFILL



            log("Creating RuleCondition condRouteFillOccurred");
            RuleCondition condRouteFillOccurred = new RuleCondition("RouteFillOccurred", new RouteFillOccurred(this));
            log("RuleCondition condRouteFillOccurred created.");

            log("Creating Action actShowRouteFill");
            Action actShowRouteFill = this.rmsx.CreateAction("ShowRouteFill", new ShowRouteFill(this));
            log("Action actShowRouteFill created");

            log("Creating RuleSet demoRouteRuleSet");
            RuleSet demoRouteRuleSet = this.rmsx.CreateRuleSet("DemoRouteRuleSet");
            log("RuleSet demoRouteRuleSet created");

            log("Creating Rule ruleRouteFilled");
            Rule ruleRouteFilled = demoRouteRuleSet.AddRule("RouteFilled");
            log("Rule ruleRouteFilled created");

            log("Assign RuleCondition condRouteFillOccurred to Rule ruleRouteFilled");
            ruleRouteFilled.AddRuleCondition(condRouteFillOccurred);

            log("Assign Action actShowRouteFill to Rule ruleRouteFilled");
            ruleRouteFilled.AddAction(actShowRouteFill);

            log("Rules built.");

        }

        public void processNotification(Notification notification)
        {

            if (notification.category == Notification.NotificationCategory.ROUTE)
            {
                if ((notification.type == Notification.NotificationType.NEW) || (notification.type == Notification.NotificationType.INITIALPAINT))
                {
                    this.parseRoute(notification.getRoute());
                }
            }
        }

        public void parseRoute(Route r)
        {
            DataSet newDataSet = this.rmsx.CreateDataSet("DS_RT_" + r.field("EMSX_SEQUENCE").value() + "." + r.field("EMSX_ROUTE_ID").value());
            newDataSet.AddDataPoint("RouteStatus", new EMSXFieldDataPointSource(r.field("EMSX_STATUS")));
            newDataSet.AddDataPoint("RouteOrderNumber", new EMSXFieldDataPointSource(r.field("EMSX_SEQUENCE")));
            newDataSet.AddDataPoint("RouteID", new EMSXFieldDataPointSource(r.field("EMSX_ROUTE_ID")));
            newDataSet.AddDataPoint("RouteFilled", new EMSXFieldDataPointSource(r.field("EMSX_FILLED")));
            newDataSet.AddDataPoint("RouteAmount", new EMSXFieldDataPointSource(r.field("EMSX_AMOUNT")));
            newDataSet.AddDataPoint("RouteLastShares", new EMSXFieldDataPointSource(r.field("EMSX_LAST_SHARES")));
            newDataSet.AddDataPoint("LastFillShown", new GenericIntegerDataPointSource(0));
            this.rmsx.GetRuleSet("RouteStates").Execute(newDataSet);
        }


        class GenericIntegerDataPointSource : DataPointSource
        {
            int value;

            internal GenericIntegerDataPointSource(int initialValue)
            {
                this.value = initialValue;
            }

            public override object GetValue()
            {
                return this.value;
            }

            public void setValue(int newValue)
            {
                this.value = newValue;
                this.SetStale();
            }
        }

        class EMSXFieldDataPointSource : DataPointSource, NotificationHandler
        {
            Field field;
            String value;
            String previousValue;

            internal EMSXFieldDataPointSource(Field field)
            {
                this.field = field;
                this.value = field.value();
                this.previousValue = null;

                this.field.addNotificationHandler(this);
            }

            public override object GetValue()
            {
                return this.value;
            }

            public object GetPreviousValue()
            {
                return this.previousValue;
            }

            public void processNotification(Notification notification)
            {
                this.previousValue = this.value;
                this.value = notification.getFieldChanges()[0].newValue;

                if (this.previousValue != this.value) this.SetStale();
            }
        }

        class RouteNew : RuleEvaluator
        {
            public RouteNew()
            {
                this.AddDependantDataPointName("RouteStatus");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                EMSXFieldDataPointSource routeStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("RouteStatus").GetSource();

                String currentStatus = Convert.ToString(routeStatusSource.GetValue());
                String previousStatus = Convert.ToString(routeStatusSource.GetPreviousValue());
                return (previousStatus == null) && (currentStatus=="SENT");
            }
        }

        class ShowRouteState : ActionExecutor
        {
            RMSXEMSXRouteStateIdentifier parent;
            String state = "";
            public ShowRouteState(RMSXEMSXRouteStateIdentifier parent, String state)
            {
                this.parent = parent;
                this.state = state;
            }

            public void Execute(DataSet dataSet)
            {
                this.parent.log("Route " + dataSet.GetDataPoint("RouteOrderNumber").GetValue() + "." + dataSet.GetDataPoint("RouteID").GetValue() + ": " + this.state);
            }
        }
    }
}
