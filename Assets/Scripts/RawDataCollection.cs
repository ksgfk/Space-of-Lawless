using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KSGFK
{
    public readonly struct RawDataInfo
    {
        public string Path { get; }
        public Type DataType { get; }
        public object Data { get; }

        public RawDataInfo(string path, Type dataType, object data)
        {
            Path = path;
            DataType = dataType;
            Data = data;
        }
    }

    public class RawDataCollection : IEnumerable<RawDataInfo>
    {
        private readonly Dictionary<string, (Type, List<object>)> _collection;

        internal Dictionary<string, (Type, List<object>)> RawDataStruct => _collection;

        public RawDataCollection() { _collection = new Dictionary<string, (Type, List<object>)>(); }

        public void Push(string path, Type type, object data)
        {
            if (_collection.TryGetValue(path, out var value))
            {
                var (t, list) = value;
                if (type != t)
                {
                    throw new InvalidOperationException();
                }

                list.Add(data);
            }
            else
            {
                var list = new List<object> {data};
                _collection.Add(path, (type, list));
            }
        }

        public void Push(string path, Type type, IEnumerable<object> ie)
        {
            if (_collection.TryGetValue(path, out var value))
            {
                var (t, list) = value;
                if (type != t)
                {
                    throw new InvalidOperationException();
                }

                list.AddRange(ie);
            }
            else
            {
                _collection.Add(path, (type, ie.ToList()));
            }
        }

        public IEnumerator<RawDataInfo> GetEnumerator()
        {
            foreach (var pair in _collection)
            {
                var path = pair.Key;
                var type = pair.Value.Item1;
                var list = pair.Value.Item2;
                foreach (var data in list)
                {
                    yield return new RawDataInfo(path, type, data);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        public IEnumerable<object> Query(string path)
        {
            if (_collection.TryGetValue(path, out var pair))
            {
                return pair.Item2;
            }

            return Array.Empty<object>();
        }
    }
}