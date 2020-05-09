using UnityEngine.ResourceManagement.AsyncOperations;

namespace KSGFK
{
    public class AddrAsyncWrapper : IAsyncHandleWrapper
    {
        public AsyncOperationHandle Handle { get; }

        public bool IsDone => Handle.IsDone;

        public AddrAsyncWrapper(AsyncOperationHandle handle) { Handle = handle; }

        public void OnComplete() { }
    }
}