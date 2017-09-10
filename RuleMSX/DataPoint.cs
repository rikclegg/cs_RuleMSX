using System.Collections.Generic;

namespace com.bloomberg.samples.rulemsx
{
    public class DataPoint
    {

        private string name;
        private List<DataPoint> dependencies;
        private DataPointSource source;
        private DataSet dataSet;

        public DataPoint(DataSet dataSet, string name)
        {
            this.name = name;
            this.dataSet = dataSet;
            this.dependencies = new List<DataPoint>();
        }

        public string GetName()
        {
            return this.name;
        }

        public void SetDataPointSource(DataPointSource source)
        {
            this.source = source;
        }

        public DataPointSource GetSource()
        {
            return this.source;
        }

        public DataSet GetDataSet()
        {
            return this.dataSet;
        }

        public void AddDependency(DataPoint dependency)
        {
            this.dependencies.Add(dependency);
        }
    }
}