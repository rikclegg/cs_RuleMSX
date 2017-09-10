namespace com.bloomberg.samples.rulemsx {

    public class RuleSet : RuleContainer {

        private string name;

        internal RuleSet(string name) {
            this.name = name;
        }

        public string getName() {
            return this.name;
        }

        public void execute(DataSet dataSet) {
            executeRules(this, dataSet);
        }

        private void executeRules(RuleContainer source, DataSet dataSet) {

            foreach (Rule r in source.rules) {

                if (r.GetEvaluator().Evaluate(dataSet)) {

                    foreach (RuleAction a in r.GetActions()) {
                        a.Execute(dataSet);
                    }
                    executeRules(r, dataSet);
                }
            }
        }
    }
}