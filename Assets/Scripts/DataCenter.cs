using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace KSGFK
{
    public class DataCenter
    {
        private readonly Dictionary<string, IReadOnlyList<object>> _data;
        private List<(string, Type)> _tempDataPath;
        private readonly Dictionary<string, DataLoader> _loaders;

        internal IDictionary<string, IReadOnlyList<object>> DataDict => _data;

        public DataCenter()
        {
            _data = new Dictionary<string, IReadOnlyList<object>>();
            _loaders = new Dictionary<string, DataLoader>();
            _tempDataPath = new List<(string, Type)>();
        }

        public void AddDataLoader(string fileExt, DataLoader loader)
        {
            try
            {
                _loaders.Add(fileExt, loader);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        public DataLoader GetDataLoader(string fileExt) { return _loaders[fileExt]; }

        public void StartLoad()
        {
            var load = GameManager.Load;
            load.Complete += () =>
            {
                Debug.LogFormat("load data complete");
                foreach (var kv in _data)
                {
                    Debug.LogFormat("[Count:{0}][Path:{1}]", kv.Value.Count, kv.Key);
                }
            };
#if UNITY_EDITOR_WIN
            var platform = RuntimePlatform.WindowsPlayer.ToString();
#else
            var platform = Application.platform.ToString();
#endif
            foreach (var (path, type) in _tempDataPath)
            {
                var ext = Path.GetExtension(path);
                if (!_loaders.TryGetValue($"{platform}{ext}", out var loader))
                {
                    Debug.LogWarningFormat("[平台{1}]:未找到文件类型{2}的Loader,忽略.[路径{0}]", path, platform, ext);
                    continue;
                }

                try
                {
                    var wrapper = loader.StartLoad(type, path);
                    load.Request(wrapper);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }

            _tempDataPath = null;
        }

        public void AddPath(Type dataType, string path) { _tempDataPath.Add((path, dataType)); }

        public IEnumerable<T> Query<T>(string path)
        {
            IEnumerable<object> enumerable;
            if (_data.TryGetValue(path, out var data))
            {
                enumerable = data;
            }
            else
            {
                Debug.LogWarningFormat("不存在路径:{0},查询失败", path);
                enumerable = new T[0].Cast<object>();
            }

            return enumerable.Cast<T>();
        }
    }
}