using System;
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
            if (name == null || name == "") throw new ArgumentException("DataSet name cannot be null or empty");
            DataSet newDataSet = new DataSet(name);
            Log.LogMessage(Log.LogLevels.DETAILED, "Adding new DataSet " + newDataSet.getName() + " to DataSets list.");
            dataSets.Add(newDataSet);
            Log.LogMessage(Log.LogLevels.BASIC, "New DataSet created: " + newDataSet.getName());
            return newDataSet;
        }

        public RuleSet createRuleSet(string name)
        {
            Log.LogMessage(Log.LogLevels.BASIC, "Creating RuleSet: " + name);
            if (name == null || name == "") throw new ArgumentException("DataSet name cannot be null or empty");
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

        public RuleSet getRuleSet(string name)
        {
            foreach(RuleSet rs in ruleSets) {
                if (rs.getName().Equals(name)) return rs;
            }
            return null;
        }

        public DataSet getDataSet(string name)
        {
            foreach (DataSet ds in dataSets)
            {
                if (ds.getName().Equals(name)) return ds;
            }
            return null;
        }

        public bool Stop()
        {
            Log.LogMessage(Log.LogLevels.BASIC, "Stopping all RuleSet agents.");
            bool result = true;

            foreach (RuleSet rs in this.ruleSets)
            {
                Log.LogMessage(Log.LogLevels.DETAILED, "Stopping RuleSet: " + rs.getName());
                if (!rs.Stop()) result = false;
            }

            return result;
        }
    }
}
