using System;
using com.bloomberg.samples.rulemsx;
using com.bloomberg.emsx.samples;
using Action = com.bloomberg.samples.rulemsx.Action;
using System.Globalization;
using LogRmsx = com.bloomberg.samples.rulemsx.Log;

namespace RMSXEMSXOrderStateIdentifier
{
    class RMSXEMSXOrderStateIdentifier : NotificationHandler
    {

        RuleMSX rmsx;
        EasyMSX emsx;

        static void Main(string[] args)
        {
            RMSXEMSXOrderStateIdentifier Test = new RMSXEMSXOrderStateIdentifier();
            Test.Run();

            System.Console.WriteLine("Press enter to terminate...");
            System.Console.ReadLine();

            Test.Stop();

            System.Console.WriteLine("Terminating.");
        }

        private void log(String msg)
        {
            System.Console.WriteLine(DateTime.Now.ToString("yyyyMMddHHmmssfffzzz") + "(RMSXEMSXOrderStateIdentifier): \t" + msg);
        }

        private void Run()
        {

            log("RMSXEMSXOrderStateIdentifier - Identify state changes in EMSX orders");

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

            this.emsx.orders.addNotificationHandler(this);

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

            log("Creating RuleSet rsOrderStates");
            RuleSet rsOrderStates = this.rmsx.CreateRuleSet("OrderStates");

            log("Creating rule for ORDER_NEW");
            Rule ruleOrderNew = rsOrderStates.AddRule("OrderNew");
            ruleOrderNew.AddRuleCondition(new RuleCondition("OrderNew", new OrderNew()));
            ruleOrderNew.AddAction(this.rmsx.CreateAction("ShowOrderNew", new ShowOrderState(this, "New Order")));

            log("Creating rule for ORDER_FIELD_UPDATE_ON_SENT");
            Rule ruleOrderFieldUpdateOnSent = rsOrderStates.AddRule("OrderFieldUpdateOnSent");
            ruleOrderFieldUpdateOnSent.AddRuleCondition(new RuleCondition("OrderFieldUpdateOnSent", new OrderFieldUpdateOnSent()));
            ruleOrderFieldUpdateOnSent.AddAction(this.rmsx.CreateAction("ShowOrderFieldUpdateOnSent", new ShowOrderState(this, "Field updates on order in sent status")));

            log("Creating rule for ORDER_FILL_OCCURRED");
            Rule ruleOrderFillOccurred = rsOrderStates.AddRule("OrderFillOccurred");
            ruleOrderFillOccurred.AddRuleCondition(new RuleCondition("OrderFillOccurred", new OrderFillOccurred()));
            ruleOrderFillOccurred.AddAction(this.rmsx.CreateAction("ShowOrderFillOccurred", new ShowOrderState(this, "Fill occurred")));

            log("Creating rule for ORDER_BROKER_ACK");
            Rule ruleOrderBrokerAck = rsOrderStates.AddRule("OrderBrokerAck");
            ruleOrderBrokerAck.AddRuleCondition(new RuleCondition("OrderBrokerAck", new OrderBrokerAck()));
            ruleOrderBrokerAck.AddAction(this.rmsx.CreateAction("ShowOrderBrokerAck", new ShowOrderState(this, "Broker acknowledged new route on order")));

            log("Creating rule for ORDER_INIT_PAINT_EXPIRED");
            Rule ruleOrderInitPaintExpired = rsOrderStates.AddRule("OrderInitPaintExpired");
            ruleOrderInitPaintExpired.AddRuleCondition(new RuleCondition("OrderInitPaintExpired", new OrderInitPaintExpired()));
            ruleOrderInitPaintExpired.AddAction(this.rmsx.CreateAction("ShowOrderInitPaintExpired", new ShowOrderState(this, "Initial paint shows order as expired")));

            log("Creating rule for ORDER_INIT_PAINT_WORKING");
            Rule ruleOrderInitPaintWorking = rsOrderStates.AddRule("OrderInitPaintWorking");
            ruleOrderInitPaintWorking.AddRuleCondition(new RuleCondition("OrderInitPaintWorking", new OrderInitPaintWorking()));
            ruleOrderInitPaintWorking.AddAction(this.rmsx.CreateAction("ShowOrderInitPaintWorking", new ShowOrderState(this, "Initial paint shows order as working")));

            log("Creating rule for ORDER_INIT_PAINT_FILLED");
            Rule ruleOrderInitPaintFilled = rsOrderStates.AddRule("OrderInitPaintFilled");
            ruleOrderInitPaintFilled.AddRuleCondition(new RuleCondition("OrderInitPaintFilled", new OrderInitPaintFilled()));
            ruleOrderInitPaintFilled.AddAction(this.rmsx.CreateAction("ShowOrderInitPaintFilled", new ShowOrderState(this, "Initial paint shows order as filled")));

            log("Creating rule for ORDER_INIT_PAINT_ASSIGN");
            Rule ruleOrderInitPaintAssign = rsOrderStates.AddRule("OrderInitPaintAssign");
            ruleOrderInitPaintAssign.AddRuleCondition(new RuleCondition("OrderInitPaintAssign", new OrderInitPaintAssign()));
            ruleOrderInitPaintAssign.AddAction(this.rmsx.CreateAction("ShowOrderInitPaintAssign", new ShowOrderState(this, "Initial paint shows order as assign")));

            log("Creating rule for ORDER_ROUTE_SENT_TO_BROKER");
            Rule ruleOrderRouteSentToBroker = rsOrderStates.AddRule("OrderRouteSentToBroker");
            ruleOrderRouteSentToBroker.AddRuleCondition(new RuleCondition("OrderRouteSentToBroker", new OrderRouteSentToBroker()));
            ruleOrderRouteSentToBroker.AddAction(this.rmsx.CreateAction("ShowOrderRouteSentToBroker", new ShowOrderState(this, "Order has single active route sent to broker")));

            log("Creating rule for ORDER_FULL_FILL");
            Rule ruleOrderFullFill = rsOrderStates.AddRule("OrderFullFill");
            ruleOrderFullFill.AddRuleCondition(new RuleCondition("OrderFullFill", new OrderFullFill()));
            ruleOrderFullFill.AddAction(this.rmsx.CreateAction("ShowOrderFullFill", new ShowOrderState(this, "Order is fully filled")));

            log("Creating rule for ORDER_ROUTE_CANCEL_ON_SENT");
            Rule ruleOrderRouteCancelOnSent = rsOrderStates.AddRule("OrderRouteCancelOnSent");
            ruleOrderRouteCancelOnSent.AddRuleCondition(new RuleCondition("OrderRouteCancelOnSent", new OrderRouteCancelOnSent()));
            ruleOrderRouteCancelOnSent.AddAction(this.rmsx.CreateAction("ShowOrderRouteCancelOnSent", new ShowOrderState(this, "Initial paint shows route working")));

            log("Creating rule for ORDER_PART_FILL_ON_WORKING");
            Rule ruleOrderPartfillOnWorking = rsOrderStates.AddRule("OrderPartfillOnWorking");
            ruleOrderPartfillOnWorking.AddRuleCondition(new RuleCondition("OrderPartfillOnWorking", new OrderPartfillOnWorking()));
            ruleOrderPartfillOnWorking.AddAction(this.rmsx.CreateAction("ShowOrderPartfillOnWorking", new ShowOrderState(this, "Order is part filled. No active route.")));

            log("Creating rule for ORDER_NEW_ROUTE_ON_PARTFILL");
            Rule ruleOrderNewRouteOnPartfill = rsOrderStates.AddRule("OrderNewRouteOnPartfill");
            ruleOrderNewRouteOnPartfill.AddRuleCondition(new RuleCondition("OrderNewRouteOnPartfill", new OrderNewRouteOnPartfill()));
            ruleOrderNewRouteOnPartfill.AddAction(this.rmsx.CreateAction("ShowOrderNewRouteOnPartfill", new ShowOrderState(this, "New active route on partfilled order")));

            log("Creating rule for ORDER_TO ASSIGN");
            Rule ruleOrderToAssign = rsOrderStates.AddRule("OrderToAssign");
            ruleOrderToAssign.AddRuleCondition(new RuleCondition("OrderToAssign", new OrderToAssign()));
            ruleOrderToAssign.AddAction(this.rmsx.CreateAction("ShowOrderToAssign", new ShowOrderState(this, "Single route on order cancelled")));

            log("Creating rule for ORDER_AMOUNT_MODIFY");
            Rule ruleOrderAmountModify = rsOrderStates.AddRule("OrderAmountModify");
            ruleOrderAmountModify.AddRuleCondition(new RuleCondition("OrderAmountModify", new OrderAmountModify()));
            ruleOrderAmountModify.AddAction(this.rmsx.CreateAction("ShowOrderAmountModify", new ShowOrderState(this, "Order amount modified")));

            log("Creating rule for ORDER_AMOUNT_MODIFY_ON_FILLED");
            Rule ruleOrderAmountModifyOnFilled = rsOrderStates.AddRule("OrderAmountModifyOnFilled");
            ruleOrderAmountModifyOnFilled.AddRuleCondition(new RuleCondition("OrderAmountModifyOnFilled", new OrderAmountModifyOnFilled()));
            ruleOrderAmountModifyOnFilled.AddAction(this.rmsx.CreateAction("ShowOrderAmountModifyOnFilled", new ShowOrderState(this, "Order amount modified on filled order")));

            log("Creating rule for ORDER_AMOUNT_MODIFY_TO_FILLED");
            Rule ruleOrderAmountModifyToFilled = rsOrderStates.AddRule("OrderAmountModifyToFilled");
            ruleOrderAmountModifyToFilled.AddRuleCondition(new RuleCondition("OrderAmountModifyToFilled", new OrderAmountModifyToFilled()));
            ruleOrderAmountModifyToFilled.AddAction(this.rmsx.CreateAction("ShowOrderAmountModifyToFilled", new ShowOrderState(this, "Order amount modified to filled amount")));

            log("Rules built.");

        }

        public void ProcessNotification(Notification notification)
        {

            if (notification.category == Notification.NotificationCategory.ORDER)
            {
                if (notification.type != Notification.NotificationType.UPDATE)
                {
                    this.parseOrder(notification.getOrder());
                }
            }
        }

        public void parseOrder(Order o)
        {
            DataSet newDataSet = this.rmsx.CreateDataSet("DS_OR_" + o.field("EMSX_SEQUENCE").value());
            newDataSet.AddDataPoint("OrderStatus", new EMSXFieldDataPointSource(o.field("EMSX_STATUS")));
            newDataSet.AddDataPoint("OrderNumber", new EMSXFieldDataPointSource(o.field("EMSX_SEQUENCE")));
            newDataSet.AddDataPoint("OrderWorking", new EMSXFieldDataPointSource(o.field("EMSX_WORKING")));
            newDataSet.AddDataPoint("OrderAmount", new EMSXFieldDataPointSource(o.field("EMSX_AMOUNT")));
            this.rmsx.GetRuleSet("OrderStates").Execute(newDataSet);
        }


        class EMSXFieldDataPointSource : DataPointSource, NotificationHandler
        {
            Field field;
            String value;

            internal EMSXFieldDataPointSource(Field field)
            {
                this.field = field;
                this.value = field.value();

                this.field.addNotificationHandler(this);

            }

            public override object GetValue()
            {
                return this.field.value();
            }

            public object GetPreviousValue()
            {
                return this.field.previousValue();
            }

            public void ProcessNotification(Notification notification)
            {
                if (this.field.previousValue() != this.field.value()) this.SetStale();
            }
        }


        class ShowOrderState : ActionExecutor
        {
            RMSXEMSXOrderStateIdentifier parent;
            String state = "";
            public ShowOrderState(RMSXEMSXOrderStateIdentifier parent, String state)
            {
                this.parent = parent;
                this.state = state;
            }

            public void Execute(DataSet dataSet)
            {
                this.parent.log("Order " + dataSet.GetDataPoint("OrderNumber").GetValue() + ": " + this.state);
            }
        }

        class OrderNew : RuleEvaluator
        {
            public OrderNew()
            {
                this.AddDependantDataPointName("OrderStatus");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                EMSXFieldDataPointSource orderStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("OrderStatus").GetSource();

                String currentStatus = Convert.ToString(orderStatusSource.GetValue());
                String previousStatus = Convert.ToString(orderStatusSource.GetPreviousValue());

                return (previousStatus == "") && (currentStatus == "NEW");
            }
        }

        class OrderFieldUpdateOnSent : RuleEvaluator
        {
            public OrderFieldUpdateOnSent()
            {
                this.AddDependantDataPointName("OrderStatus");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                EMSXFieldDataPointSource orderStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("OrderStatus").GetSource();

                String currentStatus = Convert.ToString(orderStatusSource.GetValue());
                String previousStatus = Convert.ToString(orderStatusSource.GetPreviousValue());

                return (previousStatus == "SENT") && (currentStatus == "SENT");
            }
        }

        class OrderFillOccurred : RuleEvaluator
        {
            public OrderFillOccurred()
            {
                this.AddDependantDataPointName("OrderStatus");
                this.AddDependantDataPointName("OrderWorking");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                EMSXFieldDataPointSource orderStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("OrderStatus").GetSource();
                EMSXFieldDataPointSource orderWorkingSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("OrderWorking").GetSource();

                String currentStatus = Convert.ToString(orderStatusSource.GetValue());
                String previousStatus = Convert.ToString(orderStatusSource.GetPreviousValue());

                int currentWorking = Convert.ToInt32(orderWorkingSource.GetValue());
                int previousWorking = 0;

                try
                {
                    previousWorking = Convert.ToInt32(orderWorkingSource.GetPreviousValue());
                }
                catch
                {
                    previousWorking = 0;
                }

                return ((previousWorking > currentWorking) && (previousStatus == "WORKING") && (currentStatus == "WORKING"));

            }
        }

        class OrderBrokerAck : RuleEvaluator
        {
            public OrderBrokerAck()
            {
                this.AddDependantDataPointName("OrderStatus");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                EMSXFieldDataPointSource orderStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("OrderStatus").GetSource();

                String currentStatus = Convert.ToString(orderStatusSource.GetValue());
                String previousStatus = Convert.ToString(orderStatusSource.GetPreviousValue());

                return (previousStatus == "SENT") && (currentStatus == "WORKING");
            }
        }

        class OrderInitPaintExpired : RuleEvaluator
        {
            public OrderInitPaintExpired()
            {
                this.AddDependantDataPointName("OrderStatus");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                EMSXFieldDataPointSource orderStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("OrderStatus").GetSource();

                String currentStatus = Convert.ToString(orderStatusSource.GetValue());
                String previousStatus = Convert.ToString(orderStatusSource.GetPreviousValue());

                return (previousStatus == "") && (currentStatus == "EXPIRED");
            }
        }

        class OrderInitPaintWorking : RuleEvaluator
        {
            public OrderInitPaintWorking()
            {
                this.AddDependantDataPointName("OrderStatus");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                EMSXFieldDataPointSource orderStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("OrderStatus").GetSource();

                String currentStatus = Convert.ToString(orderStatusSource.GetValue());
                String previousStatus = Convert.ToString(orderStatusSource.GetPreviousValue());

                return (previousStatus == "") && (currentStatus == "WORKING");
            }
        }

        class OrderInitPaintFilled : RuleEvaluator
        {
            public OrderInitPaintFilled()
            {
                this.AddDependantDataPointName("OrderStatus");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                EMSXFieldDataPointSource orderStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("OrderStatus").GetSource();

                String currentStatus = Convert.ToString(orderStatusSource.GetValue());
                String previousStatus = Convert.ToString(orderStatusSource.GetPreviousValue());

                return (previousStatus == "") && (currentStatus == "FILLED");
            }
        }

        class OrderInitPaintAssign : RuleEvaluator
        {
            public OrderInitPaintAssign()
            {
                this.AddDependantDataPointName("OrderStatus");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                EMSXFieldDataPointSource orderStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("OrderStatus").GetSource();

                String currentStatus = Convert.ToString(orderStatusSource.GetValue());
                String previousStatus = Convert.ToString(orderStatusSource.GetPreviousValue());

                return (previousStatus == "") && (currentStatus == "ASSIGN");
            }
        }

        class OrderRouteSentToBroker : RuleEvaluator
        {
            public OrderRouteSentToBroker()
            {
                this.AddDependantDataPointName("OrderStatus");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                EMSXFieldDataPointSource orderStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("OrderStatus").GetSource();

                String currentStatus = Convert.ToString(orderStatusSource.GetValue());
                String previousStatus = Convert.ToString(orderStatusSource.GetPreviousValue());

                return (previousStatus == "NEW") && (currentStatus == "SENT");
            }
        }

        class OrderFullFill : RuleEvaluator
        {
            public OrderFullFill()
            {
                this.AddDependantDataPointName("OrderStatus");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                EMSXFieldDataPointSource orderStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("OrderStatus").GetSource();

                String currentStatus = Convert.ToString(orderStatusSource.GetValue());
                String previousStatus = Convert.ToString(orderStatusSource.GetPreviousValue());

                return (previousStatus == "WORKING") && (currentStatus == "FILLED");
            }
        }

        class OrderRouteCancelOnSent : RuleEvaluator
        {
            public OrderRouteCancelOnSent()
            {
                this.AddDependantDataPointName("OrderStatus");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                EMSXFieldDataPointSource orderStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("OrderStatus").GetSource();

                String currentStatus = Convert.ToString(orderStatusSource.GetValue());
                String previousStatus = Convert.ToString(orderStatusSource.GetPreviousValue());

                return (previousStatus == "SENT") && (currentStatus == "ASSIGN");
            }
        }

        class OrderPartfillOnWorking : RuleEvaluator
        {
            public OrderPartfillOnWorking()
            {
                this.AddDependantDataPointName("OrderStatus");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                EMSXFieldDataPointSource orderStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("OrderStatus").GetSource();

                String currentStatus = Convert.ToString(orderStatusSource.GetValue());
                String previousStatus = Convert.ToString(orderStatusSource.GetPreviousValue());

                return (previousStatus == "WORKING") && (currentStatus == "PARTFILL");
            }
        }


        class OrderNewRouteOnPartfill : RuleEvaluator
        {
            public OrderNewRouteOnPartfill()
            {
                this.AddDependantDataPointName("OrderStatus");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                EMSXFieldDataPointSource orderStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("OrderStatus").GetSource();

                String currentStatus = Convert.ToString(orderStatusSource.GetValue());
                String previousStatus = Convert.ToString(orderStatusSource.GetPreviousValue());

                return (previousStatus == "PARTFILL") && (currentStatus == "SENT");
            }
        }


        class OrderToAssign : RuleEvaluator
        {
            public OrderToAssign()
            {
                this.AddDependantDataPointName("OrderStatus");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                EMSXFieldDataPointSource orderStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("OrderStatus").GetSource();

                String currentStatus = Convert.ToString(orderStatusSource.GetValue());
                String previousStatus = Convert.ToString(orderStatusSource.GetPreviousValue());

                return (previousStatus == "WORKING") && (currentStatus == "ASSIGN");
            }
        }

        class OrderAmountModify : RuleEvaluator
        {
            public OrderAmountModify()
            {
                this.AddDependantDataPointName("OrderStatus");
                this.AddDependantDataPointName("OrderAmount");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                EMSXFieldDataPointSource orderStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("OrderStatus").GetSource();
                EMSXFieldDataPointSource orderAmountSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("OrderAmount").GetSource();

                String currentStatus = Convert.ToString(orderStatusSource.GetValue());
                String previousStatus = Convert.ToString(orderStatusSource.GetPreviousValue());

                int currentAmount = Convert.ToInt32(orderAmountSource.GetValue());
                int previousAmount= 0;

                try
                {
                    previousAmount = Convert.ToInt32(orderAmountSource.GetPreviousValue());
                }
                catch
                {
                    previousAmount = 0;
                }

                return ((previousAmount != currentAmount) && (previousStatus == currentStatus));
            }
        }


        class OrderAmountModifyOnFilled : RuleEvaluator
        {
            public OrderAmountModifyOnFilled()
            {
                //this.AddDependantDataPointName("OrderStatus");
                this.AddDependantDataPointName("OrderAmount");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                EMSXFieldDataPointSource orderStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("OrderStatus").GetSource();
                EMSXFieldDataPointSource orderAmountSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("OrderAmount").GetSource();

                String currentStatus = Convert.ToString(orderStatusSource.GetValue());
                String previousStatus = Convert.ToString(orderStatusSource.GetPreviousValue());

                int currentAmount = Convert.ToInt32(orderAmountSource.GetValue());
                int previousAmount = 0;

                try
                {
                    previousAmount = Convert.ToInt32(orderAmountSource.GetPreviousValue());
                }
                catch
                {
                    previousAmount = 0;
                }

                return ((previousAmount != currentAmount) && (previousStatus == "FILLED" && currentStatus=="PARTFILL"));
            }
        }


        class OrderAmountModifyToFilled : RuleEvaluator
        {
            public OrderAmountModifyToFilled()
            {
                //this.AddDependantDataPointName("OrderStatus");
                this.AddDependantDataPointName("OrderAmount");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                EMSXFieldDataPointSource orderStatusSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("OrderStatus").GetSource();
                EMSXFieldDataPointSource orderAmountSource = (EMSXFieldDataPointSource)dataSet.GetDataPoint("OrderAmount").GetSource();

                String currentStatus = Convert.ToString(orderStatusSource.GetValue());
                String previousStatus = Convert.ToString(orderStatusSource.GetPreviousValue());

                int currentAmount = Convert.ToInt32(orderAmountSource.GetValue());
                int previousAmount = 0;

                try
                {
                    previousAmount = Convert.ToInt32(orderAmountSource.GetPreviousValue());
                }
                catch
                {
                    previousAmount = 0;
                }

                return ((previousAmount != currentAmount) && (previousStatus == "PARTFILL" && currentStatus == "FILLED"));
            }
        }

    }
}
