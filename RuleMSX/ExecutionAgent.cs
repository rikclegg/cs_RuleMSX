using System;
using System.Collections.Generic;
using System.Threading;

namespace com.bloomberg.samples.rulemsx
{
    class ExecutionAgent {

        static readonly object dataSetLock = new object();
        static readonly object openSetLock = new object();
        List<WorkingRule> workingSet = new List<WorkingRule>();
        List<WorkingRule> openSetQueue = new List<WorkingRule>();
        List<WorkingRule> openSet = new List<WorkingRule>();
        Queue<DataSet> dataSetQueue = new Queue<DataSet>();
        Thread workingSetAgent;
        RuleSet ruleSet;
        volatile bool stop = false;

        internal ExecutionAgent(RuleSet ruleSet, DataSet dataSet) {

            this.ruleSet = ruleSet;

            addDataSet(dataSet);

            ThreadStart agent = new ThreadStart(WorkingSetAgent);
            workingSetAgent = new Thread(agent);
            workingSetAgent.Start();

        }

        internal void addDataSet (DataSet dataSet)
        {
            lock(dataSetLock)
            {
                dataSetQueue.Enqueue(dataSet);
            }
        }

        internal void Stop() {
            this.stop = true;
        }

        internal void WorkingSetAgent() {

            while (!stop)
            {

                // Ingest any new DataSets
                while (dataSetQueue.Count > 0)
                {
                    DataSet ds;
                    lock (dataSetLock) {
                        ds = dataSetQueue.Dequeue();
                    }
                    ingestDataSet(this.ruleSet, ds, null);
                }

                while (openSetQueue.Count > 0)
                {

                    lock (openSetLock)
                    {
                        openSet = openSetQueue;
                        openSetQueue = new List<WorkingRule>();
                    }

                    foreach (WorkingRule wr in openSet)
                    {
                        if (wr.evaluator.Evaluate(wr.dataSet))
                        {
                            foreach (WorkingRule nwr in wr.workingRules) AddToOpenSetQueue(nwr);
                            foreach (RuleAction a in wr.actions) a.Execute(wr.dataSet);
                        }
                    }
                }
            }
        }

        private void ingestDataSet(RuleContainer rc, DataSet dataSet, WorkingRule parent)
        {
            // Create WorkingRule object for each DataPoint of each DataSet for each Rule in RuleSet.
            foreach(Rule r in rc.GetRules())
            {
                WorkingRule wr = new WorkingRule(this, r, dataSet);
                workingSet.Add(wr);
                if (parent != null) parent.addWorkingRule(wr);
                ingestDataSet(r, dataSet, wr);
                if (rc is RuleSet)
                {
                    AddToOpenSetQueue(wr); // Add alpha nodes to openSet queue
                }
            }
        }

        internal void AddToOpenSetQueue(WorkingRule wr)
        {
            lock(openSetLock) {
                if(!openSet.Contains(wr)) openSetQueue.Add(wr); //Only add working rule if it's not already in the queue
            }
        }

    }
}
