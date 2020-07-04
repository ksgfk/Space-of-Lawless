using System;

namespace KSGFK
{
    [Serializable]
    public class GameMetaData
    {
        [Serializable]
        public class DataInfo
        {
            public string Path;
            public string Type;

            public DataInfo(string path, string type)
            {
                Path = path;
                Type = type;
            }
        }

        public DataInfo[] EntityInfo;
        public DataInfo[] JobInfo;
    }
}