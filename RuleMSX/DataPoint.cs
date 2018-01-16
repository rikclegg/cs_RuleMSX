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
            Log.LogMessage(Log.LogLevels.DETAILED, "DataPoint constructor: " + name);
            this.name = name;
            this.dataSet = dataSet;
        }

        internal DataPoint(DataSet dataSet, string name, DataPointSource source)
        {
            Log.LogMessage(Log.LogLevels.DETAILED, "DataPoint constructor: " + name);
            this.name = name;
            this.dataSet = dataSet;
            this.SetDataPointSource(source);
        }

        public string GetName()
        {
            return this.name;
        }

        public void SetDataPointSource(DataPointSource source)
        {
            source.SetDataPoint(this);
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

        public object GetValue()
        {
            return this.source.GetValue();
        }
    }
}