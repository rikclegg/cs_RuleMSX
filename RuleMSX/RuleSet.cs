using System.Collections.Generic;

namespace com.bloomberg.samples.rulemsx {

    public class RuleSet : RuleContainer {

        private string name;
        private ExecutionAgent executionAgent = null;

        internal RuleSet(string name) {
            this.name = name;
        }

        public string getName() {
            return this.name;
        }

        public void Execute(DataSet dataSet) {

            if (this.executionAgent == null) {
                this.executionAgent = new ExecutionAgent(this, dataSet);
            } else {
                this.executionAgent.addDataSet(dataSet);
            }
        }

        /*
        public void execute(DataSet dataSet) {

            // Create WorkingSet
            this.workingSet = new List<WorkingRule>();

            // Create Working Rules
            foreach (Rule r in this.rules) {
                WorkingRule wr = new WorkingRule(r, r.GetEvaluator(), r.GetActions());
                r.setWorkingRule(wr);
                this.workingSet.Add(wr);
            }

            // Add child working rules to each working rule
            foreach (WorkingRule wr in this.workingSet) {
                foreach(Rule r in wr.getRule().GetRules()) {
                    wr.addWorkingRule(r.getWorkingRule());
                }
            }

            while (open.Count > 0) executeOpen(open, dataSet);
        }

        private void executeOpen(List<Rule> source, DataSet dataSet) {

            List<Rule> newOpen = new List<Rule>();

            foreach (Rule r in source) {
                if (r.GetEvaluator().Evaluate(dataSet)) {
                    foreach (Rule c in r.rules) newOpen.Add(c);
                    foreach (RuleAction a in r.GetActions()) a.Execute(dataSet);
                }
            }

            this.open = newOpen;
        }
        */
    }
}