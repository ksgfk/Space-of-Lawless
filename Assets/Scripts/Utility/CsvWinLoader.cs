using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using UnityEngine;

namespace KSGFK
{
    /// <summary>
    /// Windows平台的CSV格式文件加载
    /// </summary>
    public static class CsvWinLoader
    {
        public static readonly List<ClassMap> ClassMaps = new List<ClassMap>();

        static CsvWinLoader() { ClassMaps.Add(new ItemGunInfoMap()); }

        public static async Task<RegisterData> ReadAsync(string path, Type dataType)
        {
            var strReader = new StreamReader(path, Encoding.UTF8);
            var csvReader = new CsvReader(strReader, CultureInfo.CurrentCulture);
            foreach (var classMap in ClassMaps)
            {
                csvReader.Configuration.RegisterClassMap(classMap);
            }

            var list = new List<object>();
            var asyncEnumerator = csvReader.GetRecordsAsync(dataType).GetAsyncEnumerator();
            try
            {
                while (await asyncEnumerator.MoveNextAsync())
                {
                    var human = asyncEnumerator.Current;
                    list.Add(human);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"无法读取:{path}");
                Debug.LogError(e);
            }
            finally
            {
                await asyncEnumerator.DisposeAsync();
            }

            csvReader.Dispose();
            strReader.Dispose();
            return new RegisterData(path, dataType, list);
        }
    }
}