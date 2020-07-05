using UnityEngine.ResourceManagement.AsyncOperations;

namespace KSGFK
{
    /// <summary>
    /// Addressables异步请求的包装
    /// </summary>
    public class AddrLoadWrapper : IAsyncHandleWrapper
    {
        public AsyncOperationHandle Handle { get; }

        public bool IsDone => Handle.IsDone;

        public AddrLoadWrapper(AsyncOperationHandle handle) { Handle = handle; }

        public void OnComplete() { }
    }
}