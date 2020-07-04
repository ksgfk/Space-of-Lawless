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
        private Queue<IAsyncHandleWrapper> _requestQueue;
        private Coroutine _workingCoroutine;
        private Action _completeCallback;

        public LoadState NowState => nowState;

        public event Action Complete
        {
            add
            {
                if (nowState != LoadState.Ready) throw new InvalidOperationException("准备状态才能添加回调");
                _completeCallback += value;
            }
            remove
            {
                if (nowState != LoadState.Ready) throw new InvalidOperationException("准备状态才能删除回调");
                _completeCallback = (Action) Delegate.Remove(_completeCallback, value);
            }
        }

        public void Init() { nowState = LoadState.Sleep; }

        public void Ready()
        {
            if (nowState != LoadState.Sleep) throw new InvalidOperationException("休眠状态才能准备加载");
            _requestQueue = new Queue<IAsyncHandleWrapper>();
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

        public void Request(AsyncOperationHandle handle) { Request(new AddrAsyncWrapper(handle)); }

        public void Request(IAsyncHandleWrapper wrapper)
        {
            if (nowState != LoadState.Ready)
            {
                if (nowState != LoadState.Working)
                {
                    throw new InvalidOperationException("准备,工作状态才能添加请求");
                }

                _requestQueue.Enqueue(wrapper);
            }
            else
            {
                _requestQueue.Enqueue(wrapper);
            }
        }

        public void Work()
        {
            if (nowState != LoadState.Ready) throw new InvalidOperationException("准备状态才能开始工作");
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
                    head.OnComplete();
                    _requestQueue.Dequeue();
                }

                yield return null;
            }

            nowState = LoadState.Finish;
            _completeCallback?.Invoke();
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
            _completeCallback = null;
            _workingCoroutine = null;
            _requestQueue = null;
            nowState = LoadState.Sleep;
        }
    }
}