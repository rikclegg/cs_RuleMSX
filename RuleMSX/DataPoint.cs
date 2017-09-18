using System.Collections.Generic;

namespace com.bloomberg.samples.rulemsx
{
    public class DataPoint
    {

        private string name;
        private DataPointSource source;
        private DataSet dataSet;

        internal DataPoint(DataSet dataSet, string name)
        {
            this.name = name;
            this.dataSet = dataSet;
        }

        public string GetName()
        {
            return this.name;
        }

        public void SetDataPointSource(DataPointSource source)
        {
            source.setDataPoint(this);
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
    }
}