using System.Collections.Generic;

namespace com.bloomberg.samples.rulemsx {

    public abstract class RuleContainer
    {
        protected List<Rule> rules = new List<Rule>();

        public void AddRule(Rule newRule)
        {
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
                if (r.getName().equals(name)) return r;
            }
            return null;
        }
    }
}
