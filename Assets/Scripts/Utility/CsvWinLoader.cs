using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using UnityEngine;

namespace KSGFK
{
    /// <summary>
    /// Windows平台的CSV格式文件加载
    /// </summary>
    public static class CsvWinLoader
    {
        private static readonly Dictionary<Type, Func<string, object>> Parsers;

        /// <summary>
        /// 字符串解释器,将csv中的字符串解析为对象
        /// </summary>
        public static IDictionary<Type, Func<string, object>> Parser => Parsers;

        static CsvWinLoader()
        {
            Parsers = new Dictionary<Type, Func<string, object>>
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

        public static IAsyncHandleWrapper StartLoad(Type type, string path, RegisterDataCollection collection)
        {
            var task = Read(type, path);
            return new TaskWrapper<IEnumerable<object>>(task, loaded => collection.Push(path, type, loaded));
        }

        public static async Task<IEnumerable<object>> Read(Type type, string path)
        {
            var fieldsDict = Helper.GetReflectionInjectFields(type).ToDictionary(f => f.Name);
            var reader = new StreamReader(path, Encoding.UTF8);
            var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var res = new List<object>();
            await csv.ReadAsync();
            if (!csv.ReadHeader())
            {
                Debug.LogWarningFormat("csv:{0},头部读取失败", path);
                return res;
            }

            while (await csv.ReadAsync())
            {
                var ins = Activator.CreateInstance(type);
                foreach (var pair in fieldsDict)
                {
                    var name = pair.Key;
                    var info = pair.Value;
                    if (!Parsers.TryGetValue(info.FieldType, out var func))
                    {
                        Debug.LogWarningFormat("csv:{0},不支持的字段类型:{1}", path, info.FieldType.FullName);
                        continue;
                    }

                    var str = csv.GetField(name);
                    if (string.IsNullOrEmpty(str))
                    {
                        Debug.LogWarningFormat("csv:{0},不存在列数据:{1}", path, name);
                        continue;
                    }

                    try
                    {
                        info.SetValue(ins, func(str));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }

                res.Add(ins);
            }

            csv.Dispose();
            reader.Dispose();
            return res;
        }

        public static async Task<RegisterData> AsyncReadRegisterData(string path, Type dataType)
        {
            var strReader = new StreamReader(path, Encoding.UTF8);
            var csvReader = new CsvReader(strReader, CultureInfo.InvariantCulture);
            await csvReader.ReadAsync();
            if (!csvReader.ReadHeader())
            {
                Debug.LogWarningFormat("csv:{0},头部读取失败", path);
                return null;
            }

            var list = new List<object>();
            var fields = dataType.GetFields();
            while (await csvReader.ReadAsync())
            {
                var ins = Activator.CreateInstance(dataType);
                foreach (var fieldInfo in fields)
                {
                    var name = fieldInfo.Name;
                    var info = fieldInfo;
                    if (!Parsers.TryGetValue(info.FieldType, out var func))
                    {
                        Debug.LogWarningFormat("csv:{0},不支持的字段类型:{1}", path, info.FieldType.FullName);
                        continue;
                    }

                    string str;
                    try
                    {
                        str = csvReader.GetField(name);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                        continue;
                    }

                    if (string.IsNullOrEmpty(str))
                    {
                        Debug.LogWarningFormat("csv:{0},不存在列数据:{1}", path, name);
                        continue;
                    }

                    try
                    {
                        info.SetValue(ins, func(str));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }

                list.Add(ins);
            }

            csvReader.Dispose();
            strReader.Dispose();
            return new RegisterData(path, dataType, list);
        }
    }
}