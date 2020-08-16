using MPipeline;
using Unity.Collections;
using Unity.Jobs;

namespace KSGFK
{
    public abstract class TempJobWrapper<TInput, TOutput> : JobWrapper
        where TInput : struct
        where TOutput : unmanaged
    {
        private NativeList<TOutput> _dataList;

        protected NativeList<TOutput> DataList => _dataList;
        
        public virtual void AddValue(TInput input)
        {
            if (!_dataList.isCreated)
            {
                _dataList = new NativeList<TOutput>(6, Allocator.Temp);
            }

            _dataList.Add(ConvertData(ref input));
        }

        protected abstract TOutput ConvertData(ref TInput input);

        public sealed override JobHandle OnUpdate() { return !_dataList.isCreated ? default : Update(); }

        protected abstract JobHandle Update();

        public override void AfterUpdate()
        {
            if (!_dataList.isCreated)
            {
                return;
            }

            _dataList.Dispose();
            _dataList = default;
        }

        public override void Dispose() { _dataList.Dispose(); }
    }
}