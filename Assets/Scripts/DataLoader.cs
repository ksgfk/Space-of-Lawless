using System;
using System.Collections.Generic;

namespace KSGFK
{
    public abstract class DataLoader
    {
        /// <summary>
        /// 适用平台
        /// </summary>
        public string Platform { get; }

        /// <summary>
        /// 适用文件类型
        /// </summary>
        public string FileExt { get; }

        protected DataLoader(string platform, string fileExt)
        {
            Platform = platform;
            FileExt = fileExt;
        }

        /// <summary>
        /// 开始加载
        /// </summary>
        /// <param name="type">数据类型</param>
        /// <param name="path">数据索引</param>
        /// <param name="result">数据储存,key是数据索引,value是所有数据</param>
        /// <returns>加载请求</returns>
        public abstract IAsyncHandleWrapper StartLoad(
            Type type,
            string path,
            Dictionary<string, IReadOnlyList<object>> result);
    }
}