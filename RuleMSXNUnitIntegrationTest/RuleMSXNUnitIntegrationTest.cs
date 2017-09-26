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
        public void SingleRuleITest()
        {
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

            rs.Execute(ds);

            GenericAction rao = (GenericAction)rai;
            Assert.That(rao.getOutgoing, Is.EqualTo(actionMessage));
        }

        private class GenericBoolRule : RuleEvaluator
        {
            bool ret;

            public GenericBoolRule(bool returnValue)
            {
                this.ret = returnValue;
            }

            public override bool Evaluate(DataSet dataSet)
            {
                return this.ret;
            }
        }

        private class GenericAction : RuleAction
        {
            string message;
            string outgoing;

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

    }
}
