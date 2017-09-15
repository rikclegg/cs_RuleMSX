using System;
using System.Collections.Generic;
using System.Threading;

namespace com.bloomberg.samples.rulemsx
{
    class ExecutionAgent {

        Thread workingSetAgent;
        RuleSet ruleSet;
        DataSet dataSet;
        volatile bool stop = false;

        internal ExecutionAgent(RuleSet ruleSet, DataSet dataSet) {

            this.ruleSet = ruleSet;
            this.dataSet = dataSet;

            ThreadStart agent = new ThreadStart(WorkingSetAgent);
            Thread executionAgent = new Thread(agent);
            executionAgent.Start();

        }

        internal void Stop() {
            this.stop = true;
        }

        internal void WorkingSetAgent() {

            // Create WorkingSet
            // Create WorkingRule for each Rule in RuleSet
            // Collate datapoint references
            // Register call-backs for DataPointSource state change -> stale
            // Add alpha nodes to newOpenSet
            // while newOpenSet.count > 0 
            //    openset = newOpen
            //    execute openset
            //        create empty newOpenSet                      
            //        for each working rule in openset
            //          call evaluator
            //            if evaluator = true
            //                add each child workingrule to newOpenSet
            //                execute all actions on current workingrule
        }
    }
}
