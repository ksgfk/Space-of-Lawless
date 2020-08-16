using System;

namespace KSGFK
{
    public sealed class JobInfo
    {
        public static JobInfo Default => new JobInfo(default, -1);

        private JobWrapper _wrapper;
        private int _index;

        public JobWrapper Wrapper => _wrapper;
        public int Index => _index;
        public bool IsDefault => Wrapper == null || Index < 0;

        public JobInfo(JobWrapper wrapper, int index)
        {
            _wrapper = wrapper;
            _index = index;
        }

        public void SetIndex(int newIndex) { _index = newIndex; }

        public void SetWrapper(JobWrapper wrapper)
        {
            _wrapper = wrapper ?? throw new InvalidOperationException();
        }

        public void Release()
        {
            _wrapper = null;
            _index = -1;
        }
    }
}