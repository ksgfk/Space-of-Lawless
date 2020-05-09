using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace KSGFK
{
    public enum LoadState
    {
        Sleep,
        Ready,
        Working,
        Finish
    }

    public class LoadManager : MonoBehaviour
    {
        [SerializeField] private LoadState nowState;
        private Queue<AsyncOperationHandle> _requestQueue;
        private Action _onLoadFinish;
        private Coroutine _workingCoroutine;

        public void Init()
        {
            _requestQueue = new Queue<AsyncOperationHandle>();
            nowState = LoadState.Sleep;
        }

        public void Ready()
        {
            if (nowState != LoadState.Sleep) throw new InvalidOperationException("休眠状态才能准备加载");
            nowState = LoadState.Ready;
        }

        public void Request<T>(string address, Action<T> callback) where T : UnityEngine.Object
        {
            var handle = Addressables.LoadAssetAsync<T>(address);
            handle.Completed += handleCallback => { callback?.Invoke(handleCallback.Result); };
            Request(handle);
        }

        public void Request<T>(AsyncOperationHandle<T> handle) where T : UnityEngine.Object
        {
            Request((AsyncOperationHandle) handle);
        }

        public void Request(AsyncOperationHandle handle)
        {
            if (nowState != LoadState.Ready) throw new InvalidOperationException("准备状态才能添加请求");
            _requestQueue.Enqueue(handle);
        }

        public void Work(Action callback)
        {
            if (nowState != LoadState.Ready) throw new InvalidOperationException("准备状态才能开始工作");
            _onLoadFinish = callback;
            nowState = LoadState.Working;
            _workingCoroutine = StartCoroutine(QueueAsyncOpHandles());
        }

        private IEnumerator QueueAsyncOpHandles()
        {
            while (_requestQueue.Count > 0)
            {
                var head = _requestQueue.Peek();
                if (head.IsDone)
                {
                    _requestQueue.Dequeue();
                }

                yield return null;
            }

            nowState = LoadState.Finish;
            _onLoadFinish?.Invoke();
            Finish();
        }

        public void ForceStop()
        {
            if (nowState != LoadState.Working) throw new InvalidOperationException("工作状态才能强制停止查询");
            StopCoroutine(_workingCoroutine);
            Finish();
        }

        private void Finish()
        {
            _onLoadFinish = null;
            _workingCoroutine = null;
            nowState = LoadState.Sleep;
        }
    }
}