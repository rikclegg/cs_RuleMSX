using System.Collections.Generic;

namespace com.bloomberg.samples.rulemsx
{

    public class DataSet
    {

        private string name;
        private Dictionary<string, DataPoint> dataPoints;

        internal DataSet(string name)
        {
            Log.LogMessage(Log.LogLevels.DETAILED, "DataSet constructor: " + name);
            this.name = name;
            this.dataPoints = new Dictionary<string, DataPoint>();
        }

        public DataPoint addDataPoint(string name)
        {
            Log.LogMessage(Log.LogLevels.BASIC, "Adding DataPoint: " + name + " to DataSet: " + this.name);
            DataPoint newDataPoint = new DataPoint(this, name);
            dataPoints.Add(name, newDataPoint);
            return newDataPoint;
        }

        public string getName()
        {
            return this.name;
        }

        public DataPoint getDataPoint(string name)
        {
            return dataPoints[name];
        }

        public Dictionary<string, DataPoint> getDataPoints()
        {
            return this.dataPoints;
        }

    }
}