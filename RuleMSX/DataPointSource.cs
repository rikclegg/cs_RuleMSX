using System;
using static com.bloomberg.samples.rulemsx.RuleMSX;

namespace com.bloomberg.samples.rulemsx
{

    public abstract class DataPointSource
    {
        private DataPoint dataPoint;
        public abstract object GetValue();
        public void SetStale() {
            if(this.dataPoint != null) this.dataPoint.refresh();
        }

        internal void setDataPoint(DataPoint dataPoint) {
            this.dataPoint = dataPoint;
        }

        public DataPoint getDataPoint() {
            return this.dataPoint;
        }
    }
}