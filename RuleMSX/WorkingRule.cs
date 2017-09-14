using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.bloomberg.samples.rulemsx
{
    class WorkingRule
    {
        Rule rule;
        RuleEvaluator evaluator;
        List<RuleAction> actions;
        List<WorkingRule> workingRules;

        internal WorkingRule(Rule r, RuleEvaluator e, List<RuleAction> a) {
            this.rule = r;
            this.evaluator = e;
            this.actions = a;
        }

        internal Rule getRule() {
            return this.rule;
        }

        internal void addWorkingRule(WorkingRule wr) {
            this.workingRules.Add(wr);
        }
    }
}
