using System;
using System.Threading.Tasks;

namespace KSGFK
{
    /// <summary>
    /// 对带返回值的Task的包装
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TaskWrapper<T> : IAsyncHandleWrapper
    {
        private readonly Task<T> _result;
        private readonly Action<T> _callback;

        public bool IsDone => _result.IsCompleted;

        public TaskWrapper(Task<T> task, Action<T> callback)
        {
            _result = task;
            _callback = callback;
        }

        public void OnComplete() { _callback?.Invoke(_result.Result); }

        public static TaskWrapper<T> Build(Task<T> t, Action<T> callback) { return new TaskWrapper<T>(t, callback); }
    }
}