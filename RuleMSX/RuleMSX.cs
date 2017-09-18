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
            Log.LogMessage(Log.LogLevels.BASIC, "Instantiating RuleMSX");

            dataSets = new List<DataSet>();
            ruleSets = new List<RuleSet>();

            Log.LogMessage(Log.LogLevels.BASIC, "Instantiating RuleMSX complete.");
        }

        public DataSet createDataSet(string name)
        {
            Log.LogMessage(Log.LogLevels.BASIC, "Creating DataSet: " + name);
            DataSet newDataSet = new DataSet(name);
            Log.LogMessage(Log.LogLevels.DETAILED, "Adding new DataSet " + newDataSet.getName() + " to DataSets list.");
            dataSets.Add(newDataSet);
            Log.LogMessage(Log.LogLevels.BASIC, "New DataSet created: " + newDataSet.getName());
            return newDataSet;
        }

        public RuleSet createRuleSet(string name)
        {
            Log.LogMessage(Log.LogLevels.BASIC, "Creating RuleSet: " + name);
            RuleSet newRuleSet = new RuleSet(name);
            Log.LogMessage(Log.LogLevels.DETAILED, "Adding new RuleSet " + newRuleSet.getName() + " to RuleSets list.");
            ruleSets.Add(newRuleSet);
            Log.LogMessage(Log.LogLevels.BASIC, "New RuleSet created: " + newRuleSet.getName());
            return newRuleSet;
        }

        public List<DataSet> getDataSets()
        {
            Log.LogMessage(Log.LogLevels.DETAILED, "Get DataSets");
            return this.dataSets;
        }

        public List<RuleSet> getRuleSets()
        {
            Log.LogMessage(Log.LogLevels.DETAILED, "Get RuleSets");
            return this.ruleSets;
        }

        public void Stop()
        {
            Log.LogMessage(Log.LogLevels.BASIC, "Stopping all RuleSet agents.");
            foreach (RuleSet rs in this.ruleSets)
            {
                Log.LogMessage(Log.LogLevels.DETAILED, "Stopping RuleSet: " + rs.getName());
                rs.Stop();
            }
        }
    }
}
