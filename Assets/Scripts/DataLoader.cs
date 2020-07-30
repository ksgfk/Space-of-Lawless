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
        /// <param name="collection">数据储存,将加载完毕的数据存入集合</param>
        /// <returns>加载请求</returns>
        public abstract IAsyncHandleWrapper StartLoad(Type type, string path, RawDataCollection collection);
    }
}