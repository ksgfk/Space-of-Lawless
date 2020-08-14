using System;
using MPipeline;

namespace KSGFK
{
    public sealed unsafe class JobInfo<T> where T : unmanaged
    {
        public static JobInfo<T> Default => new JobInfo<T>(default, -1);

        private T* _list;
        private int _index;

        public T* Pointer => _list;
        public int Index => _index;
        public bool IsDefault => Pointer == null || Index < 0;

        public JobInfo(NativeList<T> list, int index)
        {
            _list = list.isCreated ? list.unsafePtr : null;
            _index = index;
        }

        public void SetIndex(int newIndex) { _index = newIndex; }

        public void SetPointer(NativeList<T> dataList)
        {
            if (_list != null) throw new InvalidOperationException();
            _list = dataList.unsafePtr;
        }

        public void Release()
        {
            _list = null;
            _index = -1;
        }
    }
}