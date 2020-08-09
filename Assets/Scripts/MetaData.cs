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
            public string EntryType;
            public string DataType;

            public Info(string path, string entryType, string dataType)
            {
                Path = path;
                EntryType = entryType;
                DataType = dataType;
            }
        }

        public Info[] EntityInfo;
        public Info[] JobInfo;
        public Info[] ItemInfo;
        public Info[] WorldInfo;
    }
}