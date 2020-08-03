using System;

namespace KSGFK
{
    [Serializable]
    public class MetaData
    {
        [Serializable]
        public class Info
        {
            public string Path;
            public string Type;

            public Info(string path, string type)
            {
                Path = path;
                Type = type;
            }
        }

        public Info[] EntityInfo;
        public Info[] JobInfo;
        public Info[] ItemInfo;
        public Info[] WorldInfo;
    }
}