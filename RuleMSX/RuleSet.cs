﻿using System.Collections.Generic;

namespace com.bloomberg.samples.rulemsx {

    public class RuleSet : RuleContainer {

        private string name;
        private ExecutionAgent executionAgent = null;

        internal RuleSet(string name) {
            Log.LogMessage(Log.LogLevels.DETAILED, "RuleSet constructor: " + name);
            this.name = name;
        }

        public string getName() {
            return this.name;
        }

        public void Execute(DataSet dataSet) {

            Log.LogMessage(Log.LogLevels.BASIC, "Execute RuleSet " + this.name + " with DataSet " + dataSet.getName());

            if (this.executionAgent == null) {
                Log.LogMessage(Log.LogLevels.DETAILED, "First execution, creating ExecutionAgent");
                this.executionAgent = new ExecutionAgent(this, dataSet);
            } else {
                Log.LogMessage(Log.LogLevels.DETAILED, "RuleSet already has ExecutionAgent, adding DataSet " + dataSet.getName());
                this.executionAgent.addDataSet(dataSet);
            }
        }

        public bool Stop()
        {
            Log.LogMessage(Log.LogLevels.BASIC, "Stoping ExecutionAgent for RuleSet " + this.name);
            if(this.executionAgent != null) return (this.executionAgent.Stop());
            else return true;
        }
    }
}