using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KSGFK
{
    public class RegisterData
    {
        public string DataName { get; }
        public Type DataType { get; }
        public ICollection<object> Data { get; }

        public RegisterData(string dataName, Type dataType, ICollection<object> data)
        {
            DataName = dataName;
            DataType = dataType;
            Data = data;
        }
    }

    public class RegisterDataCollection : IEnumerable<RegisterData>
    {
        private readonly Dictionary<string, RegisterData> _collection;

        public Dictionary<string, RegisterData> RawDataStruct => _collection;

        public RegisterDataCollection() { _collection = new Dictionary<string, RegisterData>(); }

        public void Push(RegisterData data)
        {
            if (_collection.TryGetValue(data.DataName, out var d))
            {
                if (data.DataType == d.DataType)
                {
                    foreach (var o in data.Data)
                    {
                        d.Data.Add(o);
                    }
                }
                else
                {
                    throw new ArgumentException();
                }
            }
            else
            {
                _collection.Add(data.DataName, data);
            }
        }

        public void Push(string path, Type type, object data)
        {
            if (_collection.TryGetValue(path, out var value))
            {
                if (type != value.DataType)
                {
                    throw new ArgumentException();
                }

                value.Data.Add(data);
            }
            else
            {
                var list = new List<object> {data};
                var d = new RegisterData(path, type, list);
                _collection.Add(d.DataName, d);
            }
        }

        public void Push(string path, Type type, IEnumerable<object> ie)
        {
            if (_collection.TryGetValue(path, out var value))
            {
                if (type != value.DataType)
                {
                    throw new InvalidOperationException();
                }

                foreach (var o in ie)
                {
                    value.Data.Add(o);
                }
            }
            else
            {
                var d = new RegisterData(path, type, ie.ToList());
                _collection.Add(d.DataName, d);
            }
        }

        public IEnumerator<RegisterData> GetEnumerator() { return _collection.Values.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        public IEnumerable<object> Query(string path)
        {
            return _collection.TryGetValue(path, out var data) ? data.Data : Array.Empty<object>();
        }
    }
}