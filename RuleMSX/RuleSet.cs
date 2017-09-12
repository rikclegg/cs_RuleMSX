using System.Collections.Generic;

namespace com.bloomberg.samples.rulemsx {

    public class RuleSet : RuleContainer {

        private string name;
        private List<Rule> open;

        internal RuleSet(string name) {
            this.name = name;
            this.open = new List<Rule>();
        }

        public string getName() {
            return this.name;
        }

        public void execute(DataSet dataSet) {

            // Move Alpha rules into open
            foreach (Rule r in this.rules) {
                open.Add(r);
            }

            while(open.Count > 0) executeOpen(open, dataSet);
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
    }
}