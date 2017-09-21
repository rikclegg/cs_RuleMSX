using com.bloomberg.samples.rulemsx;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace RuleMSXNUnitTest
{

    [TestFixture]
    public class RuleMSXNUnitTest

    {
        [Test]
        public void InstantiateRuleMSXEmptyConstGivesEmptyRuleandDataSets()
        {
            RuleMSX rmsx = new RuleMSX();
            Assert.That(rmsx.getRuleSets().Count, Is.EqualTo(0));
            Assert.That(rmsx.getDataSets().Count, Is.EqualTo(0));
        }

        [Test]
        public void GetDataSetsReturnsEmptyDataSetList()
        {
            RuleMSX rmsx = new RuleMSX();
            List<DataSet> dataSets = rmsx.getDataSets();
            Assert.That(dataSets.Count, Is.EqualTo(0));
        }

        [Test]
        public void GetRuleSetsReturnsEmptyRuleSetList()
        {
            RuleMSX rmsx = new RuleMSX();
            List<RuleSet> ruleSets = rmsx.getRuleSets();
            Assert.That(ruleSets.Count, Is.EqualTo(0));
        }

        [Test]
        public void CreateDataSetReturnsNewDataSet()
        {
            RuleMSX rmsx = new RuleMSX();
            string newDataSetName = "NewDataSet";
            DataSet dataSet = rmsx.createDataSet(newDataSetName);
            Assert.That(dataSet.getName(), Is.EqualTo(newDataSetName));
        }

        [Test]
        public void CreateRuleSetReturnsNewRuleSet()
        {
            RuleMSX rmsx = new RuleMSX();
            string newRuleSetName = "NewRuleSet";
            RuleSet ruleSet = rmsx.createRuleSet(newRuleSetName);
            Assert.That(ruleSet.getName(), Is.EqualTo(newRuleSetName));
        }

        [Test]
        public void CreateDataSetWithEmptyNameFails()
        {
            RuleMSX rmsx = new RuleMSX();
            string newDataSetName = "";
            Assert.Throws<ArgumentException>(() => rmsx.createDataSet(newDataSetName));
        }

        [Test]
        public void CreateDataSetWithNullNameFails()
        {
            RuleMSX rmsx = new RuleMSX();
            string newDataSetName = null;
            Assert.Throws<ArgumentException>(() => rmsx.createDataSet(newDataSetName));
        }

        [Test]
        public void CreateRuleSetWithEmptyNameFails()
        {
            RuleMSX rmsx = new RuleMSX();
            string newRuleSetName = "";
            Assert.Throws<ArgumentException>(() => rmsx.createRuleSet(newRuleSetName));
        }

        [Test]
        public void CreateRuleSetWithNullNameFails()
        {
            RuleMSX rmsx = new RuleMSX();
            string newRuleSetName = "";
            Assert.Throws<ArgumentException>(() => rmsx.createRuleSet(newRuleSetName));
        }

        [Test]
        public void RuleMSXSTopShouldReturnTrueWithNoRuleSets()
        {
            RuleMSX rmsx = new RuleMSX();
            Assert.That(rmsx.Stop(), Is.EqualTo(true));
        }

        [Test]
        public void RuleMSXStopShouldReturnTrueWithActiveRuleSet()
        {
            RuleMSX rmsx = new RuleMSX();
            RuleSet rs = rmsx.createRuleSet("TestRuleSet");
            Rule r = new Rule("Rule1", new GenericRule(true));
            DataSet ds = rmsx.createDataSet("TestDataSet");
            rs.AddRule(r);
            rs.Execute(ds);
            Assert.That(rmsx.Stop(), Is.EqualTo(true));
        }

        private class GenericRule : RuleEvaluator
        {
            bool ret;

            public GenericRule(bool returnValue)
            {
                this.ret = returnValue;
            }

            public override bool Evaluate(DataSet dataSet)
            {
                return this.ret;
            }
        }

        [Test]
        public void RuleMSXStopShouldReturnTrueWithUnexecutedRuleSet()
        {
            RuleMSX rmsx = new RuleMSX();
            RuleSet rs = rmsx.createRuleSet("TestRuleSet");
            Rule r = new Rule("Rule1", new GenericRule(true));
            DataSet ds = rmsx.createDataSet("TestDataSet");
            rs.AddRule(r);
            // We are deliberately not executing the ruleset...
            //rs.Execute(ds);
            Assert.That(rmsx.Stop(), Is.EqualTo(true));
        }

        [Test]
        public void RuleSetGetNameShouldReturnName()
        {
            RuleMSX rmsx = new RuleMSX();
            RuleSet rs = rmsx.createRuleSet("TestRuleSet");
            Assert.That(rs.getName(), Is.EqualTo("TestRuleSet"));                   
        }
    }
}

