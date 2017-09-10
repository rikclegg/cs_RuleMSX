using System.Collections.Generic;

namespace com.bloomberg.samples.rulemsx
{

    public class RuleMSX
    {

        public enum DataPointState
        {
            STALE,
            CURRENT
        }

        List<DataSet> dataSets;
        List<RuleSet> ruleSets;

        public RuleMSX()
        {
            dataSets = new List<DataSet>();
            ruleSets = new List<RuleSet>();
        }

        public DataSet createDataSet(string name)
        {
            DataSet newDataSet = new DataSet(name);
            dataSets.Add(newDataSet);
            return newDataSet;
        }

        public RuleSet createRuleSet(string name)
        {
            RuleSet newRuleSet = new RuleSet(name);
            ruleSets.Add(newRuleSet);
            return newRuleSet;
        }

        public List<DataSet> getDataSets()
        {
            return this.dataSets;
        }

        public List<RuleSet> getRuleSets()
        {
            return this.ruleSets;
        }
    }
}
