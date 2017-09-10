using System;
using static com.bloomberg.samples.rulemsx.RuleMSX;

namespace com.bloomberg.samples.rulemsx
{
    public interface DataPointSource
    {
        Object GetValue();
        DataPointState GetState();
        void SetState(DataPointState state);
    }
}