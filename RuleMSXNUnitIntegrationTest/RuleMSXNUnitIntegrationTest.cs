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

            rs.AddRule(new Rule("TestRule1", new GenericBoolRule(true)));
            rs.AddRule(new Rule("TestRule2", new GenericBoolRule(false)));
            rs.AddRule(new Rule("TestRule3", new GenericBoolRule(false)));
            rs.AddRule(new Rule("TestRule4", new GenericBoolRule(true)));

            string report = rs.report();

            System.Console.WriteLine(report);

            Assert.That(report.Length, Is.EqualTo(241));

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

            RuleAction rai = new GenericAction(actionMessage);

            Rule r = new Rule(newRuleName, new GenericBoolRule(true), rai);
            rs.AddRule(r);

            ds.addDataPoint("TestDataPoint", new StringDataPoint("test_data_point"));

            rs.Execute(ds);

            System.Threading.Thread.Sleep(500);

            GenericAction rao = (GenericAction)rai;
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

        private class GenericAction : RuleAction
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
    }
}
