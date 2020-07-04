using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KSGFK
{
    public class CsvLoader : DataLoader
    {
        protected readonly Dictionary<Type, Func<string, object>> parsers;

        public Dictionary<Type, Func<string, object>> Parser => parsers;

        public CsvLoader()
        {
            parsers = new Dictionary<Type, Func<string, object>>
            {
                {typeof(string), str => str},
                {typeof(int), str => int.Parse(str)},
                {typeof(float), str => float.Parse(str)},
                {typeof(double), str => double.Parse(str)},
                {typeof(bool), str => bool.Parse(str)},
                {
                    typeof(Color), str =>
                    {
                        var c = typeof(Color);
                        var color = c.GetProperty(str, BindingFlags.Public | BindingFlags.Static);
                        if (color != null)
                        {
                            return color.GetGetMethod().Invoke(null, null);
                        }

                        var rgba = str.Split(';').Select(rgb => int.Parse(rgb) / 255f).ToArray();
                        switch (rgba.Length)
                        {
                            case 3:
                                return new Color(rgba[0], rgba[1], rgba[2]);
                            case 4:
                                return new Color(rgba[0], rgba[1], rgba[2], rgba[3]);
                            default:
                                throw new ArgumentException("颜色是RGBA格式且用;分开");
                        }
                    }
                },
                {typeof(ulong), str => ulong.Parse(str)}
            };
        }

        public override IAsyncHandleWrapper StartLoad(Type type, string path)
        {
            var task = Read(type, path);
            return new CsvLoaderWrapper(task, path);
        }

        private async Task<IEnumerable<object>> Read(Type type, string path)
        {
            var fields =
                type.GetAllInheritedFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var fieldsDict = fields.ToDictionary(fieldInfo => fieldInfo.Name);
            using (var reader = new StreamReader(path, Encoding.UTF8))
            {
                var firstLine = await reader.ReadLineAsync();
                if (string.IsNullOrEmpty(firstLine))
                {
                    return Array.CreateInstance(type, 0).Cast<object>();
                }

                var n = firstLine.Split(',');
                var list = new List<object>();
                var lineNum = 1;
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }

                    var split = line.Split(',');
                    var count = n.Length == split.Length
                        ? n.Length
                        : throw new ArgumentException($"[行:{lineNum}]:列数量[{split.Length}]与第一行[{n.Length}]不匹配");
                    var instance = Activator.CreateInstance(type);
                    for (var i = 0; i < count; i++)
                    {
                        var val = split[i];
                        if (!fieldsDict.TryGetValue(n[i], out var field))
                        {
                            Debug.LogWarningFormat("不存在字段{0}", n[i]);
                            continue;
                        }

                        var fieldType = field.FieldType;
                        if (parsers.TryGetValue(fieldType, out var func))
                        {
                            object pauseVal;
                            try
                            {
                                pauseVal = func(val);
                            }
                            catch (Exception e)
                            {
                                Debug.LogError(e);
                                pauseVal = null;
                            }

                            field.SetValue(instance, pauseVal);
                        }
                        else
                        {
                            throw new ArgumentException($"不支持赋值的类型{fieldType}");
                        }
                    }

                    list.Add(instance);
                    lineNum += 1;
                }

                return list;
            }
        }
    }

    internal class CsvLoaderWrapper : IAsyncHandleWrapper
    {
        private readonly string _path;
        private readonly Type _dataType;
        private readonly Task<IEnumerable<object>> _result;

        public bool IsDone => _result.IsCompleted;

        internal CsvLoaderWrapper(Task<IEnumerable<object>> task, string path)
        {
            _path = path;
            _result = task;
        }

        public void OnComplete() { GameManager.Data.DataDict.Add(_path, _result.Result.ToArray()); }
    }
}