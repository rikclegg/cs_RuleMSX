using com.bloomberg.samples.rulemsx;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace RuleMSXNUnitIntegrationTest
{
    [TestFixture]
    public class RuleMSXNUnitIntegrationTest
    {

        [Test]
        public void RuleSetReportingSizeCheckSingleRule()
        {
            RuleMSX rmsx = new RuleMSX();

            string newRuleSetName = "TestRuleSet";
            string newDataSetName = "TestDataSet";
            string newRuleName = "IsBooleanTrue";

            RuleSet rs = rmsx.createRuleSet(newRuleSetName);
            DataSet ds = rmsx.createDataSet(newDataSetName);

            Rule r = new Rule(newRuleName, new GenericBoolRule(true));
            rs.AddRule(r);

            string report = rs.report();

            System.Console.WriteLine(report);

            Assert.That(report.Length, Is.EqualTo(126));

        }

        [Test]
        public void RuleSetReportingSizeCheckMultiTop()
        {
            RuleMSX rmsx = new RuleMSX();

            string newRuleSetName = "TestRuleSet";

            RuleSet rs = rmsx.createRuleSet(newRuleSetName);

            rs.AddRule(new Rule("TestRule1", new GenericBoolRule(true), rmsx.createAction("TestAction1", new GenericAction("TestAction1"))));
            Rule r2 = new Rule("TestRule2", new GenericBoolRule(false), rmsx.createAction("TestAction4", new GenericAction("TestAction4")));
            rs.AddRule(r2);
            r2.AddRule(new Rule("TestRule5", new GenericBoolRule(true), rmsx.createAction("TestAction3", new GenericAction("TestAction3"))));
            r2.AddRule(new Rule("TestRule6", new GenericBoolRule(false), rmsx.createAction("TestAction5", new GenericAction("TestAction5"))));
            rs.AddRule(new Rule("TestRule3", new GenericBoolRule(false)));
            Rule r4 = new Rule("TestRule4", new GenericBoolRule(true), rmsx.createAction("TestAction2", new GenericAction("TestAction2")));
            rs.AddRule(r4);
            r4.AddRule(new Rule("TestRule7", new GenericBoolRule(false), rmsx.createAction("TestAction6", new GenericAction("TestAction6"))));
            r4.AddRule(new Rule("TestRule8", new GenericBoolRule(true), rmsx.createAction("TestAction7", new GenericAction("TestAction7"))));

            string report = rs.report();

            System.Console.WriteLine(report);

            Assert.That(report.Length, Is.EqualTo(664));

        }


        [Test]
        public void SingleRuleITest()
        {

            Log.logLevel = Log.LogLevels.DETAILED;
            RuleMSX rmsx = new RuleMSX();

            string newRuleSetName = "SingleIterationRuleSet";
            string newDataSetName = "SingleIterationDataSet";
            string newRuleName = "IsBooleanTrue";
            string actionMessage = "ActionMessage";

            RuleSet rs = rmsx.createRuleSet(newRuleSetName);
            DataSet ds = rmsx.createDataSet(newDataSetName);

            RuleAction rai = rmsx.createAction("RuleActionIn", new GenericAction(actionMessage));

            Rule r = new Rule(newRuleName, new GenericBoolRule(true), rai);
            rs.AddRule(r);

            ds.addDataPoint("TestDataPoint", new StringDataPoint("test_data_point"));

            rs.Execute(ds);

            // We have to wait for the action to take place. Under normal circumstances there 
            // would be no interation with the data from outside the evaluators/actions.
            System.Threading.Thread.Sleep(100);

            GenericAction rao = (GenericAction)rai.getExecutor();
            Assert.That(rao.getOutgoing(), Is.EqualTo(actionMessage));
        }

        private class GenericBoolRule : RuleEvaluator
        {
            bool ret;

            public GenericBoolRule(bool returnValue)
            {
                this.ret = returnValue;
                if (returnValue)
                {
                    this.addDependantDataPointName("TestDependency1");
                    this.addDependantDataPointName("TestDependency2");
                }
            }

            public override bool Evaluate(DataSet dataSet)
            {
                return this.ret;
            }
        }

        private class GenericAction : ActionExecutor
        {
            string message;
            string outgoing="unset";

            public GenericAction(string message)
            {
                this.message = message;
            }
            public void Execute(DataSet dataSet)
            {
                this.outgoing = message;
            }

            public string getOutgoing()
            {
                return this.outgoing;
            }
        }

        private class StringDataPoint : DataPointSource
        {
            private string val;

            public StringDataPoint(string val)
            {
                this.val = val;
            }
            public override object GetValue()
            {
                return val;
            }
        }

        [Test]
        public void CounterTest()
        {

            Log.logLevel = Log.LogLevels.BASIC;

            RuleMSX rmsx = new RuleMSX();

            RuleSet ruleSet = rmsx.createRuleSet("CounterTestRuleSet");
            DataSet dataSet = rmsx.createDataSet("CounterTestDataSet");

            dataSet.addDataPoint("counter", new GenericIntDataPointSource(0));

            int maxVal = 10000;

            RuleAction counterSignalAndStep10 = rmsx.createAction("CounterSignalAndStep10", new CounterSignalAndStep(10));
            RuleAction counterSignalAndStep100 = rmsx.createAction("CounterSignalAndStep100", new CounterSignalAndStep(100));
            RuleAction counterSignalAndStep1000 = rmsx.createAction("CounterSignalAndStep1000", new CounterSignalAndStep(1000));
            RuleAction counterSignalAndStepMax = rmsx.createAction("CounterSignalAndStepMax", new CounterSignalAndStep(maxVal));
            EqualSignal equalSignal = new EqualSignal();
            RuleAction equalSignalMax = rmsx.createAction("EqualSignalMax", equalSignal);

            Rule lessThan10 = new Rule("LessThan10", new IntMinMaxEvaluator(0, 9),counterSignalAndStep10);
            Rule greaterThanOrEqualTo10LessThan100 = new Rule("GreaterThanOrEqualTo10LessThan100", new IntMinMaxEvaluator(10, 99), counterSignalAndStep100);
            Rule greaterThanOrEqualTo100LessThan1000 = new Rule("GreaterThanOrEqualTo100LessThan1000", new IntMinMaxEvaluator(100, 999), counterSignalAndStep1000);
            Rule greaterThanOrEqualTo1000LessThanMax = new Rule("GreaterThanOrEqualTo1000LessThanMax", new IntMinMaxEvaluator(1000, maxVal-1), counterSignalAndStepMax);
            Rule equalMax = new Rule("EqualMax", new EqualEvaluator(maxVal), equalSignalMax);

            ruleSet.AddRule(lessThan10);
            ruleSet.AddRule(greaterThanOrEqualTo10LessThan100);
            ruleSet.AddRule(greaterThanOrEqualTo100LessThan1000);
            ruleSet.AddRule(greaterThanOrEqualTo1000LessThanMax);
            ruleSet.AddRule(equalMax);

            System.Console.WriteLine(ruleSet.report());

            ruleSet.Execute(dataSet);

            int maxMS = 400000;
            int step = 10;
            while(maxMS > 0)
            {
                if (equalSignal.fired)
                {
                    System.Console.WriteLine("Target reached");
                    break;
                }
                System.Threading.Thread.Sleep(step);
                maxMS -= step;
            }

            if(maxMS==0) System.Console.WriteLine("Timeout");

            ruleSet.Stop();

            System.Console.WriteLine("Execution Time (ms): " + ruleSet.GetLastCycleExecutionTime());

            Assert.That(equalSignal.fired, Is.EqualTo(true));
        }

        private class GenericIntDataPointSource : DataPointSource
        {
            int val;

            public GenericIntDataPointSource(int initVal)
            {
                this.val = initVal;
            }

            public override object GetValue()
            {
                return this.val;
            }

            public void SetVal(int newValue)
            {
                this.val = newValue;
                this.SetStale();
            }
        }

        private class IntMinMaxEvaluator : RuleEvaluator
        {
            int min;
            int max;

            public IntMinMaxEvaluator(int min, int max)
            {
                this.min = min;
                this.max = max;
                this.addDependantDataPointName("counter");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                int counter = (int)dataSet.getDataPoint("counter").GetValue();
                if (counter >= this.min && counter <= this.max) return true;
                return false;
            }
        }

        private class EqualEvaluator : RuleEvaluator
        {
            int val;

            public EqualEvaluator(int val)
            {
                this.val = val;
                this.addDependantDataPointName("counter");
            }

            public override bool Evaluate(DataSet dataSet)
            {
                int counter = (int)dataSet.getDataPoint("counter").GetValue();
                if (counter == this.val) return true;
                return false;
            }
        }

        private class CounterSignalAndStep : ActionExecutor
        {
            int boundary;
            volatile bool crossed = false;

            public CounterSignalAndStep(int boundary)
            {
                this.boundary = boundary;
            }

            public void Execute(DataSet dataSet)
            {
                GenericIntDataPointSource counter = (GenericIntDataPointSource)dataSet.getDataPoint("counter").GetSource();

                if (!this.crossed)
                {
                    counter.SetVal((int)counter.GetValue() + 1);
                    if ((int)counter.GetValue() >= this.boundary)
                    {
                        this.crossed = true;
                        System.Console.WriteLine("Counter is >= " + boundary.ToString());
                    }
                    else
                    {
                        // System.Console.WriteLine("Counter value is now: " + counter.GetValue());

                    }
                }
            }
        }

        private class EqualSignal : ActionExecutor
        {
            public volatile bool fired = false;

            public void Execute(DataSet dataSet)
            {

                if (!fired)
                {
                    System.Console.WriteLine("Counter value equals Maximum");
                    fired = true;
                }
            }
        }

    }
}
