using System;
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

        internal string ruleContainerReport(string prefix, bool isLast)
        {
            string report = "";
            string subPrefix;
            string indent;

            if (isLast)
            {
                indent = "+-- ";
                subPrefix = prefix + "\x9";  
            } else
            {
                indent = "|-- ";
                subPrefix = prefix + "|" + "\x9";
            }

            report = report + prefix + indent + "Rule: " + ((Rule)this).GetName() + "\n";

            for(int i=0; i <((Rule)this).GetEvaluator().dependantDataPointNames.Count; i++)
            {
                if ((i == ((Rule)this).GetEvaluator().dependantDataPointNames.Count - 1) && ((Rule)this).GetActions().Count == 0 && ((Rule)this).GetRules().Count == 0) 
                {
                    indent = "+-- ";
                }
                else
                {
                    indent = "|-- ";
                }
                report = report + prefix + "\x9\x9" + indent + "Dep: " + ((Rule)this).GetEvaluator().dependantDataPointNames[i] + "\n";
            }

            List<Rule> reportRules = new List<Rule>();

            foreach (Rule r in this.GetRules())
            {
                reportRules.Add(r);
            }

            for (int i = 0; i < reportRules.Count; i++)
            {
                Rule r = reportRules[i];
                report = report + r.ruleContainerReport(subPrefix, (i == (reportRules.Count - 1) ? true : false));
            }

            return report;
        }
    }
}
