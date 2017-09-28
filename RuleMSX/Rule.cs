using System.Collections.Generic;

namespace com.bloomberg.samples.rulemsx {

    public class Rule : RuleContainer {

        private string name;
        private RuleEvaluator evaluator;
        private List<ActionExecutor> actions = new List<ActionExecutor>();

        public Rule(string name, RuleEvaluator evaluator)
        {
            Log.LogMessage(Log.LogLevels.DETAILED, "Rule Constructor: " + name);
            this.name = name;
            this.evaluator = evaluator;
        }

        public Rule(string name, RuleEvaluator evaluator, ActionExecutor action)
        {
            Log.LogMessage(Log.LogLevels.DETAILED, "Rule Constructor (with Action): " + name);
            this.name = name;
            this.evaluator = evaluator;
            AddAction(action);
        }

        public void AddAction(ActionExecutor action)
        {
            Log.LogMessage(Log.LogLevels.DETAILED, "Adding action to Rule: " + name);
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

        public List<ActionExecutor> GetActions()
        {
            return actions;
        }
    }
}
