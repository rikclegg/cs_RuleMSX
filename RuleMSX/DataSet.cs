using System.Collections.Generic;

namespace com.bloomberg.samples.rulemsx
{

    public class DataSet
    {

        private string name;
        private Dictionary<string, DataPoint> dataPoints;

        DataSet(string name)
        {
            this.name = name;
            this.dataPoints = new Dictionary<string, DataPoint>();
        }

        public DataPoint addDataPoint(string name)
        {

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
    }
}