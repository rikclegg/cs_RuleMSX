/* Copyright 2017. Bloomberg Finance L.P.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to
deal in the Software without restriction, including without limitation the
rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
sell copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:  The above
copyright notice and this permission notice shall be included in all copies
or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace com.bloomberg.samples.rulemsx
{
    class ExecutionAgent {

        RuleSet ruleSet;
        static readonly object dataSetLock = new object();
        Thread workingSetAgent;
        volatile bool running = true;
        Queue<DataSet> dataSetQueue = new Queue<DataSet>();
        List<WorkingRule> openSetQueue = new List<WorkingRule>();
        List<WorkingRule> openSet = new List<WorkingRule>();
        static readonly object openSetLock = new object();
        List<WorkingRule> workingSet = new List<WorkingRule>();


        internal ExecutionAgent(RuleSet ruleSet, DataSet dataSet) {

            Log.LogMessage(Log.LogLevels.DETAILED, "ExecutionEngine constructor for RuleSet: " + ruleSet.GetName());

            this.ruleSet = ruleSet;

            addDataSet(dataSet);

            Log.LogMessage(Log.LogLevels.DETAILED, "Creating thread for WorkingSetAgent for RuleSet: " + ruleSet.GetName());

            ThreadStart agent = new ThreadStart(WorkingSetAgent);
            workingSetAgent = new Thread(agent);
            Log.LogMessage(Log.LogLevels.DETAILED, "Starting thread for WorkingSetAgent for RuleSet: " + ruleSet.GetName());
            workingSetAgent.Start();
        }

        internal void addDataSet (DataSet dataSet)
        {
            Log.LogMessage(Log.LogLevels.DETAILED, "Adding DataSet to DataSet queue for RuleSet: " + this.ruleSet.GetName() + " and DataSet: " + dataSet.getName());

            lock (dataSetLock)
            {
                dataSetQueue.Enqueue(dataSet);
            }
        }

        internal bool Stop() {
            Log.LogMessage(Log.LogLevels.DETAILED, "Stoping thread for WorkingSetAgent for RuleSet: " + ruleSet.GetName());
            this.running = false;
            try
            {
                workingSetAgent.Join();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        internal void WorkingSetAgent() {

            Log.LogMessage(Log.LogLevels.DETAILED, "Running WorkingSetAgent for " + ruleSet.GetName());

            while (running)
            {

                // Ingest any new DataSets
                while (dataSetQueue.Count > 0)
                {
                    DataSet ds;
                    lock (dataSetLock) {
                        ds = dataSetQueue.Dequeue();
                        ingestDataSet(this.ruleSet, ds, null);
                    }
                }

                Stopwatch cycleTime = System.Diagnostics.Stopwatch.StartNew();

                while (openSetQueue.Count > 0)
                {

                    Log.LogMessage(Log.LogLevels.DETAILED, "OpenSetQueue not empty...");

                    lock (openSetLock)
                    {
                        Log.LogMessage(Log.LogLevels.DETAILED, "Move OpenSetQueue to OpenSet, reset OpenSetQueue");
                        openSet = openSetQueue;
                        openSetQueue = new List<WorkingRule>();
                    }

                    // We need to cache the values for each datapoint that underlies a WorkingRule in the Open set.
                    // This guarentees that each datapoint is referencing the same 'generation' of values.
                    // TODO
                    //Maybe!!! I think this may be resovled by managing the openset correcty.

                    foreach (WorkingRule wr in openSet)
                    {

                        Log.LogMessage(Log.LogLevels.DETAILED, "Evaluating WorkingRule for Rule: " + wr.getRule().GetName() + " with DataSet: " + wr.dataSet.getName());
                        if (wr.evaluator.Evaluate(wr.dataSet))
                        {
                            Log.LogMessage(Log.LogLevels.DETAILED, "Evaluator returned True");
                            foreach (WorkingRule nwr in wr.workingRules) {
                                Log.LogMessage(Log.LogLevels.DETAILED, "Add WorkingRule for Rule: " + nwr.getRule().GetName() + " to OpenSetQueue");
                                AddToOpenSetQueue(nwr);
                            }
                            foreach (ActionExecutor a in wr.actionExecutors) {
                                Log.LogMessage(Log.LogLevels.DETAILED, "Executing Action for Rule: " + wr.getRule().GetName());
                                a.Execute(wr.dataSet);
                            }
                        }
                        else Log.LogMessage(Log.LogLevels.DETAILED, "Evaluator returned False");

                    }
                }

                cycleTime.Stop();
                long dur = cycleTime.ElapsedMilliseconds;
                if(dur>0) ruleSet.setLastCycleTime(dur);
            }
        }

        private void ingestDataSet(RuleContainer rc, DataSet dataSet, WorkingRule parent)
        {
            Log.LogMessage(Log.LogLevels.DETAILED, "Ingesting dataSet " + dataSet.getName() + " for " + ruleSet.getName() + " agent");

            // Create WorkingRule object for each DataPoint of each DataSet for each Rule in RuleSet.
            foreach (Rule r in rc.GetRules())
            {
                Log.LogMessage(Log.LogLevels.DETAILED, "Creating WorkingRule for Rule: " + r.GetName());

                WorkingRule wr = new WorkingRule(this, r, dataSet, parent);
                workingSet.Add(wr);
                if (parent != null) {
                    Log.LogMessage(Log.LogLevels.DETAILED, "Adding WorkingRule to parent");
                    parent.addWorkingRule(wr);
                }
                if (r.GetRules().Count > 0)
                {
                    Log.LogMessage(Log.LogLevels.DETAILED, "Checking chained rules...");
                    ingestDataSet(r, dataSet, wr);
                }
                if (rc is RuleSet)
                {
                    AddToOpenSetQueue(wr); // Add alpha nodes to openSet queue
                }
            }
        }

        internal void AddToOpenSetQueue(WorkingRule wr)
        {

            //When adding to the OpenSet queue, the following rules should be obeyed :-
            //
            // 1. WorkingRule with same identifier must not already be in the queue
            // 2. If ancestor of this WorkingRule is already in the queue, do not add this workingRule
            // 3. If this rule is an ancestor of a working rule in the queue, that rule must be removed from the queue.
            //TODO
            lock(openSetLock) {
                if (!openSetQueue.Contains(wr) && !hasAncestor(wr, openSetQueue)) {
                    //Only add working rule if it's not already in the queue, and if there isn't already an ancestor in the queue
                    removeDecendants(wr.workingRules, openSetQueue);

                    Log.LogMessage(Log.LogLevels.DETAILED, "Adding WorkingRule to OpenSetQueue");
                    openSetQueue.Add(wr); 
                }
                else Log.LogMessage(Log.LogLevels.DETAILED, "...ignored (already in queue or ancestor already in queue)");
            }
        }

        private bool hasAncestor(WorkingRule wr, List<WorkingRule> q)
        {

            // Slow! Can be optimized using branch address that contains parent address for each WorkingRule. TODO!!
            WorkingRule target = wr.parent;

            while (target!=null)
            {
                foreach(WorkingRule w in q)
                {
                    if (target.Equals(w)) return true;
                }
                target = target.parent;
            }
            return false;
        }

        private void removeDecendants(List<WorkingRule> decendants, List<WorkingRule> q)
        {
            // Remove any decendant working rules from the openSetQueue
            // Slow!!! Must find other way of doing this!! TODO

            foreach (WorkingRule target in decendants) 
            {
                foreach(WorkingRule wr in q)
                {
                    if (wr.Equals(target)) q.Remove(wr);
                }
                removeDecendants(target.workingRules, q);
            }
        }
    }
}
