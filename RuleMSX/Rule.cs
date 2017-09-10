namespace com.bloomberg.samples.rulemsx {

    public class Rule : RuleContainer {

        private string name;
        private RuleEvaluator evaluator;
        private List<RuleAction> actions = new List<RuleAction>();
        private List<String> dependencies = new List<String>();

        public Rule(string name, RuleEvaluator evaluator)
        {
            this.name = name;
            this.evaluator = evaluator;
        }

        public Rule(string name, RuleEvaluator evaluator, RuleAction action)
        {
            this.name = name;
            this.evaluator = evaluator;
            this.actions.add(action);
        }

        public string GetName()
        {
            return this.name;
        }

        public RuleEvaluator GetEvaluator()
        {
            return this.evaluator;
        }

        public List<RuleAction> GetActions()
        {
            return actions;
        }

        public void AddDependency(string dataPointName)
        {
            this.dependencies.add(dataPointName);
        }
    }
}
