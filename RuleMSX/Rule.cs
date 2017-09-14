using System.Collections.Generic;

namespace com.bloomberg.samples.rulemsx {

    public class Rule : RuleContainer {

        private string name;
        private RuleEvaluator evaluator;
        private List<RuleAction> actions = new List<RuleAction>();
        private List<string> dependencies = new List<string>();
        private WorkingRule workingRule;

        public Rule(string name, RuleEvaluator evaluator)
        {
            this.name = name;
            this.evaluator = evaluator;
        }

        public Rule(string name, RuleEvaluator evaluator, RuleAction action)
        {
            this.name = name;
            this.evaluator = evaluator;
            AddAction(action);
        }

        public void AddAction(RuleAction action)
        {
            this.actions.Add(action);
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
            this.dependencies.Add(dataPointName);
        }

        internal void setWorkingRule(WorkingRule wr) {
            this.workingRule = wr;
        }

        internal WorkingRule getWorkingRule() {
            return this.workingRule;
        }
    }
}
