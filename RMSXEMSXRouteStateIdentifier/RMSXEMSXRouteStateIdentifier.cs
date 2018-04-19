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
            LogRmsx.logLevel = LogRmsx.LogLevels.NONE;
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
            ruleRouteInitPaintFilled.AddAction(this.rmsx.CreateAction("ShowRouteFillMulti", new ShowRouteState(this, "Initial paint shows route filled")));

            log("Creating rule for ROUTE_INIT_PAINT_WORKING");
            Rule ruleRouteInitPaintWorking = rsRouteStates.AddRule("RouteInitPaintWorking");
            ruleRouteInitPaintWorking.AddRuleCondition(new RuleCondition("RouteInitPaintWorking", new RouteInitPaintWorking()));
            ruleRouteInitPaintWorking.AddAction(this.rmsx.CreateAction("ShowRouteInitPaintWorking", new ShowRouteState(this, "Initial paint shows route working")));

            log("Creating rule for ROUTE_INIT_PAINT_PARTFILL");
            Rule ruleRouteInitPaintPartfill = rsRouteStates.AddRule("RouteInitPaintPartfill");
            ruleRouteInitPaintPartfill.AddRuleCondition(new RuleCondition("RouteInitPaintPartfill", new RouteInitPaintPartfill()));
            ruleRouteInitPaintPartfill.AddAction(this.rmsx.CreateAction("ShowRouteInitPaintPartfill", new ShowRouteState(this, "Initial paint shows route partfilled")));

            log("Creating rule for ROUTE_INIT_PAINT_CANCEL_REQUESTED");
            Rule ruleRouteInitPaintCancelRequested = rsRouteStates.AddRule("RouteInitPaintCancelRequested");
            ruleRouteInitPaintCancelRequested.AddRuleCondition(new RuleCondition("RouteInitPaintCancelRequested", new RouteInitPaintCancelRequested()));
            ruleRouteInitPaintCancelRequested.AddAction(this.rmsx.CreateAction("ShowRouteInitPaintCancelRequested", new ShowRouteState(this, "Initial paint shows route is awaiting cancel")));

            log("Creating rule for ROUTE_CANCEL_REQUESTED_ON_WORKING");
            Rule ruleRouteCancelRequestedOnWorking = rsRouteStates.AddRule("RouteCancelRequestedOnWorking");
            ruleRouteCancelRequestedOnWorking.AddRuleCondition(new RuleCondition("RouteCancelRequestedOnWorking", new RouteCancelRequestedOnWorking()));
            ruleRouteCancelRequestedOnWorking.AddAction(this.rmsx.CreateAction("ShowRouteCancelRequestedOnWorking", new ShowRouteState(this, "Cancel requested on working route")));

            log("Creating rule for ROUTE_CANCEL_REJECTED_ON_WORKING_FROM_REQUEST");
            Rule ruleRouteCancelRejectedOnWorkingFromRequest = rsRouteStates.AddRule("RouteCancelRejectedOnWorkingFromRequest");
            ruleRouteCancelRejectedOnWorkingFromRequest.AddRuleCondition(new RuleCondition("RouteCancelRejectedOnWorkingFromRequest", new RouteCancelRejectedOnWorkingFromRequest()));
            ruleRouteCancelRejectedOnWorkingFromRequest.AddAction(this.rmsx.CreateAction("ShowRouteCancelRejectedOnWorkingFromRequest", new ShowRouteState(this, "Cancel rejected from request. Back to working state")));

            log("Creating rule for ROUTE_CANCEL_BROKER_ACK");
            Rule ruleRouteCancelBrokerAck = rsRouteStates.AddRule("RouteCancelBrokerAck");
            ruleRouteCancelBrokerAck.AddRuleCondition(new RuleCondition("RouteCancelBrokerAck", new RouteCancelBrokerAck()));
            ruleRouteCancelBrokerAck.AddAction(this.rmsx.CreateAction("ShowRouteCancelBrokerAck", new ShowRouteState(this, "Cancel request acknowledged by broker")));

            log("Creating rule for ROUTE_CANCEL_REJECTED_ON_WORKING_FROM_PENDING");
            Rule ruleRouteCancelRejectedOnWorkingFromPending = rsRouteStates.AddRule("RouteCancelRejectedOnWorkingFromPending");
            ruleRouteCancelRejectedOnWorkingFromPending.AddRuleCondition(new RuleCondition("RouteCancelRejectedOnWorkingFromPending", new RouteCancelRejectedOnWorkingFromPending()));
            ruleRouteCancelRejectedOnWorkingFromPending.AddAction(this.rmsx.CreateAction("ShowRouteCancelRejectedOnWorkingFromPending", new ShowRouteState(this, "Cancel rejected from request. Back to pending state")));

            log("Creating rule for ROUTE_CANCEL_FROM_REQUESTED");
            Rule ruleRouteCancelFromRequested = rsRouteStates.AddRule("RouteCancelFromRequested");
            ruleRouteCancelFromRequested.AddRuleCondition(new RuleCondition("RouteCancelFromRequested", new RouteCancelFromRequested()));
            ruleRouteCancelFromRequested.AddAction(this.rmsx.CreateAction("ShowRouteCancelFromRequested", new ShowRouteState(this, "Cancelled from request")));

            log("Creating rule for ROUTE_CANCEL_FROM_PENDING");
            Rule ruleRouteCancelFromPending = rsRouteStates.AddRule("RouteCancelFromPending");
            ruleRouteCancelFromPending.AddRuleCondition(new RuleCondition("RouteCancelFromPending", new RouteCancelFromPending()));
            ruleRouteCancelFromPending.AddAction(this.rmsx.CreateAction("ShowRouteCancelFromPending", new ShowRouteState(this, "Cancelled from pending")));

            log("Creating rule for ROUTE_CANCEL_REQUESTED_ON_PARTFILL");
            Rule ruleRouteCancelRequestedOnPartfill = rsRouteStates.AddRule("RouteCancelRequestedOnPartfill");
            ruleRouteCancelRequestedOnPartfill.AddRuleCondition(new RuleCondition("RouteCancelRequestedOnPartfill", new RouteCancelRequestedOnPartfill()));
            ruleRouteCancelRequestedOnPartfill.AddAction(this.rmsx.CreateAction("ShowRouteCancelRequestedOnPartfill", new ShowRouteState(this, "Cancel requested on partfilled route")));

            log("Creating rule for ROUTE_CANCEL_REJECTED_ON_PARTFILL_FROM_REQUEST");
            Rule ruleRouteCancelRejectedOnPartfillFromRequest = rsRouteStates.AddRule("RouteCancelRejectedOnPartfillFromRequest");
            ruleRouteCancelRejectedOnPartfillFromRequest.AddRuleCondition(new RuleCondition("RouteCancelRejectedOnPartfillFromRequest", new RouteCancelRejectedOnPartfillFromRequest()));
            ruleRouteCancelRejectedOnPartfillFromRequest.AddAction(this.rmsx.CreateAction("ShowRouteCancelRejectedOnPartfillFromRequest", new ShowRouteState(this, "Cancel rejected from request. Return to partfilled")));

            log("Creating rule for ROUTE_CANCEL_REJECTED_ON_PARTFILL_FROM_PENDING");
            Rule ruleRouteCancelRejectedOnPartfillFromPending = rsRouteStates.AddRule("RouteCancelRejectedOnPartfillFromPending");
            ruleRouteCancelRejectedOnPartfillFromPending.AddRuleCondition(new RuleCondition("RouteCancelRejectedOnPartfillFromPending", new RouteCancelRejectedOnPartfillFromPending()));
            ruleRouteCancelRejectedOnPartfillFromPending.AddAction(this.rmsx.CreateAction("ShowRouteCancelRejectedOnPartfillFromPending", new ShowRouteState(this, "Cancel rejected from pending. Return to partfilled")));

            log("Creating rule for ROUTE_MODIFY_REQUESTED_ON_WORKING");
            Rule ruleRouteModifyRequestedOnWorking = rsRouteStates.AddRule("RouteModifyRequestedOnWorking");
            ruleRouteModifyRequestedOnWorking.AddRuleCondition(new RuleCondition("RouteModifyRequestedOnWorking", new RouteModifyRequestedOnWorking()));
            ruleRouteModifyRequestedOnWorking.AddAction(this.rmsx.CreateAction("ShowRouteModifyRequestedOnWorking", new ShowRouteState(this, "Modify requested on working route")));

            log("Creating rule for ROUTE_MODIFY_BROKER_ACK");
            Rule ruleRouteModifyBrokerAck = rsRouteStates.AddRule("RouteModifyBrokerAck");
            ruleRouteModifyBrokerAck.AddRuleCondition(new RuleCondition("RouteModifyBrokerAck", new RouteModifyBrokerAck()));
            ruleRouteModifyBrokerAck.AddAction(this.rmsx.CreateAction("ShowRouteModifyBrokerAck", new ShowRouteState(this, "Modify request acknowledged by broker")));

            log("Creating rule for ROUTE_MODIFY_REJECTED_ON_WORKING");
            Rule ruleRouteModifyRejectedOnWorking = rsRouteStates.AddRule("RouteModifyRejectedOnWorking");
            ruleRouteModifyRejectedOnWorking.AddRuleCondition(new RuleCondition("RouteModifyRejectedOnWorking", new RouteModifyRejectedOnWorking()));
            ruleRouteModifyRejectedOnWorking.AddAction(this.rmsx.CreateAction("ShowRouteModifyRejectedOnWorking", new ShowRouteState(this, "Modify request rejected on working route")));

            log("Creating rule for ROUTE_MODIFY_APPLIED_ON_WORKING");
            Rule ruleRouteModifyAppliedOnWorking = rsRouteStates.AddRule("RouteModifyAppliedOnWorking");
            ruleRouteModifyAppliedOnWorking.AddRuleCondition(new RuleCondition("RouteModifyAppliedOnWorking", new RouteModifyAppliedOnWorking()));
            ruleRouteModifyAppliedOnWorking.AddAction(this.rmsx.CreateAction("ShowRouteModifyAppliedOnWorking", new ShowRouteState(this, "Modify request accepted and applied on working route")));

            log("Creating rule for ROUTE_MODIFY_REQUESTED_ON_PARTFILL");
            Rule ruleRouteModifyRequestedOnPartfill = rsRouteStates.AddRule("RouteModifyRequestedOnPartfill");
            ruleRouteModifyRequestedOnPartfill.AddRuleCondition(new RuleCondition("RouteModifyRequestedOnPartfill", new RouteModifyRequestedOnPartfill()));
            ruleRouteModifyRequestedOnPartfill.AddAction(this.rmsx.CreateAction("ShowRouteModifyRequestedOnPartfill", new ShowRouteState(this, "Modify requested on partfilled route")));

            log("Creating rule for ROUTE_MODIFY_REJECTED_ON_PARTFILL");
            Rule ruleRouteModifyRejectedOnPartfill = rsRouteStates.AddRule("RouteModifyRejectedOnPartfill");
            ruleRouteModifyRejectedOnPartfill.AddRuleCondition(new RuleCondition("RouteModifyRejectedOnPartfill", new RouteModifyRejectedOnPartfill()));
            ruleRouteModifyRejectedOnPartfill.AddAction(this.rmsx.CreateAction("ShowRouteModifyRejectedOnPartfill", new ShowRouteState(this, "Modify request rejected on partfilled route")));

            log("Creating rule for ROUTE_MODIFY_APPLIED_ON_PARTFILL");
            Rule ruleRouteModifyAppliedOnPartfill = rsRouteStates.AddRule("RouteModifyAppliedOnPartfill");
            ruleRouteModifyAppliedOnPartfill.AddRuleCondition(new RuleCondition("RouteModifyAppliedOnPartfill", new RouteModifyAppliedOnPartfill()));
            ruleRouteModifyAppliedOnPartfill.AddAction(this.rmsx.CreateAction("ShowRouteModifyAppliedOnPartfill", new ShowRouteState(this, "Modify request accepted and applied on partfilled route")));

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
            newDataSet.AddDataPoint("RouteWorking", new EMSXFieldDataPointSource(r.field("EMSX_WORKING")));
            newDataSet.AddDataPoint("RouteBrokerStatus", new EMSXFieldDataPointSource(r.field("EMSX_BROKER_STATUS")));
            this.rmsx.GetRuleSet("RouteStates").Execute(newDataSet);
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
                return (previousStatus == null) && (currentStatus == "SENT");
            }
        }


        class RouteSetWorkingAmount : RuleEvaluator
        {
            public RouteSetWorkingAmount()
            {
                this.AddDependantDataPointName("RouteStatus");
                this.AddDependantDataPointName("RouteWorking");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                EMSXFieldDataPointSource routeStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("RouteStatus").GetSource();
                EMSXFieldDataPointSource routeWorking = (EMSXFieldDataPointSource)dataSet.GetDataPoint("RouteWorking").GetSource();

                String currentStatus = Convert.ToString(routeStatusSource.GetValue());
                String previousStatus = Convert.ToString(routeStatusSource.GetPreviousValue());

                int currentWorking = Convert.ToInt32(routeWorking.GetValue());
                int previousWorking = Convert.ToInt32(routeWorking.GetPreviousValue());

                return ((previousWorking == 0) && (currentWorking > 0)) && ((previousStatus == "SENT") && (currentStatus == "SENT"));
            }
        }

        
        class RouteNewBrokerAck : RuleEvaluator
        {
            public RouteNewBrokerAck()
            {
                this.AddDependantDataPointName("RouteStatus");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                EMSXFieldDataPointSource routeStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("RouteStatus").GetSource();

                String currentStatus = Convert.ToString(routeStatusSource.GetValue());
                String previousStatus = Convert.ToString(routeStatusSource.GetPreviousValue());

                return ((previousStatus == "SENT") && (currentStatus == "WORKING"));
            }
        }

        class RouteFillFirstMulti : RuleEvaluator
        {
            public RouteFillFirstMulti()
            {
                this.AddDependantDataPointName("RouteStatus");
                this.AddDependantDataPointName("RouteWorking");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                EMSXFieldDataPointSource routeStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("RouteStatus").GetSource();
                EMSXFieldDataPointSource routeWorking = (EMSXFieldDataPointSource)dataSet.GetDataPoint("RouteWorking").GetSource();

                String currentStatus = Convert.ToString(routeStatusSource.GetValue());
                String previousStatus = Convert.ToString(routeStatusSource.GetPreviousValue());

                int currentWorking = Convert.ToInt32(routeWorking.GetValue());
                int previousWorking = Convert.ToInt32(routeWorking.GetPreviousValue());

                return ((previousWorking > currentWorking) && (currentWorking > 0)) && ((previousStatus == "WORKING") && (currentStatus == "PARTFILL"));
            }
        }


        class RouteFillMulti : RuleEvaluator
        {
            public RouteFillMulti()
            {
                this.AddDependantDataPointName("RouteStatus");
                this.AddDependantDataPointName("RouteWorking");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                EMSXFieldDataPointSource routeStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("RouteStatus").GetSource();
                EMSXFieldDataPointSource routeWorking = (EMSXFieldDataPointSource)dataSet.GetDataPoint("RouteWorking").GetSource();

                String currentStatus = Convert.ToString(routeStatusSource.GetValue());
                String previousStatus = Convert.ToString(routeStatusSource.GetPreviousValue());

                int currentWorking = Convert.ToInt32(routeWorking.GetValue());
                int previousWorking = Convert.ToInt32(routeWorking.GetPreviousValue());

                return ((previousWorking > currentWorking) && (currentWorking > 0)) && ((previousStatus == "PARTFILL") && (currentStatus == "PARTFILL"));
            }
        }

        class RouteFillFinalMulti : RuleEvaluator
        {
            public RouteFillFinalMulti()
            {
                this.AddDependantDataPointName("RouteStatus");
                this.AddDependantDataPointName("RouteWorking");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                EMSXFieldDataPointSource routeStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("RouteStatus").GetSource();
                EMSXFieldDataPointSource routeWorking = (EMSXFieldDataPointSource)dataSet.GetDataPoint("RouteWorking").GetSource();

                String currentStatus = Convert.ToString(routeStatusSource.GetValue());
                String previousStatus = Convert.ToString(routeStatusSource.GetPreviousValue());

                int currentWorking = Convert.ToInt32(routeWorking.GetValue());
                int previousWorking = Convert.ToInt32(routeWorking.GetPreviousValue());

                return ((previousWorking > 0) && (currentWorking == 0)) && ((previousStatus == "PARTFILL") && (currentStatus == "FILLED"));
            }
        }

        class RouteFillFirstFull : RuleEvaluator
        {
            public RouteFillFirstFull()
            {
                this.AddDependantDataPointName("RouteStatus");
                this.AddDependantDataPointName("RouteWorking");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                EMSXFieldDataPointSource routeStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("RouteStatus").GetSource();
                EMSXFieldDataPointSource routeWorking = (EMSXFieldDataPointSource)dataSet.GetDataPoint("RouteWorking").GetSource();

                String currentStatus = Convert.ToString(routeStatusSource.GetValue());
                String previousStatus = Convert.ToString(routeStatusSource.GetPreviousValue());

                int currentWorking = Convert.ToInt32(routeWorking.GetValue());
                int previousWorking = Convert.ToInt32(routeWorking.GetPreviousValue());

                return ((previousWorking > 0) && (currentWorking == 0)) && ((previousStatus == "WORKING") && (currentStatus == "FILLED"));
            }
        }


        class RouteInitPaintFilled : RuleEvaluator
        {
            public RouteInitPaintFilled()
            {
                this.AddDependantDataPointName("RouteStatus");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                EMSXFieldDataPointSource routeStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("RouteStatus").GetSource();

                String currentStatus = Convert.ToString(routeStatusSource.GetValue());
                String previousStatus = Convert.ToString(routeStatusSource.GetPreviousValue());

                return ((previousStatus == null) && (currentStatus == "FILLED"));
            }
        }


        class RouteInitPaintWorking : RuleEvaluator
        {
            public RouteInitPaintWorking()
            {
                this.AddDependantDataPointName("RouteStatus");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                EMSXFieldDataPointSource routeStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("RouteStatus").GetSource();

                String currentStatus = Convert.ToString(routeStatusSource.GetValue());
                String previousStatus = Convert.ToString(routeStatusSource.GetPreviousValue());

                return ((previousStatus == null) && (currentStatus == "WORKING"));
            }
        }


        class RouteInitPaintPartfill : RuleEvaluator
        {
            public RouteInitPaintPartfill()
            {
                this.AddDependantDataPointName("RouteStatus");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                EMSXFieldDataPointSource routeStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("RouteStatus").GetSource();

                String currentStatus = Convert.ToString(routeStatusSource.GetValue());
                String previousStatus = Convert.ToString(routeStatusSource.GetPreviousValue());

                return ((previousStatus == null) && (currentStatus == "PARTFILL"));
            }
        }

        class RouteInitPaintCancelRequested : RuleEvaluator
        {
            public RouteInitPaintCancelRequested()
            {
                this.AddDependantDataPointName("RouteStatus");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                EMSXFieldDataPointSource routeStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("RouteStatus").GetSource();

                String currentStatus = Convert.ToString(routeStatusSource.GetValue());
                String previousStatus = Convert.ToString(routeStatusSource.GetPreviousValue());

                return ((previousStatus == null) && (currentStatus == "CXLREQ"));
            }
        }


        class RouteCancelRequestedOnWorking : RuleEvaluator
        {
            public RouteCancelRequestedOnWorking()
            {
                this.AddDependantDataPointName("RouteStatus");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                EMSXFieldDataPointSource routeStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("RouteStatus").GetSource();

                String currentStatus = Convert.ToString(routeStatusSource.GetValue());
                String previousStatus = Convert.ToString(routeStatusSource.GetPreviousValue());

                return ((previousStatus == "WORKING") && (currentStatus == "CXLREQ"));
            }
        }


        class RouteCancelRejectedOnWorkingFromRequest : RuleEvaluator
        {
            public RouteCancelRejectedOnWorkingFromRequest()
            {
                this.AddDependantDataPointName("RouteStatus");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                EMSXFieldDataPointSource routeStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("RouteStatus").GetSource();

                String currentStatus = Convert.ToString(routeStatusSource.GetValue());
                String previousStatus = Convert.ToString(routeStatusSource.GetPreviousValue());

                return ((previousStatus == "CXLREQ") && (currentStatus == "WORKING"));
            }
        }


        class RouteCancelBrokerAck : RuleEvaluator
        {
            public RouteCancelBrokerAck()
            {
                this.AddDependantDataPointName("RouteStatus");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                EMSXFieldDataPointSource routeStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("RouteStatus").GetSource();

                String currentStatus = Convert.ToString(routeStatusSource.GetValue());
                String previousStatus = Convert.ToString(routeStatusSource.GetPreviousValue());

                return ((previousStatus == "CXLREQ") && (currentStatus == "CXLPEN"));
            }
        }


        class RouteCancelRejectedOnWorkingFromPending : RuleEvaluator
        {
            public RouteCancelRejectedOnWorkingFromPending()
            {
                this.AddDependantDataPointName("RouteStatus");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                EMSXFieldDataPointSource routeStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("RouteStatus").GetSource();

                String currentStatus = Convert.ToString(routeStatusSource.GetValue());
                String previousStatus = Convert.ToString(routeStatusSource.GetPreviousValue());

                return ((previousStatus == "CXLPEN") && (currentStatus == "WORKING"));
            }
        }


        class RouteCancelFromRequested : RuleEvaluator
        {
            public RouteCancelFromRequested()
            {
                this.AddDependantDataPointName("RouteStatus");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                EMSXFieldDataPointSource routeStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("RouteStatus").GetSource();

                String currentStatus = Convert.ToString(routeStatusSource.GetValue());
                String previousStatus = Convert.ToString(routeStatusSource.GetPreviousValue());

                return ((previousStatus == "CXLREQ") && (currentStatus == "CANCEL"));
            }
        }

        class RouteCancelFromPending : RuleEvaluator
        {
            public RouteCancelFromPending()
            {
                this.AddDependantDataPointName("RouteStatus");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                EMSXFieldDataPointSource routeStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("RouteStatus").GetSource();

                String currentStatus = Convert.ToString(routeStatusSource.GetValue());
                String previousStatus = Convert.ToString(routeStatusSource.GetPreviousValue());

                return ((previousStatus == "CXLPEN") && (currentStatus == "CANCEL"));
            }
        }

        class RouteCancelRequestedOnPartfill : RuleEvaluator
        {
            public RouteCancelRequestedOnPartfill()
            {
                this.AddDependantDataPointName("RouteStatus");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                EMSXFieldDataPointSource routeStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("RouteStatus").GetSource();

                String currentStatus = Convert.ToString(routeStatusSource.GetValue());
                String previousStatus = Convert.ToString(routeStatusSource.GetPreviousValue());

                return ((previousStatus == "PARTFILL") && (currentStatus == "CXLREQ"));
            }
        }


        class RouteCancelRejectedOnPartfillFromRequest : RuleEvaluator
        {
            public RouteCancelRejectedOnPartfillFromRequest()
            {
                this.AddDependantDataPointName("RouteStatus");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                EMSXFieldDataPointSource routeStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("RouteStatus").GetSource();

                String currentStatus = Convert.ToString(routeStatusSource.GetValue());
                String previousStatus = Convert.ToString(routeStatusSource.GetPreviousValue());

                return ((previousStatus == "CXLREQ") && (currentStatus == "PARTFILL"));
            }
        }


        class RouteCancelRejectedOnPartfillFromPending : RuleEvaluator
            {
            public RouteCancelRejectedOnPartfillFromPending()
            {
                this.AddDependantDataPointName("RouteStatus");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                EMSXFieldDataPointSource routeStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("RouteStatus").GetSource();

                String currentStatus = Convert.ToString(routeStatusSource.GetValue());
                String previousStatus = Convert.ToString(routeStatusSource.GetPreviousValue());

                return ((previousStatus == "CXLPEN") && (currentStatus == "PARTFILL"));
            }
        }

        class RouteModifyRequestedOnWorking : RuleEvaluator
        {
            public RouteModifyRequestedOnWorking()
            {
                this.AddDependantDataPointName("RouteStatus");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                EMSXFieldDataPointSource routeStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("RouteStatus").GetSource();

                String currentStatus = Convert.ToString(routeStatusSource.GetValue());
                String previousStatus = Convert.ToString(routeStatusSource.GetPreviousValue());

                return ((previousStatus == "WORKING") && (currentStatus == "CXLRPRQ"));
            }
        }


        class RouteModifyBrokerAck : RuleEvaluator
        {
            public RouteModifyBrokerAck()
            {
                this.AddDependantDataPointName("RouteStatus");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                EMSXFieldDataPointSource routeStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("RouteStatus").GetSource();

                String currentStatus = Convert.ToString(routeStatusSource.GetValue());
                String previousStatus = Convert.ToString(routeStatusSource.GetPreviousValue());

                return ((previousStatus == "CXLRPRQ") && (currentStatus == "REPPEN"));
            }
        }


        class RouteModifyRejectedOnWorking : RuleEvaluator
        {
            public RouteModifyRejectedOnWorking()
            {
                this.AddDependantDataPointName("RouteStatus");
                this.AddDependantDataPointName("RouteBrokerStatus");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                EMSXFieldDataPointSource routeStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("RouteStatus").GetSource();
                EMSXFieldDataPointSource routeBrokerStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("RouteBrokerStatus").GetSource();

                String currentStatus = Convert.ToString(routeStatusSource.GetValue());
                String previousStatus = Convert.ToString(routeStatusSource.GetPreviousValue());

                String currentBrokerStatus = Convert.ToString(routeBrokerStatusSource.GetValue());

                return ((previousStatus == "REPPEN") && (currentStatus == "WORKING") && (currentBrokerStatus=="CXLRPRJ"));
            }
        }

        class RouteModifyAppliedOnWorking : RuleEvaluator
        {
            public RouteModifyAppliedOnWorking()
            {
                this.AddDependantDataPointName("RouteStatus");
                this.AddDependantDataPointName("RouteBrokerStatus");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                EMSXFieldDataPointSource routeStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("RouteStatus").GetSource();
                EMSXFieldDataPointSource routeBrokerStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("RouteBrokerStatus").GetSource();

                String currentStatus = Convert.ToString(routeStatusSource.GetValue());
                String previousStatus = Convert.ToString(routeStatusSource.GetPreviousValue());

                String currentBrokerStatus = Convert.ToString(routeBrokerStatusSource.GetValue());

                return ((previousStatus == "REPPEN") && (currentStatus == "WORKING") && (currentBrokerStatus == "MODIFIED"));
            }
        }

        class RouteModifyRequestedOnPartfill : RuleEvaluator
        {
            public RouteModifyRequestedOnPartfill()
            {
                this.AddDependantDataPointName("RouteStatus");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                EMSXFieldDataPointSource routeStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("RouteStatus").GetSource();

                String currentStatus = Convert.ToString(routeStatusSource.GetValue());
                String previousStatus = Convert.ToString(routeStatusSource.GetPreviousValue());

                return ((previousStatus == "PARTFILL") && (currentStatus == "CXLRPRQ"));
            }
        }


        class RouteModifyRejectedOnPartfill : RuleEvaluator
        {
            public RouteModifyRejectedOnPartfill()
            {
                this.AddDependantDataPointName("RouteStatus");
                this.AddDependantDataPointName("RouteBrokerStatus");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                EMSXFieldDataPointSource routeStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("RouteStatus").GetSource();
                EMSXFieldDataPointSource routeBrokerStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("RouteBrokerStatus").GetSource();

                String currentStatus = Convert.ToString(routeStatusSource.GetValue());
                String previousStatus = Convert.ToString(routeStatusSource.GetPreviousValue());

                String currentBrokerStatus = Convert.ToString(routeBrokerStatusSource.GetValue());

                return ((previousStatus == "REPPEN") && (currentStatus == "PARTFILL") && (currentBrokerStatus == "CXLRPRJ"));
            }
        }

        class RouteModifyAppliedOnPartfill : RuleEvaluator
        {
            public RouteModifyAppliedOnPartfill()
            {
                this.AddDependantDataPointName("RouteStatus");
                this.AddDependantDataPointName("RouteBrokerStatus");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                EMSXFieldDataPointSource routeStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("RouteStatus").GetSource();
                EMSXFieldDataPointSource routeBrokerStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("RouteBrokerStatus").GetSource();

                String currentStatus = Convert.ToString(routeStatusSource.GetValue());
                String previousStatus = Convert.ToString(routeStatusSource.GetPreviousValue());

                String currentBrokerStatus = Convert.ToString(routeBrokerStatusSource.GetValue());

                return ((previousStatus == "REPPEN") && (currentStatus == "PARTFILL") && (currentBrokerStatus == "MODIFIED"));
            }
        }

    }
}
