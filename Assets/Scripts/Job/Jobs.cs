namespace KSGFK
{
    public static class Jobs
    {
        public static readonly JobTranslate Translate;

        static Jobs() { Translate = new JobTranslate(); }

        public static void Release() { Translate.Dispose(); }

        public static void Update() { Translate.OnUpdate(); }
    }
}