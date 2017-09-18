using System.Collections.Generic;

namespace com.bloomberg.samples.rulemsx {

    public abstract class RuleEvaluator
    {
        internal List<string> dependantDataPointNames = new List<string>();

        public abstract bool Evaluate(DataSet dataSet);
        
        public void addDependantDataPointName(string name) {
            this.dependantDataPointNames.Add(name);
        }
    }
}