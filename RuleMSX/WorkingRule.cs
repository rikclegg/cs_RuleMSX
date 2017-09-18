using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.bloomberg.samples.rulemsx
{
    class WorkingRule : RuleEventHandler
    {
        ExecutionAgent agent;
        Rule rule;
        internal DataSet dataSet;
        internal RuleEvaluator evaluator;
        internal List<RuleAction> actions = new List<RuleAction>();
        internal List<WorkingRule> workingRules = new List<WorkingRule>();

        internal WorkingRule(ExecutionAgent agent, Rule rule, DataSet dataSet) {
            this.agent = agent;
            this.rule = rule;
            this.dataSet = dataSet;

            dereference();
        }

        private void dereference()
        {
            this.actions = rule.GetActions();
            this.evaluator = rule.GetEvaluator();
            
            foreach(string dependencyName in this.evaluator.dependantDataPointNames)
            {
                // Find this dependency in the current dataSet
                DataPoint dp = this.dataSet.getDataPoint(dependencyName);
                dp.GetSource().addRuleEventHandler(this);
            }
        }

        internal Rule getRule() {
            return this.rule;
        }

        internal void addWorkingRule(WorkingRule wr) {
            this.workingRules.Add(wr);
        }

        public void handleRuleEvent()
        {
            agent.AddToOpenSetQueue(this);
        }
    }
}
