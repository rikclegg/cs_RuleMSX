using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.bloomberg.samples.rulemsx
{
    class WorkingRule
    {
        ExecutionAgent agent;
        Rule rule;
        internal DataSet dataSet;
        internal List<RuleEvaluator> evaluators = new List<RuleEvaluator>();
        internal List<ActionExecutor> executors = new List<ActionExecutor>();

        internal WorkingRule(Rule rule, DataSet dataSet, ExecutionAgent agent) {
            Log.LogMessage(Log.LogLevels.DETAILED, "WorkingRule constructor for Rule: " + rule.GetName() + " and DataSet: " + dataSet.getName());
            this.agent = agent;
            this.rule = rule;
            this.dataSet = dataSet;
            Dereference();
        }

        private void Dereference()
        {
            Log.LogMessage(Log.LogLevels.DETAILED, "Dereferencing WorkingRule for Rule: " + rule.GetName() + " and DataSet: " + dataSet.getName());

            foreach(Action a in rule.GetActions())
            {
                this.executors.Add(a.GetExecutor());
            }

            foreach(RuleCondition c in rule.GetConditions())
            {
                RuleEvaluator e = c.GetEvaluator();
                this.evaluators.Add(e);

                foreach(String dpn in e.dependantDataPointNames)
                {
                    DataPoint dp = this.dataSet.getDataPoint(dpn);
                    dp.GetSource().AssociateWorkingRule(this);
                }
            }
        }

        internal Rule getRule() {
            return this.rule;
        }

        internal void EnqueueWorkingRule()
        {
            this.agent.EnqueueWorkingRule(this);
        }
    }
}
