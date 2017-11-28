using System;

namespace Utilities
{
    public class EventData<T> : EventArgs
    {
        public T Data { get; private set; }

        public EventData(T value)
        {
            Data = value;
        }
    }

    public class EventData<T1, T2> : EventArgs
    {
        public T1 Data1 { get; private set; }
        public T2 Data2 { get; private set; }

        public EventData(T1 value1, T2 value2)
        {
            Data1 = value1;
            Data2 = value2;
        }
    }

    public class EventData<T1, T2, T3> : EventArgs
    {
        public T1 Data1 { get; private set; }
        public T2 Data2 { get; private set; }
        public T3 Data3 { get; private set; }

        public EventData(T1 value1, T2 value2, T3 value3)
        {
            Data1 = value1;
            Data2 = value2;
            Data3 = value3;
        }
    }
}
