using System.Collections.Generic;

namespace com.bloomberg.samples.rulemsx {

    public interface RuleEvaluator
    {
        bool Evaluate(DataSet dataSet);
        List<string> GetDependencies();
    }
}