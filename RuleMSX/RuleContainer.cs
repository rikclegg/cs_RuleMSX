using System.Collections.Generic;

namespace com.bloomberg.samples.rulemsx {

    public abstract class RuleContainer
    {
        internal List<Rule> rules = new List<Rule>();

        public void AddRule(Rule newRule)
        {
            if (this is RuleSet) {
                RuleSet rs = (RuleSet)this;
                Log.LogMessage(Log.LogLevels.DETAILED, "Adding child Rule: " + newRule.GetName() + " to RuleSet: " + rs.getName());
            } else
            {
                Rule r = (Rule)this;
                Log.LogMessage(Log.LogLevels.DETAILED, "Adding child Rule: " + newRule.GetName() + " to Rule: " + r.GetName());
            }

            rules.Add(newRule);
        }

        public List<Rule> GetRules()
        {
            return this.rules;
        }

        public Rule GetRule(string name)
        {
            foreach (Rule r in this.rules)
            {
                if (r.GetName().Equals(name)) return r;
            }
            return null;
        }
    }
}
