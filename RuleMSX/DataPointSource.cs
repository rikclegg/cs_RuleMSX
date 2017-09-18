using System;
using System.Collections.Generic;
using static com.bloomberg.samples.rulemsx.RuleMSX;

namespace com.bloomberg.samples.rulemsx
{

    public abstract class DataPointSource
    {
        internal List<RuleEventHandler> ruleEventHandlers = new List<RuleEventHandler>();
        private DataPoint dataPoint;
        public abstract object GetValue();

        public void SetStale() {
            foreach (RuleEventHandler h in ruleEventHandlers)
            {
                h.handleRuleEvent();
            }
        }

        internal void setDataPoint(DataPoint dataPoint) {
            this.dataPoint = dataPoint;
        }

        public DataPoint getDataPoint() {
            return this.dataPoint;
        }

        internal void addRuleEventHandler(RuleEventHandler handler) {
            this.ruleEventHandlers.Add(handler);
        }
    }
}