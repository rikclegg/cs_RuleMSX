namespace com.bloomberg.samples.rulemsx {

    public class RuleSet : RuleContainer {

        private string name;

        RuleSet(string name) {
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

                if (r.getEvaluator().evaluate(dataSet)) {

                    foreach (RuleAction a in r.getActions()) {
                        a.execute(dataSet);
                    }
                    executeRules(r, dataSet);
                }
            }
        }
    }
}